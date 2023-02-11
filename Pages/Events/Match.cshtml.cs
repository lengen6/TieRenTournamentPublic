using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using TieRenTournament.Models;

namespace TieRenTournament.Pages.Events
{
    public class MatchModel : PageModel
    {
        private readonly TieRenTournament.Data.ApplicationDbContext _context;

        public MatchModel(TieRenTournament.Data.ApplicationDbContext context)
        {
            _context = context;
        }
        [FromQuery(Name = "elimination")]
        public int Elimination { get; set; }
        [FromQuery(Name = "match")]
        public int Match { get; set; }
        [FromQuery(Name = "round")]
        public int Round { get; set; } = 1;
        public Competitor? RedComp { get; set; }
        public Competitor? BlueComp { get; set; }
        [BindProperty]
        public bool RedWins { get; set; }
        [BindProperty]
        public int RedScore { get; set; }
        [BindProperty]
        public int BlueScore { get; set; }
        [BindProperty]
        public string VictoryMethod { get; set; }
        [BindProperty]
        public int StartMinutes { get; set; }
        [BindProperty]
        public int StartSeconds { get; set; }
        [BindProperty]
        public int EndMinutes { get; set; }
        [BindProperty]
        public int EndSeconds { get; set; }


        public void OnGet()
        {
            if (_context.Competitor != null)
            {
                RedComp = _context.Competitor.Where(r => r.IsRedComp == true).FirstOrDefault();
                BlueComp = _context.Competitor.Where(b => b.IsBlueComp == true).FirstOrDefault();

                if (RedComp != null && BlueComp != null)
                {
                    RedComp.PreviousParticipant = true;
                    RedComp.LastMatch = true;
                    BlueComp.PreviousParticipant = true;
                    BlueComp.LastMatch = true;

                    _context.Attach(RedComp);
                    _context.Attach(BlueComp);
                    _context.SaveChangesAsync();
                }
            }
        }

        public IActionResult OnPost(int elimination, int match, int round)
        {
            if (_context.Competitor != null)
            {
                RedComp = _context.Competitor.Where(r => r.IsRedComp == true).FirstOrDefault();
                BlueComp = _context.Competitor.Where(b => b.IsBlueComp == true).FirstOrDefault();
                SetWinner(RedComp, BlueComp, elimination, match, round);
                CreateMatch(RedComp, BlueComp, elimination, match, round);
                _context.SaveChanges();
            }

            return RedirectToPage("./Index", new {elimination = elimination, match = match, round = round});
        }

        //Class Methods
        public void CreateMatch(Competitor redComp, Competitor blueComp, int elimination, int match, int round)
        {
            string appendedZeroStart, appendedZeroEnd, startMinutesAppendZero, endMinutesAppendZero, lessThanTenAppendZero;
            AppendZeros(out appendedZeroStart, out appendedZeroEnd, out startMinutesAppendZero, out endMinutesAppendZero, out lessThanTenAppendZero);

            string startTime = startMinutesAppendZero + StartMinutes.ToString() + ":" + lessThanTenAppendZero + StartSeconds.ToString() + appendedZeroStart;
            string endTime = endMinutesAppendZero + EndMinutes.ToString() + ":" + EndSeconds.ToString() + appendedZeroEnd;
            string duration = timeGap(startTime, endTime);

            Match currentMatch = new Match();
            currentMatch.CompeitorRed = redComp;
            currentMatch.CompeitorBlue = blueComp;
            currentMatch.MatchNumber = match;
            currentMatch.RoundNumber = round;
            currentMatch.Bracket = redComp.Bracket;
            currentMatch.VictoryMethod = VictoryMethod;
            currentMatch.CompetitorRedScore = RedScore;
            currentMatch.CompetitorBlueScore = BlueScore;
            currentMatch.StartingLength = startTime;
            currentMatch.MatchEnd = endTime;
            currentMatch.Duration = duration;

            if (RedWins)
            {
                currentMatch.Winner = redComp;
            }
            else
            {
                currentMatch.Winner = blueComp;
            }

            _context.Match.Add(currentMatch);
        }

        private void AppendZeros(out string appendedZeroStart, out string appendedZeroEnd, out string startMinutesAppendZero, out string endMinutesAppendZero, out string lessThanTenAppendZero)
        {
            appendedZeroStart = "";
            appendedZeroEnd = "";
            startMinutesAppendZero = "";
            endMinutesAppendZero = "";
            lessThanTenAppendZero = "";
            if (StartSeconds == 0)
            {
                appendedZeroStart = "0";
            }

            if (EndSeconds == 0)
            {
                appendedZeroEnd = "0";
            }

            if (StartMinutes < 10)
            {
                startMinutesAppendZero = "0";
            }

            if (EndMinutes < 10)
            {
                endMinutesAppendZero = "0";
            }
            if (StartSeconds < 10 && StartSeconds > 0)
            {
                lessThanTenAppendZero = "0";
            }
        }

        public void SetWinner(Competitor redComp, Competitor blueComp, int elimination, int match, int round)
        {
            if (RedComp != null && BlueComp != null)
            {
                if (RedWins)
                {
                    RedComp.Wins++;
                    BlueComp.Losses++;

                    RedComp.Bracket = "Winner";

                    if (BlueComp.Losses >= elimination)
                    {
                        BlueComp.Bracket = "Eliminated";
                    }
                    else
                    {
                        BlueComp.Bracket = "Loser";
                    }
                }
                else if (!RedWins)
                {
                    BlueComp.Wins++;
                    RedComp.Losses++;

                    BlueComp.Bracket = "Winner";

                    if (RedComp.Losses >= elimination)
                    {
                        RedComp.Bracket = "Eliminated";
                    }
                    else
                    {
                        RedComp.Bracket = "Loser";
                    }
                }
                RedComp.IsRedComp = false;
                BlueComp.IsBlueComp = false;
                _context.Competitor.Attach(RedComp);
                _context.Competitor.Attach(BlueComp);
            }
        }

        static int getTimeInSeconds(String str)
        {

            String[] curr_time = str.Split(':');
            int t = Int32.Parse(curr_time[0]) * 60
                    + Int32.Parse(curr_time[1]);

            return t;
        }

        static String convertSecToTime(int t)
        {
            int min = (t % 3600) / 60;
            String mm = min < 10 ? "0" + min.ToString()
                    : min.ToString();
            int sec = ((t % 3600) % 60);
            String ss = sec < 10 ? "0" + sec.ToString()
                    : sec.ToString();
            String ans = mm + ":" + ss;
            return ans;
        }

        // Function to find the time gap
        static String timeGap(String st, String et)
        {

            int t1 = getTimeInSeconds(st);
            int t2 = getTimeInSeconds(et);

            int time_diff = (t1 - t2 < 0) ? t2 - t1 : t1 - t2;

            return convertSecToTime(time_diff);
        }
    }
}
