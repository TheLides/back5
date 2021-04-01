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
    public class DiagnosesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiagnosesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Diagnoses
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
            var diagnoses = await this._context.Diagnoses
                .Include(w => w.Patient)
                .Where(x => x.PatientId == patientId)
                .ToListAsync();

            return this.View(diagnoses);
        }

        // GET: Diagnoses/Details/5
        public async Task<IActionResult> Details(Int32? id, Int32? patientId)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var diagnosis = await this._context.Diagnoses
                .Include(d => d.Patient)
                .SingleOrDefaultAsync(m => m.DiagnosisId == id && m.PatientId == patientId);
            if (diagnosis == null)
            {
                return this.NotFound();
            }
            this.ViewBag.Diagnosis = diagnosis;
            return this.View(diagnosis);
        }

        // GET: Diagnoses/Create
        public async Task<IActionResult> Create(Int32? patientId)
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

            return this.View(new DiagnosisCreateModel());
        }

        // POST: Diagnoses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Int32? patientId, DiagnosisCreateModel model)
        {
            if (patientId == null)
            {
                return this.NotFound();
            }

            var patient = await this._context.Patients
                .Include(d => d.Diagnoses)
                .SingleOrDefaultAsync(x => x.Id == patientId);

            if (patient == null)
            {
                return this.NotFound();
            }

            if (ModelState.IsValid)
            {
                var diagnosis = new Diagnosis
                {
                    PatientId = patient.Id,
                    Type = model.Type,
                    Complications = model.Complications,
                    Details = model.Details,
                    DiagnosisId = patient.Diagnoses.Any() ? patient.Diagnoses.Max(x => x.DiagnosisId) + 1 : 1
                };
                _context.Add(diagnosis);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { patientId = patient.Id});
            }
            this.ViewBag.Patient = patient;
            return this.View(model);
        }

        // GET: Diagnoses/Edit/5
        public async Task<IActionResult> Edit(Int32? id, Int32? patientId)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var diagnosis = await this._context.Diagnoses
                .Include(p => p.Patient)
                .SingleOrDefaultAsync(m => m.DiagnosisId == id && m.PatientId == patientId);
            if (diagnosis == null)
            {
                return this.NotFound();
            }

            var model = new DiagnosisCreateModel
            {
                Type = diagnosis.Type,
                Complications = diagnosis.Complications,
                Details = diagnosis.Details
            };

            this.ViewBag.Diagnosis = diagnosis;
            return this.View(model);
        }

        // POST: Diagnoses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Int32? id, DiagnosisCreateModel model)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var diagnosis = await this._context.Diagnoses.SingleOrDefaultAsync(m => m.DiagnosisId == id);
            if (diagnosis == null)
            {
                return this.NotFound();
            }

            if (ModelState.IsValid)
            {
                diagnosis.Type = model.Type;
                diagnosis.Complications = model.Complications;
                diagnosis.Details = model.Details;
                await this._context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { patientId = diagnosis.PatientId});
            }
            this.ViewBag.Diagnosis = diagnosis;
            return this.View(model);
        }

        // GET: Diagnoses/Delete/5
        public async Task<IActionResult> Delete(Int32? id, Int32? patientId)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var diagnosis = await this._context.Diagnoses
                .Include(d => d.Patient)
                .SingleOrDefaultAsync(m => m.DiagnosisId == id && m.PatientId == patientId);
            if (diagnosis == null)
            {
                return this.NotFound();
            }
            this.ViewBag.Diagnosis = diagnosis;
            return this.View(diagnosis);
        }

        // POST: Diagnoses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int patientId)
        {
            var diagnosis = await _context.Diagnoses.Include(p => p.Patient).SingleOrDefaultAsync(m => m.DiagnosisId == id && m.PatientId == patientId);
            this._context.Diagnoses.Remove(diagnosis);
            await this._context.SaveChangesAsync();
            this.ViewBag.Diagnosis = diagnosis;
            return this.RedirectToAction(nameof(Index), new { patientId = diagnosis.PatientId });
        }

    }
}
