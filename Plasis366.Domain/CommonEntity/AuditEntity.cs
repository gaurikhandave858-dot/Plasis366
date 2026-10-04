using System;
using System.Collections.Generic;
using System.Text;

namespace Plasis366.Domain
{
    public class AuditEntity
    {
        public long? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }=DateTime.Now;

        public long? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }= DateTime.Now;

        public bool IsActive { get; set; } = true;
        public long VersionNumber { get; set; }
    }
}
