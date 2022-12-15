using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AdminUser.Models;
using AdminUser.Util;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace AdminUser.Controllers
{
    public class ReservasController : Controller
    {

        private readonly RentCarContext _context;



        public ReservasController(RentCarContext context)
        {
            _context = context;
        }
        private int getUsuarioId()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var usuarioId = claimsIdentity.Claims.Where(cl => cl.Type == "IdUsuario").FirstOrDefault();
            return int.Parse(usuarioId.Value);
        }

        // GET: Reservas
        [AuthorizeUsers]
        public async Task<IActionResult> Index()
        {
            var rentCarContext = _context.Reserva.Include(r => r.Auto).Include(r => r.Usuario);
            return View(await rentCarContext.ToListAsync());
        }

        [AuthorizeUsers]
        public async Task<IActionResult> UsuariosReserve()
        {

            var usuarioId = getUsuarioId();
            var rentCarContext = _context.Reserva.Where(r => r.UsuarioId == usuarioId && r.EsActiva == true).Include(r => r.Auto).Include(r => r.Usuario);
            return View(await rentCarContext.ToListAsync());
        }

        // GET: Reservas/Details/5
        [AuthorizeUsers]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reserva
                .Include(r => r.Auto)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(m => m.ReservaId == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // GET: Reservas/Create
        [AuthorizeUsers]
        public IActionResult Create()
        {
            ViewData["AutoId"] = new SelectList(_context.Automovil.Where(a => a.EstaDisponible == true), "AutoId", "Marca");
            ViewData["UsuarioId"] = new SelectList(_context.Usuario, "UsuarioId", "Apellido");
            return View();
        }

        // POST: Reservas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUsers]
        public async Task<IActionResult> Create([Bind("ReservaId,UsuarioId,AutoId,FechaInicio,FechaFin")] Reserva reserva)
        {

            if (ModelState.IsValid)
            {

                var claimsIdentity = (ClaimsIdentity)User.Identity;

                var usuarioId = claimsIdentity.Claims.Where(cl => cl.Type == "IdUsuario").FirstOrDefault();

                reserva.UsuarioId = int.Parse(usuarioId.Value);
                reserva.EsActiva = true;

                _context.Add(reserva);
                await _context.SaveChangesAsync();

                var autoSelect = _context.Automovil.Where(a => a.AutoId == reserva.AutoId).FirstOrDefault();
                autoSelect.EstaDisponible = false;
                _context.Update(autoSelect);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(UsuariosReserve));

            }
            ViewData["AutoId"] = new SelectList(_context.Automovil, "AutoId", "Marca", reserva.AutoId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuario, "UsuarioId", "Apellido", reserva.UsuarioId);
            return View(reserva);
        }

        // GET: Reservas/Edit/5
        [AuthorizeUsers]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reserva.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }
            ViewData["AutoId"] = new SelectList(_context.Automovil, "AutoId", "Marca", reserva.AutoId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuario, "UsuarioId", "Apellido", reserva.UsuarioId);
            return View(reserva);
        }

        // POST: Reservas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUsers]
        public async Task<IActionResult> Edit(int id, [Bind("ReservaId,UsuarioId,AutoId,FechaInicio,FechaFin")] Reserva reserva)
        {
            if (id != reserva.ReservaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var usuarioId = getUsuarioId();
                    reserva.UsuarioId = usuarioId;
                    _context.Update(reserva);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaExists(reserva.ReservaId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(UsuariosReserve));
            }
            ViewData["AutoId"] = new SelectList(_context.Automovil, "AutoId", "Marca", reserva.AutoId);

            return View(reserva);
        }

        // GET: Reservas/Delete/5
        [AuthorizeUsers]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reserva
                .Include(r => r.Auto)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(m => m.ReservaId == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // POST: Reservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeUsers]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reserva = await _context.Reserva.FindAsync(id);
            _context.Reserva.Remove(reserva);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // GET: Reservas/EntregaVehiculo/5
        [AuthorizeUsers]
        public async Task<IActionResult> EntregaVehiculo(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reserva
                .Include(r => r.Auto)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(m => m.ReservaId == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        [HttpPost, ActionName("EntregaVehiculo")]
        [AuthorizeUsers]
        public async Task<IActionResult> Entrega(int? id)
        {
            var reserva = await _context.Reserva.FindAsync(id);
            var auto = _context.Automovil.Where(a => a.AutoId == reserva.AutoId).FirstOrDefault();
            auto.EstaDisponible = true;
            reserva.EsActiva = false;
            _context.Update(auto);
            _context.Update(reserva);
            _context.SaveChanges();
            return  RedirectToAction("UsuariosReserve");
        }

        private bool ReservaExists(int id)
        {
            return _context.Reserva.Any(e => e.ReservaId == id);
        }
    }
}
