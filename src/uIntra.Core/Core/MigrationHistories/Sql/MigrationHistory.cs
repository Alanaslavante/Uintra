﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Uintra.Core.Persistence;

namespace Uintra.Core.MigrationHistories.Sql
{
    [UintraTable("MigrationHistory")]
    public class MigrationHistory : SqlEntity<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        public DateTime CreateDate { get; set; }

        [StringLength(50)]
        public string Version { get; set; }
    }
}
