using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrowdSourcing.DataContracts;
using CrowdSourcing.ServiceContracts;

namespace CrowdSourcing.Core
{
    public interface ITaskResolver
    {
        ITranscriptElement ResolveAnnotation(IAnnotationTask annotationTask);
    }
}
