using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrowdSourcing.ServiceContracts;
using Newtonsoft.Json;

namespace Childes.ServiceContracts
{
    [JsonObject]
    public class Annotation : IAnnotation
    {
        public Annotation()
        {
            this.Properties = new Dictionary<string, object>();
        }

        [JsonExtensionData]
        public IDictionary<string, object> Properties { get; set; }
    }
}
