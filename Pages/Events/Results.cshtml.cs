using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TieRenTournament.Models;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace TieRenTournament.Pages.Events
{
    [Authorize]
    public class ResultsModel : PageModel
    {
        private readonly TieRenTournament.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ResultsModel(TieRenTournament.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Competitor> Results { get; set; }
        public List<Match> Matches { get; set; }


        public async Task OnGetAsync()
        {

            var user = await _userManager.GetUserAsync(HttpContext.User);
            List<Competitor> competitors = _context.Competitor.Where(c => c.CreatedBy == user).ToList();
            Results = competitors.OrderBy(c => c.Place).ToList();
            
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            Results = await _context.Competitor.ToListAsync();
            Results = Results.Where(c => c.CreatedBy == user).ToList();
            Matches = await _context.Match.ToListAsync();

            foreach(Match match in Matches)
            {
                _context.Match.Remove(match);
            }

            foreach (var competitor in Results)
            {
                competitor.Bracket = "Winner";
                competitor.Place = 0;
                competitor.PreviousParticipant = false;
                competitor.LastMatch = false;
                competitor.Byes = 0;
                competitor.Wins = 0;
                competitor.Losses = 0;
                competitor.IsRedComp = false;
                competitor.IsBlueComp = false;
                _context.Competitor.Attach(competitor);
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("/Competitors/Index");
        } 
    }
}
