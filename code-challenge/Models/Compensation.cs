using System;
using System.ComponentModel.DataAnnotations;

namespace challenge.Models
{
    public class Compensation
    {
        // TODO This really should have its own primary key and not share it with Employee. Done for convenience.
        [Key]
        public String EmployeeId { get; set; }

        // TODO Consider creating a Currency class that follows the Money pattern
        public Decimal Salary { get; set; }

        public DateTime EffectiveDate { get; set; }
    }
}
