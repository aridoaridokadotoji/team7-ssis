﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace team7_ssis.Models
{
    public class Retrieval
    {
        [Key]
        [MaxLength(20)]
        public String RetrievalId { get; set; }
        public Status Status { get; set; }
        public ApplicationUser CreatedBy { get; set; }
        public ApplicationUser UpdatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        [InverseProperty("Retrieval")]
        public List<Requisition> Requisitions { get; set; }
        [InverseProperty("Retrieval")]
        public List<Disbursement> Disbursements { get; set; }
    }
}