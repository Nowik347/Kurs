using System;
using System.Collections.Generic;

namespace Курс.Models
{
    public partial class State
    {
        public State()
        {
            Requests = new HashSet<Request>();
        }

        public int StatusId { get; set; }
        public string StatusName { get; set; } = null!;

        public virtual ICollection<Request> Requests { get; set; }
    }
}
