using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsRepository.Models
{
    public class Service
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("user")]
        public Guid UserId { get; set; }

        [ForeignKey("service_type")]
        public int TypeId { get; set; }
        public int Gender { get; set; }
        public int AgeFrom { get; set; }
        public int AgeTo { get; set; }
        public List<ServiceAttachment> Attachments { get; set; }
        public List<Request> Requests { get; set; }

        public virtual User user { get; set; }
        public virtual ServiceType service_type { get; set; }
    }
}

