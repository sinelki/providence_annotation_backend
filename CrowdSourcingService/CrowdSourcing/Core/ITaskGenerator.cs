using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrowdSourcing.DataContracts;
using CrowdSourcing.ServiceContracts;

namespace CrowdSourcing.Core
{
    public interface ITaskGenerator
    {
        IAnnotationTask CreateTask(IEnumerable<ITranscriptElement> elements, IContextInfo contextInfo, string mediaId);
    }
}
