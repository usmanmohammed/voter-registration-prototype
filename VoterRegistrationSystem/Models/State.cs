using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoterRegistrationSystem.Models
{
    [Table("states")]
    public class State
    {
        public int StateID { get; set; }
        public string StateName { get; set; }
        public virtual ICollection<Lga> LGAs { get; set; }
    }
}