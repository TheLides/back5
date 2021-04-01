using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Backend5.Data;
using Backend5.Models;
using Backend5.Models.ViewModels;

namespace Backend5.Controllers
{
    public class AnalysesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnalysesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Analyses
        public async Task<IActionResult> Index(Int32? patientId)
        {
            if (patientId == null)
            {
                return this.NotFound();
            }

            var patient = await this._context.Patients
                .SingleOrDefaultAsync(x => x.Id == patientId);

            if (patient == null)
            {
                return this.NotFound();
            }

            this.ViewBag.Patient = patient;

            var analysis = await _context.Analyses
                .Include(a => a.Lab)
                .Include(a => a.Patient)
                .Where(x => x.PatientId == patientId)
                .ToListAsync();
            return View(analysis);
        }

        // GET: Analyses/Details/5
        public async Task<IActionResult> Details(Int32? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var analysis = await _context.Analyses
                .Include(a => a.Lab)
                .Include(a => a.Patient)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (analysis == null)
            {
                return NotFound();
            }

            this.ViewBag.Analysis = analysis;
            return View(analysis);
        }

        // GET: Analyses/Create
        public async Task<IActionResult> Create(Int32? patientId)
        {
            if (patientId == null)
            {
                return this.NotFound();
            }

            var patient = await this._context.Patients
                .Include(x => x.Placements)
                .ThenInclude(x => x.Ward)
                .ThenInclude(x => x.Hospital)
                .ThenInclude(x => x.Labs)
                .ThenInclude(x => x.Lab)
                .SingleOrDefaultAsync(x => x.Id == patientId);

            if (patient == null)
            {
                return this.NotFound();
            }
            var labsOfHospital = patient.Placements.SelectMany(x => x.Ward.Hospital.Labs.Select(y => y.Lab)).ToList();
            this.ViewBag.Patient = patient;
            ViewData["LabId"] = new SelectList(labsOfHospital, "Id", "Name");
            return this.View(new AnalysisViewModel());
        }

        // POST: Analyses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Int32? patientId, AnalysisViewModel model)
        {
            if (patientId == null)
            {
                return this.NotFound();
            }

            var patient = await this._context.Patients
                .SingleOrDefaultAsync(x => x.Id == patientId);

            if (patient == null)
            {
                return this.NotFound();
            }

            if (ModelState.IsValid)
            {
                var analysis = new Analysis
                {
                    PatientId = patient.Id,
                    LabId = model.LabId,
                    Type = model.Type,
                    Date = model.Date,
                    Status = model.Status
                };
                this._context.Add(analysis);
                await this._context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { patientId = patient.Id });
            }
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "Name", model.LabId);
            this.ViewBag.Patient = patient;
            return View(model);
        }

        // GET: Analyses/Edit/5
        public async Task<IActionResult> Edit(Int32? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var analysis = await _context.Analyses.SingleOrDefaultAsync(m => m.Id == id);
            if (analysis == null)
            {
                return NotFound();
            }
            var model = new AnalysisViewModel
            {
                LabId = analysis.LabId,
                Type = analysis.Type,
                Date = analysis.Date,
                Status = analysis.Status
            };
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "Name", analysis.LabId);
            this.ViewBag.Analysis = analysis;
            return View(model);
        }

        // POST: Analyses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Int32? id, AnalysisViewModel model)
        {
            if (id == null)
            {
                return NotFound();
            }

            var analysis = await this._context.Analyses
                .SingleOrDefaultAsync(a => a.Id == id);

            if(analysis == null)
            {
                return this.NotFound();
            }

            if (this.ModelState.IsValid)
            {
                analysis.LabId = model.LabId;
                analysis.Type = model.Type;
                analysis.Status = model.Status;
                analysis.Date = model.Date;
                await this._context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { patientId = analysis.PatientId});
            }
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "Name", analysis.LabId);
            this.ViewBag.Analysis = analysis;
            return View(model);
        }

        // GET: Analyses/Delete/5
        public async Task<IActionResult> Delete(Int32? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var analysis = await _context.Analyses
                .Include(a => a.Lab)
                .Include(a => a.Patient)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (analysis == null)
            {
                return NotFound();
            }

            this.ViewBag.Analysis = analysis;
            return View(analysis);
        }

        // POST: Analyses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var analysis = await _context.Analyses.SingleOrDefaultAsync(m => m.Id == id);
            _context.Analyses.Remove(analysis);
            await _context.SaveChangesAsync();
            this.ViewBag.Analysis = analysis;
            return this.RedirectToAction(nameof(Index), new { patientId = analysis.PatientId });
        }
    }
}
