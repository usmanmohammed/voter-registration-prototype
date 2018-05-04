using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoterRegistrationSystem.Models
{
    [Table("local_governments")]
    public class Lga
    {
        public int LgaID { get; set; }
        public int StateID { get; set; }
        public virtual State State { get; set; }
        public string LgaName { get; set; }
        public virtual ICollection<Ward> Wards { get; set; }

    }
}