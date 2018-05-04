
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoterRegistrationSystem.Models
{
    [Table("fp_fingerprints")]
    public class Fingerprint
    {
        public int VoterID { get; set; }
        [ForeignKey("VoterID")]
        public virtual Voter Voter { get; set; }
        public byte[] LeftOne { get; set; }
        public byte[] LeftTwo { get; set; }
        public byte[] LeftThree { get; set; }
        public byte[] LeftFour { get; set; }
        public byte[] LeftFive { get; set; }
        public byte[] RightOne { get; set; }
        public byte[] RightTwo { get; set; }
        public byte[] RightThree { get; set; }
        public byte[] RightFour { get; set; }
        public byte[] RightFive { get; set; }
    }
}