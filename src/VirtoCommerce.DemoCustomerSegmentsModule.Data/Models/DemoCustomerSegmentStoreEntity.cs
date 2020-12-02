using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.DemoSolutionFeaturesModule.Data.Models
{
    public class DemoCustomerSegmentStoreEntity: Entity
    {
        public string CustomerSegmentId { get; set; }

        public DemoCustomerSegmentEntity CustomerSegment { get; set; }

        [Required]
        [StringLength(128)]
        public string StoreId { get; set; }
    }
}
