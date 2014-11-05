// Copyright (C) 2014 Del Mayhak
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSlackers.AspNet.Identity.EntityFramework.Entities
{
    public class IdentityHostDomain
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Index("IX_Host", 0)]
        public long HostId { get; set; }

        [Required]
        [MaxLength(250)]
        [Index("IX_Domain")]
        public string DomainName { get; set; }
    }
}
