using CrowdSourcing.DataContracts;

namespace CrowdSourcing.ServiceContracts
{
    public interface IAnnotationTask
    {
        string CorrelationId { get; set; }

        IAnnotation Annotation { get; set; }

        IPhraseConstruct Phrase { get; set; }

        IPhraseContext Context { get; set; }
    }
}
