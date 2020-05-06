using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CrowdSourcing.Utils
{
    public class PushStreamResult : IActionResult
    {
        private readonly Action<ActionContext> _onStreamAvailabe;
        private readonly string _contentType;

        public PushStreamResult(Action<ActionContext> onStreamAvailabe, string contentType)
        {
            _onStreamAvailabe = onStreamAvailabe;
            _contentType = contentType;
        }

        public Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.GetTypedHeaders().ContentType = new MediaTypeHeaderValue(_contentType);
            _onStreamAvailabe(context);
            return Task.CompletedTask;
        }
    }
}
