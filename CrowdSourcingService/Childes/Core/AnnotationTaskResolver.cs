using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Childes.DataContracts;
using CrowdSourcing.Core;
using CrowdSourcing.DataContracts;
using CrowdSourcing.ServiceContracts;

namespace Childes.Core
{
    public class AnnotationTaskResolver : ITaskResolver
    {
        public ITranscriptElement ResolveAnnotation(IAnnotationTask annotationTask)
        {
            Utterance utterance = new Utterance()
            {
                Id = annotationTask.Phrase.Id,
                Phrase = annotationTask.Phrase.Phrase,
                Author = annotationTask.Phrase.Author,
            };

            utterance.SetProperty("annotation", annotationTask.Annotation);
            utterance.SetProperty("bucket_id", annotationTask.CorrelationId);

            return utterance;
        }
    }
}
