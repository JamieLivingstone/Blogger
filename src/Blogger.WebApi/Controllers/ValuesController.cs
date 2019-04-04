using Microsoft.AspNetCore.Mvc;

namespace Blogger.WebApi.Controllers
{
    [Route("/api/values")]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetValues()
        {
            return Ok(new [] { 1, 2, 3, 4 });
        }
    }
}