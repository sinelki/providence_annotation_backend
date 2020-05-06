using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrowdSourcing.ServiceContracts
{
    public interface IPhraseConstruct
    {
        string Id { get; set; }

        string Author { get; set; }

        string Phrase { get; set; }
    }
}
