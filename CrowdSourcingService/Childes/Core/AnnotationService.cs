using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Childes.DataContracts;
using Childes.ServiceContracts;
using CrowdSourcing.Core;
using CrowdSourcing.DataContracts;
using CrowdSourcing.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Childes.Core
{
    public class AnnotationService : IAnnotationService
    {
        private IDataProvider dataProvider;
        private ITaskGenerator taskGenerator;
        private ITaskResolver taskResolver;
        private IMediaStreamer mediaStreamer;

        public AnnotationService(IDataProvider dataProvider, ITaskGenerator taskGenerator, ITaskResolver taskResolver, IMediaStreamer mediaStreamer)
        {
            this.dataProvider = dataProvider;
            this.taskGenerator = taskGenerator;
            this.taskResolver = taskResolver;
            this.mediaStreamer = mediaStreamer;
        }

        public IEnumerable<IAnnotationTask> GetAnnotationTasks(int numberOfTasks)
        {
            IEnumerable<ITranscriptElement> elements = dataProvider.GetNLeastAnnotated(numberOfTasks);
            List<IAnnotationTask> annotationTasks = new List<IAnnotationTask>();
            foreach (ITranscriptElement element in elements)
            {
                IContextInfo contextInfo = this.dataProvider.GetContextInfo(element);
                IEnumerable<ITranscriptElement> contextTranscripts = this.dataProvider.GetContextContent(contextInfo);

                IAnnotationTask annotationTask = this.taskGenerator.CreateTask(contextTranscripts, contextInfo, element.Properties["video_id"].ToString());
                annotationTasks.Add(annotationTask);
            }

            return annotationTasks;
        }

        public IEnumerable<IAnnotationTask> GetPracticeTasks()
        {
            IEnumerable<ITranscriptElement> elements = dataProvider.GetPracticeTasks();
            List<IAnnotationTask> annotationTasks = new List<IAnnotationTask>();
            foreach (ITranscriptElement element in elements)
            {
                IContextInfo contextInfo = this.dataProvider.GetContextInfo(element);
                IEnumerable<ITranscriptElement> contextTranscripts = this.dataProvider.GetContextContent(contextInfo);

                IAnnotationTask annotationTask = this.taskGenerator.CreateTask(contextTranscripts, contextInfo, element.Properties["video_id"].ToString());
                annotationTasks.Add(annotationTask);
            }

            return annotationTasks;
        }

        public IActionResult GetMedia(string mediaId)
        {
            return this.mediaStreamer.Stream(this.dataProvider.GetMedia(mediaId));
        }

        public bool HasWorker(string workerId)
        {
            return this.dataProvider.AddWorker(workerId);
        }

        public void RecordAnnotations(IEnumerable<IAnnotationTask> annotationTasks, string workerId, string assignmentId)
        {
            //string uniqueId = Guid.NewGuid().ToString();
            foreach (IAnnotationTask annotationTask in annotationTasks)
            {
                string correlationId = null;
                if (string.IsNullOrWhiteSpace(annotationTask.CorrelationId))
                {
                    throw new Exception("Missing correlation ID");
                }

                if (correlationId == null)
                {
                    correlationId = annotationTask.CorrelationId;
                }

                if (!string.Equals(annotationTask.CorrelationId, correlationId, StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("Multiple Correlation IDs");
                }

                ITranscriptElement transcript = this.taskResolver.ResolveAnnotation(annotationTask);
                //annotationTask.Annotation.Properties["annotator"] = uniqueId;
                //add verification step here? Check for... annotationTask.Annotation length? 
                this.dataProvider.AddAnnotation(transcript, annotationTask.Annotation, workerId, assignmentId);
            }
            //return uniqueId;
        }

        public void RecordMetadata(string id, IEnumerable<IAnnotationTask> metadata)
        {
            foreach (IAnnotationTask fakeAnnotationTask in metadata)
            {
                string annotator = id;

                try
                {
                    this.dataProvider.AddAnnotatorInformation(annotator, fakeAnnotationTask.Annotation);
                }
                catch (Exception)
                {
                }
            }
        }

        public bool CheckPractice(IAnnotationTask practiceChunk, string section)
        {
            string correlationId = null;
            if (string.IsNullOrWhiteSpace(practiceChunk.CorrelationId))
            {
                throw new Exception("Missing correlation ID");
            }

            if (correlationId == null)
            {
                correlationId = practiceChunk.CorrelationId;
            }

            if (!string.Equals(practiceChunk.CorrelationId, correlationId, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Multiple Correlation IDs");
            }

            ITranscriptElement transcript = this.taskResolver.ResolveAnnotation(practiceChunk);
            IPropertyBag correctAnno = this.dataProvider.GetAnnotation(transcript.Id);
            return checkCorrectness(correctAnno, practiceChunk.Annotation, section);
        }

        private bool checkCorrectness(IPropertyBag correctAnno, IPropertyBag userAnno, string section)
        {
            if (userAnno == null)
            {
                return false;
            }
            if (!userAnno.Properties.Keys.Contains(section) || !correctAnno.Properties.Keys.Contains(section))
            {
                return false;
            }
            if (userAnno.Properties[section] is IList)
            {
                string userPropertyString = userAnno.Properties[section].ToString();
                string userStripped = new string(userPropertyString.Where(
                    c =>
                    {
                        return (char.IsLetterOrDigit(c) || c == ',' || c == '-');
                    }
                ).ToArray());
                string[] userPropertyParts = userStripped.Split(",");
                Array.Sort(userPropertyParts);

                string correctPropertyString = correctAnno.Properties[section].ToString();
                string correctStripped = new string(correctPropertyString.Where(
                    c =>
                    {
                        return (char.IsLetterOrDigit(c) || c == ',' || c == '-');
                    }
                ).ToArray());
                string[] correctPropertyParts = correctStripped.Split(",");
                if (section.Equals("syntacticProperties"))
                {
                    if (correctPropertyParts.Contains("pronominal")) { correctPropertyParts = correctPropertyParts.Select(x => x.Replace("pronominal", "pronoun")).ToArray(); }
                    if (correctPropertyParts.Contains("demonstrative")) { correctPropertyParts = correctPropertyParts.Select(x => x.Replace("demonstrative", "pronoun")).ToArray(); }
                }
                Array.Sort(correctPropertyParts);
                return correctPropertyParts.ContainSameItems(userPropertyParts);
            }
            else if (userAnno.Properties[section] is string)
            {
                string userProperty = (string)userAnno.Properties[section];
                string correctProperty = (string)correctAnno.Properties[section];
                return userProperty.Equals(correctProperty);
            }

            return false;
        }

        private List<string> convertToList(object property)
        {
            JsonSerializer serializer = new JsonSerializer();
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, property);
                using (StreamReader sr = new StreamReader(ms))
                {
                    using (JsonReader jsonReader = new JsonTextReader(sr))
                    {
                        return serializer.Deserialize<List<string>>(jsonReader);
                    }
                }
            }
        }
    }

    public static class ListEqualityExtension {
        public static bool ContainSameItems(this IList<string> lhs, IList<string> rhs)
        {
            bool equal = true;
            if (lhs != null && rhs != null)
            {
                if (lhs.Count != rhs.Count)
                {
                    equal = false;
                }
                else
                {
                    foreach (string prop in rhs)
                    {
                        if (!lhs.Contains(prop))
                        {
                            equal = false;
                        }
                    }
                }
            }
            else
            {
               equal = false;
            }

            return equal;
        }
    }

}
