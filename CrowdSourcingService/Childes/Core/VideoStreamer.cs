using Childes.DataContracts;
using CrowdSourcing.Core;
using CrowdSourcing.DataContracts;
using CrowdSourcing.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Childes.Core
{
    public class VideoStreamer : IMediaStreamer
    {
        private const long BufferLength = 65536;

        private IMedia media;

        public void OnStreamAvailable(ActionContext context)
        {
            Stream outputStream = context.HttpContext.Response.Body;
            try
            {
                using (Stream videoStream = new MediaStream(this.media))
                {
                    byte[] buffer = new byte[BufferLength];
                    long length = videoStream.Length;
                    long bytesRead = 1;

                    context.HttpContext.Response.GetTypedHeaders().ContentLength = length;
                    context.HttpContext.Response.GetTypedHeaders().ContentRange = new ContentRangeHeaderValue(0, length - 1, length);

                    while (length > 0 && bytesRead > 0)
                    {
                        bytesRead = videoStream.Read(buffer);
                        outputStream.Write(buffer, 0, (int)Math.Min(bytesRead, length));
                        length -= bytesRead;
                    }
                }
            }
            catch (Exception ex)
            {
                return;
            }
            finally
            {
                // Closes connection to client.
                outputStream.Close();
                this.media = null;
            }
        }

        public IActionResult Stream(IMedia media)
        {
            this.media = media;
            return new PushStreamResult(this.OnStreamAvailable, "video/mp4");
        }
    }
}
