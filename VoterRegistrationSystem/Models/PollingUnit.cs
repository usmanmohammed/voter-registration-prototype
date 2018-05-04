using System.ComponentModel.DataAnnotations.Schema;

namespace VoterRegistrationSystem.Models
{
    [Table("polling_units")]
    public class PollingUnit
    {
        public int PollingUnitID { get; set; }
        public int WardID { get; set; }
        public virtual Ward Ward { get; set; }
        public string PollingUnitName { get; set; }
    }
}