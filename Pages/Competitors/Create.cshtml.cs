using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TieRenTournament.Data;
using TieRenTournament.Models;

namespace TieRenTournament.Pages.Competitors
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly TieRenTournament.Data.ApplicationDbContext _context;

        private readonly UserManager<IdentityUser> _userManager;

        public CreateModel(TieRenTournament.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Competitor Competitor { get; set; }


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid)
            {
                return Page();
            }

            Competitor.CreatedBy = await _userManager.GetUserAsync(HttpContext.User);
            _context.Competitor.Add(Competitor);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
