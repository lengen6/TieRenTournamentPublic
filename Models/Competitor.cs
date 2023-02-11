using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace TieRenTournament.Models
{
    public class Competitor
    {
        [Key]
        public int CompetitorId { get; set; }
        [Required]
        [RegularExpression(@"[a-zA-Z/'/-]+")]
        [StringLength(25)]
        [Display(Name ="First Name")]
        public string FirstName { get; set; }
        [Required]
        [RegularExpression(@"[a-zA-Z/'/-]+")]
        [StringLength(25)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public int Wins { get; set; } = 0;
        public int Losses { get; set; } = 0;
        public int Byes { get; set; } = 0;
        public int Place { get; set; } = 0;
        public bool IsRedComp { get; set; } = false;
        public bool IsBlueComp { get; set; } = false;
        public string Bracket { get; set; } = "Winner";
       
        public bool PreviousParticipant { get; set; } = false;
        public bool LastMatch { get; set; } = false;
        public IdentityUser? CreatedBy { get; set; }
    }
}
