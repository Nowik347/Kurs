using System;
using System.Collections.Generic;

namespace Курс.Models
{
    public partial class Request
    {
        public int RequestId { get; set; }
        public int UserId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? LastChangeDate { get; set; }
        public string TroubleDevices { get; set; } = null!;
        public string ProblemDescription { get; set; } = null!;
        public int Status { get; set; }

        public virtual State StatusNavigation { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
