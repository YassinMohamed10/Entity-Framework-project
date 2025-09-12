using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinQProject.Models
{
    internal class EmployeeProject
    {
        public int EmployeeId { get; set; }

        public int ProjectId { get; set; }

        public string Role { get; set; } = null!;

        public virtual Employee Employee { get; set; } = null!;

        public virtual Project Project { get; set; } = null!;
    }
}
