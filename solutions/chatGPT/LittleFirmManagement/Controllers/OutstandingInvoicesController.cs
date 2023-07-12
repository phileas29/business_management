using System;
using System.Linq;
using LittleFirmManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LittleFirmManagement.Controllers
{
    public class OutstandingInvoicesController : Controller
    {
        private readonly FirmContext _context;

        public OutstandingInvoicesController(FirmContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var outstandingInvoices = _context.FInterventions.Where(i=> i.IFkInvoice != null && i.IFkInvoice.InCreditDate == null).OrderByDescending(i=>i.IFkInvoiceId).Select(i=> new { no = i.IFkInvoice.InInvoiceId, i.IFkClient.CName, i.IDate, i.IFkInvoice.InInvoiceDate, i.IFkInvoice.InReceiptDate, i.IFkInvoice.InCreditDate, i.IDescription, i.IFkInvoice.InAmount }).ToList();

            var paymentsWithNull = _context.FCategories.Where(c => c.CaFkCategoryType.CtName == "paiement").ToList();
            paymentsWithNull.Insert(0, new FCategory { CaId = -1, CaName = "Select a payment" });

            List<object> subjectsWithNull = new List<object>
            {
                new FCategory { CaId = -1, CaName = "Select a payment" },
                new FCategory { CaId = 0, CaName = "reception" },
                new FCategory { CaId = 1, CaName = "credit" }
            };

            ViewData["PaymentId"] = new SelectList(paymentsWithNull, "CaId", "CaName");
            ViewData["SubjectId"] = new SelectList(subjectsWithNull, "CaId", "CaName");


            ViewData["Data"] = outstandingInvoices;


            return View();
        }

        // POST: FPurchases/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(OutstandingInvoicesViewModel outstandingInvoicesViewModel)
        {
            if (ModelState.IsValid)
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var outstandingInvoices = _context.FInterventions.Where(i => i.IFkInvoice != null && i.IFkInvoice.InCreditDate == null).OrderByDescending(i => i.IFkInvoiceId).Select(i => new { no = i.IFkInvoice.InInvoiceId, i.IFkClient.CName, i.IDate, i.IFkInvoice.InInvoiceDate, i.IFkInvoice.InReceiptDate, i.IFkInvoice.InCreditDate, i.IDescription }).ToList();

            var paymentsWithNull = _context.FCategories.Where(c => c.CaFkCategoryType.CtName == "paiement").ToList();
            paymentsWithNull.Insert(0, new FCategory { CaId = -1, CaName = "Select a payment" });

            List<object> subjectsWithNull = new List<object>
            {
                new FCategory { CaId = -1, CaName = "Select a payment" },
                new FCategory { CaId = 0, CaName = "reception" },
                new FCategory { CaId = 1, CaName = "credit" }
            };

            ViewData["PaymentId"] = new SelectList(paymentsWithNull, "CaId", "CaName");
            ViewData["SubjectId"] = new SelectList(subjectsWithNull, "CaId", "CaName");


            ViewData["Data"] = outstandingInvoices;
            return View();
        }
    }
}
