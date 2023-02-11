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
    public class DetailsModel : PageModel
    {
        private readonly TieRenTournament.Data.ApplicationDbContext _context;

        public DetailsModel(TieRenTournament.Data.ApplicationDbContext context)
        {
            _context = context;
        }

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
    }
}
