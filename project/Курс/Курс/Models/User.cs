using System;
using System.Collections.Generic;

namespace Курс.Models
{
    public partial class User
    {
        public User()
        {
            Requests = new HashSet<Request>();
        }

        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public string? Surname { get; set; }
        public string Email { get; set; } = null!;
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int Role { get; set; }

        public virtual Role RoleNavigation { get; set; } = null!;
        public virtual ICollection<Request> Requests { get; set; }
    }
}
