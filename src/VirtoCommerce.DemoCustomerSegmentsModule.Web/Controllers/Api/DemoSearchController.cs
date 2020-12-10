using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.DemoCustomerSegmentsModule.Web.Controllers.Api
{
    [Route("/api/demo/search")]
    [Produces("application/json")]
    [Authorize]
    public class DemoSearchController : Controller
    {
        private readonly ISearchProvider _searchProvider;

        public DemoSearchController(ISearchProvider searchProvider)
        {
            _searchProvider = searchProvider;
        }

        /// <summary>
        /// Get search index for specified document type and document id.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("index/{documentType}/{documentId}")]
        public async Task<ActionResult<SearchDocument>> GetDocumentIndexAsync(string documentType, string documentId)
        {
            var request = new SearchRequest
            {
                Filter = new IdsFilter
                {
                    Values = new[] { documentId },
                },
            };

            var result = await _searchProvider.SearchAsync(documentType, request);
            return Ok(result.Documents.FirstOrDefault());
        }
    }
}
