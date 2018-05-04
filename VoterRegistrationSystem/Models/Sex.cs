using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoterRegistrationSystem.Models
{
    [Table("genders")]
    public class Sex
    {
        [Key]
        public int SexID { get; set; }
        public string SexTitle { get; set; }
    }
}