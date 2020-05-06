using System.Collections.Generic;

namespace CrowdSourcing.DataContracts
{
    public interface IContextInfo : IPropertyBag
    {
        string Id { get; set; }

        string CorrelationId { get; set; }

        IEnumerable<string> TranscriptIds { get; set; }
    }
}
