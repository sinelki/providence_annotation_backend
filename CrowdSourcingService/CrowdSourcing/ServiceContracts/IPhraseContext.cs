using System.Collections.Generic;
using CrowdSourcing.DataContracts;

namespace CrowdSourcing.ServiceContracts
{
    public interface IPhraseContext
    {
        string Id { get; set; }

        int FocusBegin { get; set; }

        int FocusEnd { get; set; }

        IEnumerable<string> Context { get; set; }
    }
}
