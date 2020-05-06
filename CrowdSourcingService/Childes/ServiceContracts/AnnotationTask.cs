using System.Collections.Generic;
using CrowdSourcing.DataContracts;
using CrowdSourcing.ServiceContracts;
using Newtonsoft.Json;

namespace Childes.ServiceContracts
{
    [JsonObject]
    public class AnnotationTask : IAnnotationTask
    {
        public AnnotationTask(
            string correlationId,
            NounPhrase phrase,
            NounPhraseContext context, 
            Annotation annotation = null)
        {
            this.Phrase = phrase;
            this.Context = context;
            this.CorrelationId = correlationId;
            this.Annotation = annotation;
        }

        [JsonProperty("id")]
        public string CorrelationId { get; set; }

        [JsonProperty("annotation", NullValueHandling = NullValueHandling.Ignore)]
        public IAnnotation Annotation { get; set; }

        [JsonProperty("phrase", NullValueHandling = NullValueHandling.Ignore)]
        public IPhraseConstruct Phrase { get; set; }

        [JsonProperty("context", NullValueHandling = NullValueHandling.Ignore)]
        public IPhraseContext Context { get; set; }
    }
}
