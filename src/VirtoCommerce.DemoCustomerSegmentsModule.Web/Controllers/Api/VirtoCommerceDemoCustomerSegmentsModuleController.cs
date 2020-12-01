using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.DemoCustomerSegmentsModule.Core;


namespace VirtoCommerce.DemoCustomerSegmentsModule.Web.Controllers.Api
{
    [Route("api/VirtoCommerceDemoCustomerSegmentsModule")]
    public class VirtoCommerceDemoCustomerSegmentsModuleController : Controller
    {
        // GET: api/VirtoCommerceDemoCustomerSegmentsModule
        /// <summary>
        /// Get message
        /// </summary>
        /// <remarks>Return "Hello world!" message</remarks>
        [HttpGet]
        [Route("")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public ActionResult<string> Get()
        {
            return Ok(new { result = "Hello world!" });
        }
    }
}
