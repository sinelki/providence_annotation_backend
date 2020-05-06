using CrowdSourcing.DataContracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CrowdSourcing.Core
{
    public interface IMediaStreamer
    {
        IActionResult Stream(IMedia media);

        void OnStreamAvailable(ActionContext context);
    }
}
