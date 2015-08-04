using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSlackers.AspNet.Identity.EntityFramework.Entities
{
    public class IdentityHost<TKey>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Index("IX_Id")]
        public TKey HostId { get; set; }

        [Required]
        [MaxLength(250)]
        [Index("IX_Name")]
        public string Name { get; set; }

        public bool IsSystemHost { get; set; }

        ICollection<IdentityHostDomain> Domains { get; set; }
    }

    public class IdentityHostString : IdentityHost<string>
    {
    }

    public class IdentityHostGuid : IdentityHost<Guid>
    {
    }

    public class IdentityHostInt : IdentityHost<int>
    {
    }

    public class IdentityHostLong : IdentityHost<long>
    {
    }
}
