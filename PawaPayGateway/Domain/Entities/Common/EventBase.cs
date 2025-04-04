using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PawaPayGateway.Domain.Enums;

namespace PawaPayGateway.Domain.Entities.Common
{
    public class EventBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public EventTypeEnum EventType { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
