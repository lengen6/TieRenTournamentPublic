using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Graph;
using Microsoft.EntityFrameworkCore;
using TieRenTournament.Models;

namespace TieRenTournament.Pages.Events
{
    public class HistoryModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string CompetitorFirstNameSearch { get; set; }
        [BindProperty(SupportsGet = true)]
        public string CompetitorLastNameSearch { get; set; }
        [BindProperty(SupportsGet = true)]
        public string WinnerFirstNameSearch { get; set; }
        [BindProperty(SupportsGet = true)]
        public string WinnerLastNameSearch { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PointsSearch { get; set; }
        [BindProperty(SupportsGet = true)]
        public string VictoryMethodSearch { get; set; }

        private readonly TieRenTournament.Data.ApplicationDbContext _context;

        public HistoryModel(TieRenTournament.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Match> Matches { get; set; }
        public void OnGet()
        {
            Matches = _context.Match.Include(r => r.CompeitorRed).Include(b => b.CompeitorBlue).ToList();

            if(CompetitorFirstNameSearch != null)
            {
                Matches = Matches.Where(m => m.CompeitorRed.FirstName.Contains(CompetitorFirstNameSearch)
            || m.CompeitorBlue.FirstName.Contains(CompetitorFirstNameSearch)).ToList();
            }

            if (CompetitorLastNameSearch != null)
            {
                Matches = Matches.Where(m => m.CompeitorRed.LastName.Contains(CompetitorLastNameSearch)
            || m.CompeitorBlue.LastName.Contains(CompetitorLastNameSearch)).ToList();
            }

            if (WinnerFirstNameSearch != null)
            {
                Matches = Matches.Where(m => m.Winner.FirstName.Contains(WinnerFirstNameSearch)).ToList();
            }

            if (WinnerLastNameSearch != null)
            {
                Matches = Matches.Where(m => m.Winner.LastName.Contains(WinnerLastNameSearch)).ToList();
            }
            if (PointsSearch != null)
            {
                Matches = Matches.Where(m => m.CompetitorRedScore >= PointsSearch
            || m.CompetitorBlueScore >= PointsSearch).ToList();
            }

            if(VictoryMethodSearch != null)
            {
                Matches = Matches.Where(m => m.VictoryMethod.Contains(VictoryMethodSearch)).ToList();
            }
        }
    }
}

//Where(m => m.CompeitorRed.FirstName.Contains(CompetitorFirstNameSearch)
//            || m.CompeitorBlue.FirstName.Contains(CompetitorFirstNameSearch) || m.CompeitorRed.LastName.Contains(CompetitorLastNameSearch) ||
//            m.CompeitorBlue.LastName.Contains(CompetitorLastNameSearch))