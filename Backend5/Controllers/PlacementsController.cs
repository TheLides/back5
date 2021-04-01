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
    public class PlacementsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PlacementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Placements
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

            var placements = await _context.Placements
                .Include(p => p.Patient)
                .Include(p => p.Ward)
                .Where(x => x.PatientId == patientId)
                .ToListAsync();
            return this.View(placements);
        }

        // GET: Placements/Details/5
        public async Task<IActionResult> Details(Int32? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var placement = await _context.Placements
                .Include(p => p.Patient)
                .Include(p => p.Ward)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (placement == null)
            {
                return this.NotFound();
            }
            this.ViewBag.Placement = placement;
            return this.View(placement);
        }

        // GET: Placements/Create
        public async Task<IActionResult> Create(Int32? patientId)
        {
            if (patientId == null)
            {
                return this.NotFound();
            }

            var patient = await this._context.Patients
                .SingleOrDefaultAsync(x => x.Id == patientId);

            var patient2 = await this._context.Patients
                .Include(s => s.Doctors)
                .ThenInclude(s => s.Doctor)
                .ThenInclude(s => s.Hospitals)
                .ThenInclude(s => s.Hospital)
                .ThenInclude(s => s.Wards)
                .SingleOrDefaultAsync(x => x.Id == patientId);
            if (patient == null)
            {
                return this.NotFound();
            }

            //var wardsOfHospital = patient2.Doctors.SelectMany(x => x.Doctor.Hospitals.SelectMany(y => y.Hospital.Wards)).ToList(); - не работает
            this.ViewBag.Patient = patient;
            this.ViewData["WardId"] = new SelectList(_context.Wards, "Id", "Name");
            return this.View(new PlacementViewModel());
        }

        // POST: Placements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Int32? patientId, PlacementViewModel model)
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

            if (this.ModelState.IsValid)
            {
                var placement = new Placement
                {
                    PatientId = patient.Id,
                    WardId = model.WardId,
                    Bed = model.Bed
                };
                this._context.Add(placement);
                await this._context.SaveChangesAsync();
                return this.RedirectToAction(nameof(Index), new { patientId = patient.Id});
            }
            this.ViewBag.Patient = patient;
            this.ViewData["WardId"] = new SelectList(this._context.Wards, "Id", "Name", model.WardId);
            return this.View(model);
        }

        // GET: Placements/Edit/5
        public async Task<IActionResult> Edit(Int32? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var placement = await _context.Placements.SingleOrDefaultAsync(m => m.Id == id);
            if (placement == null)
            {
                return this.NotFound();
            }

            var model = new PlacementViewModel
            {
                WardId = placement.WardId,
                Bed = placement.Bed
            };
            this.ViewBag.Placement = placement;
            ViewData["WardId"] = new SelectList(_context.Wards, "Id", "Name", placement.WardId);
            return View(model);
        }

        // POST: Placements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Int32? id, PlacementViewModel model)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var placement = await this._context.Placements
                .SingleOrDefaultAsync(m => m.Id == id);

            if (placement == null)
            {
                return this.NotFound();
            }
            if (this.ModelState.IsValid)
            {
                placement.WardId = model.WardId;
                placement.Bed = model.Bed;
                await this._context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { patientId = placement.PatientId});
            }
            this.ViewBag.Placement = placement;
            ViewData["WardId"] = new SelectList(_context.Wards, "Id", "Name", placement.WardId);
            return View(model);
        }

        // GET: Placements/Delete/5
        public async Task<IActionResult> Delete(Int32? patientId, Int32? wardId)
        {
            if (patientId == null || wardId == null)
            {
                return this.NotFound();
            }

            var placement = await this._context.Placements
                .Include(p => p.Patient)
                .Include(p => p.Ward)
                .SingleOrDefaultAsync(m => m.PatientId == patientId && m.WardId == wardId);
            if (placement == null)
            {
                return this.NotFound();
            }
            this.ViewBag.Placement = placement;
            return this.View(placement);
        }

        // POST: Placements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Int32 patientId, Int32 wardId)
        {
            var placement = await _context.Placements.SingleOrDefaultAsync(m => m.PatientId == patientId && m.WardId == wardId);
            _context.Placements.Remove(placement);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { patientId = patientId });
        }
    }
}
