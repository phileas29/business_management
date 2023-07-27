using LittleFirmManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LittleFirmManagement.Controllers
{
    public class TreasuryController : Controller
    {
        private readonly FirmContext _context;


        private Dictionary<int, string> granularityMapping = new Dictionary<int, string>
        {
            { 0, "Month" },
            { 1, "Quarter" },
            { 2, "Semester" },
            { 3, "Year" }
        };

        private Dictionary<int, string> detailsMapping = new Dictionary<int, string>
        {
            { 0, "General" },
            { 1, "Detailed" }
        };

        private Dictionary<int, int> periodMapping = new Dictionary<int, int>
        {
            { 0,  1 },
            { 1,  3 },
            { 2,  6 },
            { 3, 12 }
        };

        public TreasuryController(FirmContext context)
        {
            _context = context;
        }

        public IActionResult Index(int selectedGranularity = 3, int selectedDetails = 0)
        {
            var cashflowIn = _context.FInvoices
                .Where(i => i.InCreditDate != null)
                .Select(i => new {
                    ct = new { id = -1, label = "overall sales" },
                    c = new { id = i.FInterventions.First().IFkCategory.CaId, label = i.FInterventions.First().IFkCategory.CaName },
                    date = i.InCreditDate.Value,
                    quantity = (decimal)i.InAmount
                })
                .ToList();

            var cashflowOut = _context.FPurchases
                .Where(i => i.PDebitDate != null)
                .Select(i => new {
                    ct = new { id = i.PFkCategory.CaId, label = i.PFkCategory.CaName },
                    c = new { id = i.PFkSupplier.CaId, label = i.PFkSupplier.CaName },
                    date = i.PDebitDate.Value,
                    quantity = i.PAmount
                })
                .ToList();

            var data = cashflowIn
                .Concat(cashflowOut)
                .ToList();

            List<DateTime> limitDatesRaw = new List<DateTime>() {
                data.Select(c => c.date).Min(),
                data.Select(c => c.date).Max()
            };

            List<DateTime> limitDatesRounded = new();
            foreach (DateTime limitDate in limitDatesRaw)
                limitDatesRounded.Add(new DateTime(limitDate.Year, limitDate.Month, 1, 0, 0, 0));

            int n = periodMapping.GetValueOrDefault(selectedGranularity, 12);
            List<DateTime> limitDates = new List<DateTime>
            {
                limitDatesRounded[0].AddMonths(-((limitDatesRounded[0].Month - 1) % n)),
                limitDatesRounded[1]
            };

            var rows = data
                .Select(p => new { p.ct, c = selectedDetails == 0 ? new { id = -1, label = "" } : p.c })
                .OrderBy(p => p.ct.id)
                .ThenBy(p => p.c.id)
                .Distinct()
                .ToList();

            int N = (int)Math.Ceiling(((limitDates[1].Year - limitDates[0].Year) * 12 + limitDates[1].Month - limitDates[0].Month + 1) / (decimal)n);

            decimal[,] array = new decimal[rows.Count(), N];
            DateTime startDate;
            DateTime endDate;
            decimal totalAmount;
            for (int r = 0; r < rows.Count; r++)
            {
                for (int k = 0; k < N; k++)
                {
                    startDate = limitDates[0].AddMonths(n * k);
                    endDate = startDate.AddMonths(n);
                    totalAmount = data
                        .Where(i => i.ct.id == rows[r].ct.id && (selectedDetails == 0 || i.c.id == rows[r].c.id) && startDate <= i.date && i.date < endDate)
                        .Sum(i => i.quantity);

                    array[r, k] = totalAmount;
                }
            }

            TreasuryIndexViewModel model = new()
            {
                Data = array,
                N = n,
                Begin = limitDates[0],
                Labels = rows
                    .Select(p => new List<string> { p.ct.label, p.c.label })
                    .ToList(),
                SelectedGranularity = selectedGranularity,
                SelectedDetails = selectedDetails,
                DetailsMapping = detailsMapping,
                GranularityMapping = granularityMapping
            };

            return View(model);
        }
    }
}
