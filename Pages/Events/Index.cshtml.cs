using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TieRenTournament.Models;
using TieRenTournament.Utils;
using Newtonsoft.Json;
using TieRenTournament.Data;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Identity;

namespace TieRenTournament.Pages.Events
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly TieRenTournament.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(TieRenTournament.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        List<Competitor> Competitors { get; set; }
        public List<Competitor> Winners { get; set; }

        public List<Competitor> Losers { get; set; }
        public List<Competitor> Eliminated { get; set; }
        public List<Competitor> Byes { get; set; }
        public List<Competitor> PreviousParticipants { get; set; }
        [FromQuery(Name = "elimination")]
        public int Elimination { get; set; }
        [FromQuery(Name = "match")]
        public int Match { get; set; }
        [FromQuery(Name = "round")]
        public int Round { get; set; }

        public Competitor compRed;
        public Competitor compBlue;

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (_context.Competitor != null)
            {
                Competitors = await _context.Competitor.Where(c => c.CreatedBy == user).ToListAsync();
                Winners = await _context.Competitor.Where(w => w.Bracket == "Winner" && w.CreatedBy == user).ToListAsync();
                Losers = await _context.Competitor.Where(l => l.Bracket == "Loser" && l.CreatedBy == user).ToListAsync();
                Eliminated = await _context.Competitor.Where(e => e.Bracket == "Eliminated" && e.CreatedBy == user ).ToListAsync();
                Byes = await _context.Competitor.Where(b => b.Bracket == "Bye" && b.CreatedBy == user ).ToListAsync();
            }

            if(Competitors.Count < 2)
            {
                return Redirect("/CompetitorCountError");
            }

            if (Eliminated.Count == Competitors.Count - 1)
            {
                Winners[0].Place = 1;
                List<Competitor> orderedByWins = Eliminated.OrderByDescending(e => e.Wins).ToList();
                for (int i = 0; i < orderedByWins.Count; i++)
                {
                    orderedByWins[i].Place = (i + 2);
                }
                AlignLocalStateToDB(Winners, Losers, Eliminated, Byes);
                return RedirectToPage("./Results");
            }

            if (Winners.Count == 1 && Losers.Count == 1)
            {
                IsFirstRound();
                compRed = Winners[0];
                compBlue = Losers[0];
                compRed.IsRedComp = true;
                compBlue.IsBlueComp = true;
                Match++;
                AlignLocalStateToDB(Winners, Losers, Eliminated, Byes);
                return RedirectToPage("./Match", new {elimination = Elimination, match = Match, round = Round});
            }

            if (Winners != null)
            {
                IsFirstRound();
                DetermineByes(Winners);
                if (!IsBracketFinished(Winners))
                {
                    if (ArrangeMatch(Winners))
                    {
                        Match++;
                        AlignLocalStateToDB(Winners, Losers, Eliminated,Byes);
                        return RedirectToPage("./Match", new { elimination = Elimination, match = Match, round = Round });
                    }
                    
                }
            }

            if (Losers != null)
            {
                IsFirstRound();
                DetermineByes(Losers);
                if (!IsBracketFinished(Losers))
                {
                    if (ArrangeMatch(Losers))
                    {
                        Match++;
                        AlignLocalStateToDB(Winners, Losers, Eliminated, Byes);
                        return RedirectToPage("./Match", new { elimination = Elimination, match = Match, round = Round });
                    }

                }
            }

            AlignLocalStateToDB(Winners, Losers, Eliminated, Byes);
            if(_context.Competitor != null)
            {
                Byes = await _context.Competitor.Where(b => b.Bracket == "Bye" && b.CreatedBy == user).ToListAsync();
            }

            if (Byes.Count > 1)
            {
                IsFirstRound();
                foreach (Competitor comp in Byes)
                {
                    comp.Byes--;
                    comp.PreviousParticipant = false;
                }

                if (ArrangeMatch(Byes))
                {
                    Match++;
                    AlignLocalStateToDB(Winners, Losers, Eliminated, Byes);
                    return RedirectToPage("./Match", new { elimination = Elimination, match = Match, round = Round });
                }
            }          

            bool RoundOver = IsRoundOver();

            if(RoundOver)
            {
                Match = 0;
                Round++;
                ResetParticipant();
                AlignLocalStateToDB(Winners, Losers, Eliminated, Byes);
                return RedirectToPage("./Index", new { elimination = Elimination, match = Match, round = Round });
            }
            
            return Redirect("~/");
        }

        //Class Methods go here
        public void IsFirstRound()
        {
            if(Round == 0)
            {
                Round++;
            }
        }
        public bool IsRoundOver()
        {
            bool isRoundOver = true;

            foreach (Competitor comp in Winners)
            {
                
                if (comp.PreviousParticipant == false)
                {
                    isRoundOver = false;
                }
            }

            foreach (Competitor comp in Losers)
            {

                if (comp.PreviousParticipant == false)
                {
                    isRoundOver = false;
                }
            }

            return isRoundOver;
        }
        public void ResetParticipant()
        {
            foreach (Competitor comp in Winners)
            {
                comp.PreviousParticipant = false;
        
            }

            foreach (Competitor comp in Losers)
            {
                comp.PreviousParticipant = false;
            }
        }

        public bool IsBracketFinished(List<Competitor> bracket)
        {
            bool isFinished = true;

            foreach(Competitor comp in bracket)
            {
                if(comp.PreviousParticipant == false)
                {
                    isFinished = false;
                }
            }

            return isFinished;
        }
        public void DetermineByes(List<Competitor> bracket)
        {
           bool wasPreviouslyRun = false;
            foreach (Competitor comp in bracket)
            {
                if (comp.PreviousParticipant)
                {
                    wasPreviouslyRun = true;
                }
            }

            if (!wasPreviouslyRun)
            {
               if(bracket.Count % 2 != 0)
                {
                    Random bracketPicker = new Random();
                    int currentPick = bracketPicker.Next(bracket.Count);
                    bracket[currentPick].Byes++;
                    bracket[currentPick].Bracket = "Bye";
                    bracket[currentPick].PreviousParticipant = true;
                }

            }

        }

         public Competitor PickPlayer(List<Competitor> argBracket)
        {
            Random bracketPicker = new Random();
            int compPick = bracketPicker.Next(argBracket.Count);
            Competitor comp = argBracket[compPick];
            argBracket.RemoveAt(compPick);
            return comp;
        }

        public bool ArrangeMatch(List<Competitor> argBracket)
        {
            List<Competitor> viableCompetitors = new List<Competitor>();
            foreach(Competitor comp in argBracket)
            {
                if(comp.PreviousParticipant == false)
                {
                    viableCompetitors.Add(comp);
                }
            }

            if(viableCompetitors.Count > 1)
            {
                compRed = PickPlayer(viableCompetitors);
                compBlue = PickPlayer(viableCompetitors);
                compRed.IsRedComp = true;
                compBlue.IsBlueComp = true;
                return true;
            } else if (viableCompetitors[0] != null)
            {
                viableCompetitors[0].Byes++;
                viableCompetitors[0].Bracket = "Bye";
                viableCompetitors[0].PreviousParticipant = true;
                return false;
            }

            return true;
        }

        public void AlignLocalStateToDB(List<Competitor> winnersToSave, List<Competitor> losersToSave, List<Competitor> eliminatedToSave, List<Competitor> byesToSave)
        {

            if (winnersToSave.Count != null)
            {
                foreach (var winner in winnersToSave)
                {
                    winner.Bracket = "Winner";
                    _context.Competitor.Attach(winner);
                }
            }

            if (losersToSave.Count != null)
            {
                foreach (var loser in losersToSave)
                {
                    loser.Bracket = "Loser";
                    _context.Competitor.Attach(loser);
                }
            }

            if (eliminatedToSave != null)
            {
                foreach (var eliminated in eliminatedToSave)
                {
                    eliminated.Bracket = "Eliminated";
                    _context.Competitor.Attach(eliminated);
                }
            }

            if (byesToSave != null)
            {
                foreach (var bye in byesToSave)
                {
                    bye.Bracket = "Bye";
                    _context.Competitor.Attach(bye);
                }
            }

            _context.SaveChanges();
        }

    }
}
