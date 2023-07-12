using LittleFirmManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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
            ViewData["GranularityMapping"] = granularityMapping;
            ViewData["DetailsMapping"] = detailsMapping;
            ViewData["SelectedGranularity"] = selectedGranularity;
            ViewData["SelectedDetails"] = selectedDetails;

            var cashflowIn = _context.FInvoices.Where(i => i.InCreditDate != null).Select(i => new { ct = new { id = -1, label = "overall sales" }, c = new { id = -1, label = "" }, i.InCreditDate, i.InAmount }).ToList();
            var cashflowOut = _context.FPurchases.Where(i => i.PDebitDate != null).Select(i => new { ct = new { i.PFkCategory.CaId, i.PFkCategory.CaName }, c = new { i.PFkSupplier.CaId, i.PFkSupplier.CaName }, i.PDebitDate, i.PAmount }).ToList();

            List<DateTime> beginDates = new List<DateTime>()
            {
                cashflowIn.Select(c => c.InCreditDate)?.Min().Value ?? DateTime.MaxValue,
                cashflowOut.Select(c => c.PDebitDate)?.Min().Value ?? DateTime.MaxValue
            };
            List<DateTime> endDates = new List<DateTime>() {
                cashflowIn.Select(c => c.InCreditDate)?.Max().Value ?? DateTime.MinValue,
                cashflowOut.Select(c => c.PDebitDate)?.Max().Value ?? DateTime.MinValue
            };

            List<DateTime> limitDatesRaw = new List<DateTime>() {
                beginDates.Min(),
                endDates.Max()
            };

            List<DateTime> limitDatesRounded = new();
            foreach(DateTime limitDate in limitDatesRaw)
                limitDatesRounded.Add(new DateTime(limitDate.Year, limitDate.Month, 1, 0, 0, 0));

            int n = periodMapping.GetValueOrDefault(selectedGranularity,12);
            List<DateTime> limitDates = new List<DateTime>
            {
                limitDatesRounded[0].AddMonths(-((limitDatesRounded[0].Month - 1) % n)),
                limitDatesRounded[1]
            };

            int N = (int)Math.Ceiling(((limitDates[1].Year - limitDates[0].Year) * 12 + limitDates[1].Month - limitDates[0].Month + 1) / (decimal)n);

            int numRows =
                cashflowIn
                .Select(p => new { ct = p.ct.label, c = selectedDetails == 0 ? "" : p.c.label })
                .Distinct()
                .Union(cashflowOut
                    .Select(p => new { ct = p.ct.CaName, c = selectedDetails == 0 ? "" : p.c.CaName })
                    .Distinct())
                .ToList()
                .Count();

            decimal[,] array = new decimal[numRows, N];

            var distinctCouples = cashflowIn
                .OrderBy(p => p.ct.label)
                .Select(p => new { ct = p.ct.id, c = selectedDetails == 0 ? -1 : p.c.id })
                .Distinct()
                .ToList();
            for (int r = 0; r < distinctCouples.Count; r++)
            {
                for (int k = 0; k < N; k++)
                {
                    DateTime startDate = limitDates[0].AddMonths(n * k);
                    DateTime endDate = startDate.AddMonths(n);
                    decimal totalAmount = cashflowIn
                        .Where(i => i.ct.id == distinctCouples[r].ct && i.c.id == distinctCouples[r].c && i.InCreditDate >= startDate && i.InCreditDate < endDate)
                        .Sum(i => i.InAmount);

                    array[r, k] = totalAmount;
                }
            }

            int offset = distinctCouples.Count;
            distinctCouples = cashflowOut
                .OrderBy(p => p.ct.CaName)
                .Select(p => new { ct = p.ct.CaId, c = selectedDetails == 0 ? -1 : p.c.CaId })
                .Distinct()
                .ToList();
            for (int r = 0; r < distinctCouples.Count; r++)
            {
                for (int k = 0; k < N; k++)
                {
                    DateTime startDate = limitDates[0].AddMonths(n * k);
                    DateTime endDate = startDate.AddMonths(n);

                    decimal totalAmount = cashflowOut
                            .Where(p => p.ct.CaId == distinctCouples[r].ct && ( selectedDetails == 0 || p.c.CaId == distinctCouples[r].c ) && p.PDebitDate >= startDate && p.PDebitDate < endDate)
                            .Sum(p => p.PAmount);

                    array[offset+r, k] = totalAmount;
                }
            }

            ViewData["Begin"] = limitDates[0];
            ViewData["N"] = n;
            ViewData["Data"] = array;
            ViewData["Labels"] =
                cashflowIn
                .Select(p => new { ct = p.ct.label, c = selectedDetails == 0 ? "" : p.c.label })
                .Distinct()
                .OrderBy(p => p.ct)
                .Union(cashflowOut
                    .Select(p => new { ct = p.ct.CaName, c = selectedDetails == 0 ? "" : p.c.CaName })
                    .Distinct()
                    .OrderBy(p => p.ct))
                .ToList();

            return View();
        }
    }
}
