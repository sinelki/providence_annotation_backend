using System.Collections.Generic;

namespace CrowdSourcing.DataContracts
{
    public interface IPropertyBag
    {
        IDictionary<string, object> Properties { get; set; }
    }
}
