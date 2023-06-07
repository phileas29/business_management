using LittleFirmManagement.Models;
using Microsoft.AspNetCore.Mvc;
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

        public TreasuryController(FirmContext context)
        {
            _context = context;
        }

        public IActionResult Index(int selectedGranularity = 3)
        {
            ViewData["GranularityMapping"] = granularityMapping;
            ViewData["SelectedGranularity"] = selectedGranularity;

            var cashflowIn = _context.FInvoices.Where(i => i.InCreditDate != null).Select(i => new { label = "overall sales", i.InCreditDate , i.InAmount }).ToList();
            var cashflowOut = _context.FPurchases.Where(i => i.PDebitDate != null).Select(i => new { i.PFkSupplier.CaName, i.PDebitDate, i.PAmount }).ToList();
            List<DateTime> beginDates = new List<DateTime>()
            {
                cashflowIn.Select(c => c.InCreditDate)?.Min().Value ?? DateTime.MaxValue,
                cashflowOut.Select(c => c.PDebitDate)?.Min().Value.ToDateTime(new TimeOnly(11, 0)) ?? DateTime.MaxValue
            };
            ViewData["Begin"] = beginDates.Min();
            List<DateTime> endDates = new List<DateTime>() {
                cashflowIn.Select(c => c.InCreditDate).Max().Value,
                cashflowOut.Select(c => c.PDebitDate).Max().Value.ToDateTime(new TimeOnly(11, 0))
            };
            List<DateTime> limitDatesRaw = new List<DateTime>() {
                beginDates.Min(),
                endDates.Max()
            };

            List<DateTime> limitDatesRounded = new List<DateTime>();
            for(int i = 0; i < limitDatesRaw.Count; i++)
            {
                DateTime roundedDate = new DateTime(limitDatesRaw[i].Year, limitDatesRaw[i].Month, 1, 0, 0, 0);
                if (i == 1 && 1 < limitDatesRaw[i].Day)
                    roundedDate = roundedDate.AddMonths(1);
                limitDatesRounded.Add(roundedDate);
            }
            int n = 1;
            //int n = (int)(Math.Pow(2, selectedGranularity+1) - 1);

            List<DateTime> limitDates = new List<DateTime>();
            //alignment
            switch (selectedGranularity)
            {
                case 1:
                    n = 3;
                    limitDates.Add(limitDatesRounded[0].AddMonths(-((limitDatesRounded[0].Month - 1) % 3)));
                    limitDates.Add(limitDatesRounded[1].AddMonths(3-((limitDatesRounded[1].Month - 1) % 3 + 3)));
                    break;
                case 2:
                    n = 6;
                    limitDates.Add(limitDatesRounded[0].AddMonths(-((limitDatesRounded[0].Month - 1) % 6)));
                    limitDates.Add(limitDatesRounded[1].AddMonths(-((limitDatesRounded[1].Month - 1) % 6 + 6)));
                    break;
                case 3:
                    n = 12;
                    limitDates.Add(limitDatesRounded[0].AddMonths(-(limitDatesRounded[0].Month - 1)));
                    limitDates.Add(limitDatesRounded[1].AddYears(1 < limitDatesRounded[1].Month ? 1 : 0));
                    break;
            }

            int N = (int)Math.Ceiling( ( ((limitDates[1].Year - limitDates[0].Year) * 12) + limitDates[1].Month - limitDates[0].Month ) / (decimal)n);

            List<string> labels = cashflowIn.Select(i => i.label).Union(cashflowOut.Select(p => p.CaName)).ToList();
            int numRows = labels.Count;
            decimal[,] array = new decimal[numRows, N];

            for (int row = 0; row < numRows; row++)
            {
                string label = labels[row];

                for (int i = 0; i < N; i++)
                {
                    DateTime startDate = limitDates[0].AddMonths(n * i);
                    DateTime endDate = startDate.AddMonths(n);
                    decimal totalAmount = 0;

                    if (label== "overall sales")
                    {
                        totalAmount = cashflowIn
                            .Where(i => i.label == label && i.InCreditDate >= startDate && i.InCreditDate < endDate)
                            .Sum(i => i.InAmount);
                    } else
                    {
                        totalAmount = cashflowOut
                            .Where(p => p.CaName == label && p.PDebitDate?.ToDateTime(new TimeOnly(11, 0)) >= startDate && p.PDebitDate?.ToDateTime(new TimeOnly(11, 0)) < endDate)
                            .Sum(p => p.PAmount);
                    }

                    array[row, i] = totalAmount;
                }
            }

            ViewData["Data"] = array;
            ViewData["Labels"] = labels;
            //ViewData["Dates"] = labels;

            return View();
        }
    }
}
