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
    public class WardStaffsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WardStaffsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WardStaffs
        public async Task<IActionResult> Index(Int32? wardId)
        {
            if (wardId == null)
            {
                return this.NotFound();
            }

            var ward = await this._context.Wards
                .SingleOrDefaultAsync(x => x.Id == wardId);

            if(ward == null)
            {
                return this.NotFound();
            }

            this.ViewBag.Ward = ward;
            var wardstaffs = await this._context.WardStaffs
                .Include(w => w.Ward)
                .Where(x => x.WardId == wardId)
                .ToListAsync();

            return this.View(wardstaffs);
        }

        // GET: WardStaffs/Details/5
        public async Task<IActionResult> Details(Int32? id, Int32? wardId)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var wardStaff = await this._context.WardStaffs
                .Include(w => w.Ward)
                .SingleOrDefaultAsync(m => m.StaffId == id && m.WardId == wardId);
            if (wardStaff == null)
            {
                return this.NotFound();
            }
            this.ViewBag.WardStaff = wardStaff;
            return this.View(wardStaff);
        }

        // GET: WardStaffs/Create
        public async Task<IActionResult> Create(Int32? wardId)
        {
            if (wardId == null)
            {
                return this.NotFound();
            }

            var ward = await this._context.Wards
                .SingleOrDefaultAsync(x => x.Id == wardId);

            if (ward == null)
            {
                return this.NotFound();
            }

            this.ViewBag.Ward = ward;
            return View(new WardStaffViewModel());
        }

        // POST: WardStaffs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Int32? wardId, WardStaffViewModel model)
        {
            if (wardId == null)
            {
                return this.NotFound();
            }

            var ward = await this._context.Wards
                .Include(w => w.WardStaffs)
                .SingleOrDefaultAsync(x => x.Id == wardId);

            if (ward == null)
            {
                return this.NotFound();
            }
            if (this.ModelState.IsValid)
            {
                var wardStaff = new WardStaff
                {
                    WardId = ward.Id,
                    Name = model.Name,
                    Position = model.Position,
                    StaffId = ward.WardStaffs.Any() ? ward.WardStaffs.Max(x => x.StaffId) + 1 : 1
                };
                this._context.Add(wardStaff);
                await this._context.SaveChangesAsync();
                return this.RedirectToAction(nameof(Index), new { wardId = ward.Id});
            }
            this.ViewBag.Ward = ward;
            return this.View(model);
        }

        // GET: WardStaffs/Edit/5
        public async Task<IActionResult> Edit(Int32? id, Int32? wardId)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var wardStaff = await this._context.WardStaffs
                .Include(x => x.Ward)
                .SingleOrDefaultAsync(m => m.StaffId == id && m.WardId == wardId);
            if (wardStaff == null)
            {
                return this.NotFound();
            }

            var model = new WardStaffViewModel
            {
                Name = wardStaff.Name,
                Position = wardStaff.Position
            };
            this.ViewBag.WardStaff = wardStaff;
            return this.View(model);
        }

        // POST: WardStaffs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Int32? id, WardStaffViewModel model)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var wardStaff = await this._context.WardStaffs
                .Include(x => x.Ward)
                .SingleOrDefaultAsync(m => m.StaffId == id);
            if (wardStaff == null)
            {
                return this.NotFound();
            }

            if (ModelState.IsValid)
            {
                wardStaff.Name = model.Name;
                wardStaff.Position = model.Position;
                await this._context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { wardId = wardStaff.WardId});
            }
            this.ViewBag.WardStaff = wardStaff;
            return this.View(model); //здесь с wardStaff тоже сработало О_о
        }

        // GET: WardStaffs/Delete/5
        public async Task<IActionResult> Delete(Int32? id, Int32? wardId)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var wardStaff = await this._context.WardStaffs
                .Include(w => w.Ward)
                .SingleOrDefaultAsync(m => m.StaffId == id && m.WardId == wardId);
            if (wardStaff == null)
            {
                return this.NotFound();
            }
            this.ViewBag.WardStaff = wardStaff;
            return this.View(wardStaff);
        }

        // POST: WardStaffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int wardId, int id)
        {
            var wardStaff = await this._context.WardStaffs.Include(x => x.Ward).SingleOrDefaultAsync(m => m.WardId == wardId && m.StaffId == id);
            this._context.WardStaffs.Remove(wardStaff);
            await this._context.SaveChangesAsync();
            this.ViewBag.WardStaff = wardStaff;
            return this.RedirectToAction(nameof(Index), new { wardId = wardStaff.WardId});
        }

    }
}
