using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Childes.ServiceContracts;
using CrowdSourcing.Core;
using CrowdSourcing.DataContracts;
using CrowdSourcing.ServiceContracts;

namespace Childes.Core
{
    public class AnnotationTaskGenerator : ITaskGenerator
    {
        public IAnnotationTask CreateTask(IEnumerable<ITranscriptElement> elements, IContextInfo contextInfo, string mediaId)
        {
            // The phrase to be annotated in the annotation task.
            NounPhrase phrase = null;

            List<string> contextInternal = new List<string>();
            string contextFormat = "[{0}]: {1}";

            NounPhraseContext context = new NounPhraseContext()
            {
                Id = contextInfo.Id,
            };

            int contextIndex = 0;
            foreach (ITranscriptElement transcript in elements)
            {
                if (transcript.Id == contextInfo.CorrelationId)
                {
                    phrase = new NounPhrase()
                    {
                        Id = transcript.Id,
                        Author = transcript.Author,
                        Phrase = transcript.Phrase,
                    };

                    context.FocusBegin = contextIndex;
                    context.FocusEnd = contextIndex;
                }

                contextInternal.Add(string.Format(contextFormat, transcript.Author, transcript.Phrase));
                contextIndex += 1;
            }

            context.Context = contextInternal;

            return new AnnotationTask(correlationId: mediaId, phrase: phrase, context: context);
        }
    }
}
