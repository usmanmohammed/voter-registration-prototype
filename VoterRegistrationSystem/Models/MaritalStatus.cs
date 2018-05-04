using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoterRegistrationSystem.Models
{
    [Table("marital_statuses")]
    public class MaritalStatus
    {
        public int MaritalStatusID { get; set; }
        public string StatusTitle { get; set; }
    }
}
