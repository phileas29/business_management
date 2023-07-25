using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LittleFirmManagement.Models;
using System.Runtime.CompilerServices;

namespace LittleFirmManagement.Controllers
{
    public class FInterventionsController : Controller
    {
        private readonly FirmContext _context;

        public FInterventionsController(FirmContext context)
        {
            _context = context;
        }

        private void PrepareViewData(ref List<FInterventionPendingViewModel> model)
        {
            model = _context.FInterventions
                .Where(i => i.IFkInvoice == null && DateTime.Now.AddDays(-180) < i.IDate)
                .OrderByDescending(i => i.IDate)
                .Select(i => new FInterventionPendingViewModel
                {
                    InterventionId = i.IId,
                    ClientName = i.IFkClient.CName,
                    ClientId = i.IFkClientId,
                    InterventionDate = i.IDate.ToShortDateString(),
                    Description = i.IDescription,
                    Category = i.IFkCategory.CaName
                })
                .ToList();
        }

        public IActionResult Pending()
        {
            List<FInterventionPendingViewModel> model = new();

            PrepareViewData(ref model);

            return View(model);
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

        private void PrepareViewData(FInterventionCreateViewModel model, int id)
        {
            model.ActivityId = _context.FCategoryTypes.Where(c => c.CtName == "activité").Select(c => c.CtId).First();
            model.ClientId = id;
            List <SelectListItem> activitiesWithNull = _context.FCategories
                .Where(c => c.CaFkCategoryType.CtId == model.ActivityId)
                .Select(c => new SelectListItem
                {
                    Text = c.CaName,
                    Value = c.CaId.ToString()
                })
                .ToList();
            activitiesWithNull.Insert(0,new SelectListItem { Text = "Select an activity", Value = "", Selected = true });
            model.Activities = new SelectList(activitiesWithNull, "Value", "Text", "");
            model.Client = _context.FClients.Include(c => c.CFkCity).FirstOrDefault(c => c.CId == id);

            // Create a new instance of FIntervention and set default values
            model.Intervention ??= new FIntervention
            {
                IDate = DateTime.UtcNow,  // Set the default date to the current date
                //IDescription = "Default description",  // Set a default description
                INbRoundTrip = 1,  // Set the default number of round trips
                //IFkCategoryId = 8  // Set the default category ID
            };
        }

        // GET: FInterventions/Create/clientId
        public IActionResult Create(int id)
        {
            FInterventionCreateViewModel model = new();

            PrepareViewData(model,id);
            return View(model);
        }

        // POST: FInterventions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(FInterventionCreateViewModel model, int id)
        {
            if (ModelState.IsValid)
            {
                model.Intervention.IDate = DateTime.SpecifyKind(model.Intervention.IDate, DateTimeKind.Utc);
                model.Intervention.IFkClientId = id;
                _context.Add(model.Intervention);
                await _context.SaveChangesAsync();
                if (model.Choice==1)
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
