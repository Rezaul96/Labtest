using LabTest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace LabTest.Api.Helpers
{
    public class LabTestControllerBase: ControllerBase
    {
        public Registration CurrentUser
        {
            get
            {
                var str = Request.Cookies["user"];
                if (!string.IsNullOrEmpty(str))
                {
                    return JsonConvert.DeserializeObject<Registration>(HttpUtility.UrlDecode(str), new JsonSerializerSettings()
                    {
                        NullValueHandling = NullValueHandling.Include,

                        Error = (sender, args) => { args.ErrorContext.Handled = true; }
                    });
                }

                return null;
            }
        }

        public bool IsAdminRequest => Request.Headers["UserAgent"].Contains("admin.Labtest.com");

        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual IActionResult InternalServerError(Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e);
        }
    }
}
