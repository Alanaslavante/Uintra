﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using uIntra.Core.Persistence;

namespace uIntra.Core.Activity
{
    [uIntraTable("Activity")]
    public class IntranetActivityEntity : SqlEntity<Guid>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override Guid Id { get; set; }

        [StringLength(int.MaxValue)]
        public string JsonData { get; set; }

        public int Type { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifyDate { get; set; }
    }
}