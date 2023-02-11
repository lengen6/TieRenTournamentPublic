using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TieRenTournament.Data;
using TieRenTournament.Models;

namespace TieRenTournament.Pages.Competitors
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly TieRenTournament.Data.ApplicationDbContext _context;

        public DeleteModel(TieRenTournament.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Competitor Competitor { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Competitor == null)
            {
                return NotFound();
            }

            var competitor = await _context.Competitor.FirstOrDefaultAsync(m => m.CompetitorId == id);

            if (competitor == null)
            {
                return NotFound();
            }
            else 
            {
                Competitor = competitor;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Competitor == null)
            {
                return NotFound();
            }
            var competitor = await _context.Competitor.FindAsync(id);

            if (competitor != null)
            {

                if (competitor.Wins != 0 || competitor.Losses != 0 || competitor.Byes != 0)
                {
                    return Redirect("/MatchHistoryError");
                }

                Competitor = competitor;
                _context.Competitor.Remove(Competitor);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
