using System.Linq;
using LittleFirmManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LittleFirmManagement.Controllers
{
    public class KeyIndicatorsController : Controller
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

        public KeyIndicatorsController(FirmContext context)
        {
            _context = context;
		}

		public IActionResult Index(int selectedGranularity = 3, int selectedDetails = 0)
		{
            var cashflowIn = _context.FInvoices
                .Where(i => i.InReceiptDate.HasValue && i.FInterventions.Count != 0)
                .Select(i => new {
                    ct = new { id = -10, label = "overall sales" },
                    c = new { id = i.FInterventions.First().IFkCategory.CaId, label = i.FInterventions.First().IFkCategory.CaName },
                    date = i.InReceiptDate.Value,
                    quantity = (decimal)i.InAmount
                })
                .ToList();

            var mileageExpenses = _context.FInterventions
                .Select(i => new {
                    ct = new { id = -9, label = "overall mileage expenses" },
                    c = new { id = i.IFkCategory.CaId, label = i.IFkCategory.CaName },
                    date = i.IDate,
                    quantity = ( i.IFkClient.CDistance ?? 0m ) * 0.1m * 2 * i.INbRoundTrip
                })
                .ToList();

            var hoursBilled = _context.FInterventions
                .Where(i => i.IFkInvoice != null)
                .Select(i => new {
                    ct = new { id = -8, label = "overall hours billed" },
                    c = new { id = i.IFkCategory.CaId, label = i.IFkCategory.CaName },
                    date = i.IDate,
                    quantity = (decimal)(i.IFkInvoice.InAmount / (i.IFkCategory.CaName == "assistance informatique" ? 60 : 40))
                })
                .ToList();

            var kilometersTravelled = _context.FInterventions
                .Select(i => new {
                    ct = new { id = -7, label = "overall kilometers travelled" },
                    c = new { id = i.IFkCategory.CaId, label = i.IFkCategory.CaName },
                    date = i.IDate,
                    quantity = (i.IFkClient.CDistance ?? 0m) * 2m * i.INbRoundTrip
                })
                .ToList();

            var numberOfInvoices = _context.FInterventions
                .Where(i => i.IFkInvoice != null)
                .Select(i => new {
                    ct = new { id = -6, label = "overall number of invoices" },
                    c = new { id = i.IFkCategory.CaId, label = i.IFkCategory.CaName },
                    date = i.IFkInvoice.InInvoiceDate,
                    quantity = 1m
                })
                .ToList();

            var numberOfSeparateInvoicedCustomers = _context.FInterventions
                .Where(i => i.IFkInvoice != null)
                .Select(i => new {
                    ct = new { id = -5, label = "overall of separate invoiced customers" },
                    c = new { id = i.IFkCategory.CaId, label = i.IFkCategory.CaName },
                    date = i.IFkInvoice.InInvoiceDate,
                    quantity = (decimal)i.IFkClientId
                })
                .ToList();

            var data = cashflowIn
                .Concat(mileageExpenses)
                .Concat(hoursBilled)
                .Concat(kilometersTravelled)
                .Concat(numberOfInvoices)
                .Concat(numberOfSeparateInvoicedCustomers)
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

                    if (rows[r].ct.id==-5)
                        totalAmount = data
                            .Where(i => i.ct.id == rows[r].ct.id && (selectedDetails == 0 || i.c.id == rows[r].c.id) && startDate <= i.date && i.date < endDate)
                            .Select(i => i.quantity)
                            .Distinct()
                            .Count();
                    else
                        totalAmount = data
                            .Where(i => i.ct.id == rows[r].ct.id && (selectedDetails == 0 || i.c.id == rows[r].c.id) && startDate <= i.date && i.date < endDate)
                            .Sum(i => i.quantity);

                    array[r, k] = totalAmount;
                }
            }

            KeyIndicatorsIndexViewModel model = new()
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
