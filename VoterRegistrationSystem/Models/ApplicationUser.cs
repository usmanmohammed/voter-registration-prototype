using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoterRegistrationSystem.Models
{
    [Table("administrators")]
    public class ApplicationUser
    {
        [Key]
        public int OfficerID { get; set; }
        [ForeignKey("OfficerID")]
        public virtual RegistrationOfficer RegistrationOfficer { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
    }
}
