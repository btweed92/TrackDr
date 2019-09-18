using System;
using System.Collections.Generic;

namespace TrackDr.Models
{
    public partial class ParentDoctor
    {
        public int ParentDoctorId { get; set; }
        public string DoctorId { get; set; }
        public string ParentId { get; set; }

        public virtual Doctor Doctor { get; set; }
        public virtual Parent Parent { get; set; }
    }
}
