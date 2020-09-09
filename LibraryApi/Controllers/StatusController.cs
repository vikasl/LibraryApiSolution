using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryApi.Controllers
{
    public class StatusController : ControllerBase
    {

        
        // GET /status -> StatusController#GetStatus
        [HttpGet("/status")]
        public ActionResult GetStatus()
        {
            var status = new StatusResponse
            {
                Message = "Looks good on my end. Party On!",
                CheckedBy = "Joe Schmidt",
                WhenChecked = DateTime.Now
            };

            return Ok(status);
        }
    }

    public class StatusResponse
    {
        public string Message { get; set; }
        public string CheckedBy { get; set; }
        public DateTime WhenChecked { get; set; }
    }
}
