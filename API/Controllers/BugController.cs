using API.Errors;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BugController : BaseApiController
    {
        private readonly StoreContext _storeContext;

        public BugController(StoreContext storeContext)
        {
            _storeContext = storeContext;
        }

        [HttpGet("notfound")]
        public IActionResult GetNotFoundResult()
        {
            return NotFound(new ApiResponse(404));
        }
    }
}