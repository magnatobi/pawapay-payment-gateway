using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PawaPayGateway.Domain.Entities.Common
{
    //This will serve as common fields for domain
    //This means, every entity will have below props by default in ordering Microservice
    public abstract class EntityBase
    {
        //protected EntityBase() 
        //{
        //    LastModifiedOn = DateTime.Now;        
        //}

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public bool IsDeleted { get; set; }


        //Below Properties are Audit properties
        public Guid CreatedBy { get; set; } = Guid.Empty;
        public DateTime CreatedOn { get; set; }
        public Guid LastModifiedBy { get; set; } = Guid.Empty;
        public DateTime LastModifiedOn { get; set; }
    }
}
