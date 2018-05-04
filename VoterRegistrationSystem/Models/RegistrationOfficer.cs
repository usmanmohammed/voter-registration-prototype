using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoterRegistrationSystem.Models
{
    [Table("authorized_operators")]
    public class RegistrationOfficer
    {
        [Key]
        public int OfficerID { get; set; }
        public string OfficerName { get; set; }
        public string AddedBy { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
