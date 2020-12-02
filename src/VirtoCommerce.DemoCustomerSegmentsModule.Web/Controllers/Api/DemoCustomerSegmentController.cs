using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.DemoSolutionFeaturesModule.Core;
using VirtoCommerce.DemoSolutionFeaturesModule.Core.Models;
using VirtoCommerce.DemoSolutionFeaturesModule.Core.Models.Search;
using VirtoCommerce.DemoSolutionFeaturesModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.DemoSolutionFeaturesModule.Web.Controllers.Api
{
    [Route("/api/demo/customersegments")]
    public class DemoCustomerSegmentController: Controller
    {
        private readonly IDemoCustomerSegmentService _customerSegmentService;
        private readonly IDemoCustomerSegmentSearchService _customerSegmentSearchService;

        public DemoCustomerSegmentController(IDemoCustomerSegmentService customerSegmentService,
            IDemoCustomerSegmentSearchService customerSegmentSearchService)
        {
            _customerSegmentService = customerSegmentService;
            _customerSegmentSearchService = customerSegmentSearchService;
        }

        /// <summary>
        /// Get new customer segment object 
        /// </summary>
        /// <remarks>Return a new customer segment object with populated dynamic expression tree</remarks>
        [HttpGet]
        [Route("new")]
        [Authorize(ModuleConstants.Security.Permissions.Create)]
        public ActionResult<DemoCustomerSegment> GetNewCustomerSegment()
        {
            var result = AbstractTypeFactory<DemoCustomerSegment>.TryCreateInstance();

            result.ExpressionTree = AbstractTypeFactory<DemoCustomerSegmentTree>.TryCreateInstance();
            result.ExpressionTree.MergeFromPrototype(AbstractTypeFactory<DemoCustomerSegmentTreePrototype>.TryCreateInstance());
            result.IsActive = true;

            return Ok(result);
        }

        /// <summary>
        /// Get customer segment by ID
        /// </summary>
        /// <param name="id">Customer segment ID</param>
        [HttpGet]
        [Route("{id}")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public async Task<ActionResult<DemoCustomerSegment>> GetCustomerSegmentById([FromRoute] string id)
        {
            var result = (await _customerSegmentService.GetByIdsAsync(new[] { id })).FirstOrDefault();

            result?.ExpressionTree?.MergeFromPrototype(AbstractTypeFactory<DemoCustomerSegmentTreePrototype>.TryCreateInstance());

            return Ok(result);
        }

        /// <summary>
        /// Create/Update customer segments.
        /// </summary>
        /// <param name="customerSegments">The customer segments.</param>
        [HttpPost]
        [Route("")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public async Task<ActionResult<DemoCustomerSegment[]>> SaveCustomerSegments([FromBody] DemoCustomerSegment[] customerSegments)
        {
            if (!customerSegments.IsNullOrEmpty())
            {
                await _customerSegmentService.SaveChangesAsync(customerSegments);
            }

            return Ok(customerSegments);
        }

        /// <summary>
        /// Deletes customer segments by IDs.
        /// </summary>
        /// <param name="ids">Customer segment IDs.</param>
        [HttpDelete]
        [Route("")]
        [Authorize(ModuleConstants.Security.Permissions.Delete)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteCustomerSegments([FromQuery] string[] ids)
        {
            await _customerSegmentService.DeleteAsync(ids);

            return NoContent();
        }

        /// <summary>
        /// Search customer segments by specified search criteria.
        /// </summary>
        /// <param name="criteria">Search criteria</param>
        /// <returns>Search result with total number of found customer segments and all found customer segments.</returns>
        [HttpPost]
        [Route("search")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public async Task<ActionResult<DemoCustomerSegmentSearchResult>> SearchCustomerSegments([FromBody] DemoCustomerSegmentSearchCriteria criteria)
        {
            var result = await _customerSegmentSearchService.SearchCustomerSegmentsAsync(criteria);

            return Ok(result);
        }

        private static void ValidateParameters(DemoCustomerSegmentConditionEvaluationRequest conditionRequest)
        {
            if (conditionRequest == null)
            {
                throw new ArgumentNullException(nameof(conditionRequest));
            }
        }
    }
}
