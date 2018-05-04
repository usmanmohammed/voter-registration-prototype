using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoterRegistrationSystem.Models
{
    [Table("wards")]
    public class Ward
    {
        public int WardID { get; set; }
        public int LgaID { get; set; }
        public virtual Lga Lga { get; set; }
        public string WardName { get; set; }
        public virtual ICollection<PollingUnit> PollingUnits { get; set; }

    }
}
