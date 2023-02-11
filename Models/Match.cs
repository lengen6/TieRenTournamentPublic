using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TieRenTournament.Models
{
    public class Match
    {
        [Key]
        public int MatchId { get; set; }
        [Display(Name = "Red Score")]
        public int CompetitorRedScore { get; set; } = 0;
        [Display(Name = "Blue Score")]
        public int CompetitorBlueScore { get; set; } = 0;
        [Display(Name = "Match End")]
        public string MatchEnd { get; set; } = "0";
        [Display(Name = "Starting Length")]
        public string StartingLength { get; set; } = "0";
        public string Duration { get; set; } = "0";
        [Display(Name = "Round #")]
        public int RoundNumber { get; set; } = 0;
        [Display(Name = "Match #")]
        public int MatchNumber { get; set; } = 0;
        [Display(Name = "Victory By")]
        public string VictoryMethod { get; set; } = "Default";
        public string Bracket { get; set; }

        //Foreign Keys
        [ForeignKey("Competitor")]
        public int? CompetitorRedId { get; set; }
        [Display(Name ="Red Competitor")]
        public Competitor? CompeitorRed { get; set; }
        [ForeignKey("Competitor")]
        public int? CompetitorBlueId { get; set; }
        [Display(Name = "Blue Competitor")]
        public Competitor? CompeitorBlue { get; set; }
        [ForeignKey("Competitor")]
        public int? WinnerId { get; set; }
        public Competitor? Winner { get; set; }

    }
}
