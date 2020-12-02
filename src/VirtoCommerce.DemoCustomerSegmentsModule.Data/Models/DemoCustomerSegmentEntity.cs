using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.DemoSolutionFeaturesModule.Core.Models;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.DemoSolutionFeaturesModule.Data.Models
{
    public class DemoCustomerSegmentEntity: AuditableEntity
    {
        [Required]
        [StringLength(128)]
        public string Name { get; set; }
        
        [Required]
        [StringLength(1024)]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string ExpressionTreeSerialized { get; set; }

        public virtual ObservableCollection<DemoCustomerSegmentStoreEntity> Stores { get; set; } = new NullCollection<DemoCustomerSegmentStoreEntity>();

        public virtual DemoCustomerSegment ToModel(DemoCustomerSegment customerSegment)
        {
            if (customerSegment == null)
            {
                throw new ArgumentNullException(nameof(customerSegment));
            }

            customerSegment.Id = Id;
            customerSegment.CreatedBy = CreatedBy;
            customerSegment.CreatedDate = CreatedDate;
            customerSegment.ModifiedBy = ModifiedBy;
            customerSegment.ModifiedDate = ModifiedDate;

            customerSegment.Name = Name;
            customerSegment.Description = Description;
            customerSegment.IsActive = IsActive;
            customerSegment.StartDate = StartDate;
            customerSegment.EndDate = EndDate;

            customerSegment.ExpressionTree = AbstractTypeFactory<DemoCustomerSegmentTree>.TryCreateInstance();
            if (ExpressionTreeSerialized != null)
            {
                customerSegment.ExpressionTree = JsonConvert.DeserializeObject<DemoCustomerSegmentTree>(ExpressionTreeSerialized, new ConditionJsonConverter());
            }

            if (Stores != null)
            {
                customerSegment.StoreIds = Stores.Select(x => x.StoreId).ToList();
            }

            return customerSegment;
        }

        public virtual DemoCustomerSegmentEntity FromModel(DemoCustomerSegment customerSegment, PrimaryKeyResolvingMap pkMap)
        {
            if (customerSegment == null)
            {
                throw new ArgumentNullException(nameof(customerSegment));
            }

            pkMap.AddPair(customerSegment, this);

            Id = customerSegment.Id;
            CreatedBy = customerSegment.CreatedBy;
            CreatedDate = customerSegment.CreatedDate;
            ModifiedBy = customerSegment.ModifiedBy;
            ModifiedDate = customerSegment.ModifiedDate;

            Name = customerSegment.Name;
            Description = customerSegment.Description;
            IsActive = customerSegment.IsActive;
            StartDate = customerSegment.StartDate;
            EndDate = customerSegment.EndDate;

            if (customerSegment.ExpressionTree != null)
            {
                ExpressionTreeSerialized = JsonConvert.SerializeObject(customerSegment.ExpressionTree, new ConditionJsonConverter(doNotSerializeAvailCondition: true));
            }

            if (customerSegment.StoreIds != null)
            {
                Stores = new ObservableCollection<DemoCustomerSegmentStoreEntity>(customerSegment.StoreIds.Select(x => new DemoCustomerSegmentStoreEntity { StoreId = x, CustomerSegmentId = customerSegment.Id }));
            }

            return this;
        }

        public virtual void Patch(DemoCustomerSegmentEntity target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            target.Name = Name;
            target.Description = Description;
            target.IsActive = IsActive;
            target.StartDate = StartDate;
            target.EndDate = EndDate;
            target.ExpressionTreeSerialized = ExpressionTreeSerialized;

            if (!Stores.IsNullCollection())
            {
                var comparer = AnonymousComparer.Create((DemoCustomerSegmentStoreEntity entity) => entity.StoreId);
                Stores.Patch(target.Stores, comparer, (sourceEntity, targetEntity) => targetEntity.StoreId = sourceEntity.StoreId);
            }
        }
    }
}
