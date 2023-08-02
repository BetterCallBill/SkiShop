using API.Errors;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BugController : BaseApiController
    {

        public BugController()
        {
        }

        [HttpGet("testauth")]
        [Authorize]
        public ActionResult<string> GetSecretText()
        {
            return "you are authorized";
        }

        [HttpGet("notfound")]
        public IActionResult GetNotFoundResult()
        {
            return NotFound(new ApiResponse(404));
        }
    }
}