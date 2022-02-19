﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsRepository.Models
{
    public class Request
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(int.MaxValue)]
        public string Description { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }
        public double Lattiud { get; set; }
        public double Longtiud { get; set; }
        [Required]
        [ForeignKey("user")]
        public Guid SenderId { get; set; }
        [Required]
        [ForeignKey("sevice")]
        public int SeviceId { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime ExpireTime { get; set; }
        public bool IsAccepted { get; set; }
        public AcceptedRequest AcceptedInfo { get; set; }
        public FailedRequest FailedInfo { get; set; }
        public DeliveredRequest DeliveredInfo { get; set; }

        public List<Report> Reports { get; set; }
        public List<UserRating> Rates { get; set; }

        public virtual  User user { get; set; }
        public virtual  Service sevice { get; set; }

    }
}