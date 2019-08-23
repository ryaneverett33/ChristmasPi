using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ChristmasPi.Util {
    public class NotImplementedResult : IActionResult {
        public Task ExecuteResultAsync(ActionContext context) {
            context.HttpContext.Response.StatusCode = 501;
            context.HttpContext.Response.ContentType = "application/text";
            using (var streamWriter = new StreamWriter(context.HttpContext.Response.Body)) {
                streamWriter.WriteLine("Not Implemented Yet");
                streamWriter.Flush();
            }
            return Task.FromResult<NotImplementedResult>(this);
        }
    }
}
