using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Reports.DAL;
using Reports.Models;

namespace Reports.Controllers
{
    public class ItemsController : Controller
    {
        private readonly ApplicationDBContext _context;

        public ItemsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: Items
        public async Task<IActionResult> Index()
        {
            return View(await _context.Items.ToListAsync());
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Item = await _context.Items
                .FirstOrDefaultAsync(m => m.name == id);
            if (Item == null)
            {
                return NotFound();
            }

            return View(Item);
        }

        // GET: Items/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("name,parentId")] Item Item)
        {
            if (ModelState.IsValid)
            {
                _context.Add(Item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(Item);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Item = await _context.Items.FindAsync(id);
            if (Item == null)
            {
                return NotFound();
            }
            return View(Item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("name,parentId")] Item Item)
        {
            if (id != Item.name)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(Item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(Item.name))
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
            return View(Item);
        }

        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Item = await _context.Items
                .FirstOrDefaultAsync(m => m.name == id);
            if (Item == null)
            {
                return NotFound();
            }

            return View(Item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var Item = await _context.Items.FindAsync(id);
            _context.Items.Remove(Item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(string id)
        {
            return _context.Items.Any(e => e.name == id);
        }
    }
}
