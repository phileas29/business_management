using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LittleFirmManagement.Models;

namespace LittleFirmManagement.Controllers
{
    public class FInterventionsController : Controller
    {
        private readonly FirmContext _context;

        public FInterventionsController(FirmContext context)
        {
            _context = context;
        }

        private void PrepareViewData()
        {
            var pendingInvoices = _context.FInterventions
                .Where(i => i.IFkInvoice == null && DateTime.Now.AddDays(-180) < i.IDate)
                .OrderByDescending(i => i.IDate)
                .Select(i => new
                {
                    i.IId,
                    i.IFkClientId,
                    i.IFkClient.CName,
                    IDate = i.IDate.ToShortDateString(),
                    i.IDescription,
                    i.IFkCategory.CaName
                })
                .ToList();

            ViewData["Data"] = pendingInvoices;
        }

        public IActionResult Pending()
        {
            PrepareViewData();

            return View();
        }

        // GET: FInterventions
        public async Task<IActionResult> Index()
        {
            var firmContext = _context.FInterventions.Include(f => f.IFkCategory).Include(f => f.IFkClient).Include(f => f.IFkInvoice);
            return View(await firmContext.ToListAsync());
        }

        // GET: FInterventions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fIntervention = await _context.FInterventions
                .Include(f => f.IFkCategory)
                .Include(f => f.IFkClient)
                .Include(f => f.IFkInvoice)
                .FirstOrDefaultAsync(m => m.IId == id);
            if (fIntervention == null)
            {
                return NotFound();
            }

            return View(fIntervention);
        }

        private void PrepareViewData(FIntervention model, int id)
        {
            List<SelectListItem> activitiesWithNull = _context.FCategories
                .Where(c => c.CaFkCategoryType.CtName == "activité")
                .Select(c => new SelectListItem
                {
                    Text = c.CaName,
                    Value = c.CaId.ToString()
                })
                .ToList();
            activitiesWithNull.Insert(0,new SelectListItem { Text = "Select an activity", Value = "", Selected = true });
            ViewData["IFkCategoryId"] = new SelectList(activitiesWithNull, "Value", "Text", "");
            ViewData["FClient"] = _context.FClients.Include(c => c.CFkCity).FirstOrDefault(c => c.CId == id);
        }

        // GET: FInterventions/Create/clientId
        public IActionResult Create(int id)
        {
            FIntervention model = new();

            PrepareViewData(model,id);

            // Create a new instance of FIntervention and set default values
            model = new FIntervention
            {
                IDate = DateTime.UtcNow,  // Set the default date to the current date
                //IDescription = "Default description",  // Set a default description
                INbRoundTrip = 1,  // Set the default number of round trips
                //IFkCategoryId = 8  // Set the default category ID
            };
            return View(model);
        }

        // POST: FInterventions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IId,IFkClientId,IFkInvoiceId,IFkCategoryId,IDate,IDescription,INbRoundTrip")] FIntervention model, bool saveAndExit, int id)
        {
            ModelState.Remove("IFkClient");
            ModelState.Remove("IFkCategory");
            if (ModelState.IsValid)
            {
                model.IDate = DateTime.SpecifyKind(model.IDate, DateTimeKind.Utc);
                model.IFkClientId = id;
                _context.Add(model);
                await _context.SaveChangesAsync();
                if (saveAndExit)
                    return RedirectToAction(nameof(Index));
                else
                    return RedirectToAction("Create", "FInvoices", new { id });
            }
            PrepareViewData(model, id);
            return View(model);
        }

        // GET: FInterventions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fIntervention = await _context.FInterventions.FindAsync(id);
            if (fIntervention == null)
            {
                return NotFound();
            }
            ViewData["IFkCategoryId"] = new SelectList(_context.FCategories, "CaId", "CaId", fIntervention.IFkCategoryId);
            ViewData["IFkClientId"] = new SelectList(_context.FClients, "CId", "CId", fIntervention.IFkClientId);
            ViewData["IFkInvoiceId"] = new SelectList(_context.FInvoices, "InId", "InId", fIntervention.IFkInvoiceId);
            return View(fIntervention);
        }

        // POST: FInterventions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IId,IFkClientId,IFkInvoiceId,IFkCategoryId,IDate,IDescription,INbRoundTrip")] FIntervention fIntervention)
        {
            if (id != fIntervention.IId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fIntervention);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FInterventionExists(fIntervention.IId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IFkCategoryId"] = new SelectList(_context.FCategories, "CaId", "CaId", fIntervention.IFkCategoryId);
            ViewData["IFkClientId"] = new SelectList(_context.FClients, "CId", "CId", fIntervention.IFkClientId);
            ViewData["IFkInvoiceId"] = new SelectList(_context.FInvoices, "InId", "InId", fIntervention.IFkInvoiceId);
            return View(fIntervention);
        }

        // GET: FInterventions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fIntervention = await _context.FInterventions
                .Include(f => f.IFkCategory)
                .Include(f => f.IFkClient)
                .Include(f => f.IFkInvoice)
                .FirstOrDefaultAsync(m => m.IId == id);
            if (fIntervention == null)
            {
                return NotFound();
            }

            return View(fIntervention);
        }

        // POST: FInterventions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fIntervention = await _context.FInterventions.FindAsync(id);
            _context.FInterventions.Remove(fIntervention);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FInterventionExists(int id)
        {
            return _context.FInterventions.Any(e => e.IId == id);
        }
    }
}
