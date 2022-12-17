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
        // GET: Reservas
        [AuthorizeUsers]
        public async Task<IActionResult> TopReservas()
        {
            
            var contexto = (from r in _context.Reserva
                            join a in _context.Automovil on r.AutoId equals a.AutoId
                            join c in _context.Categoria on a.CategoriaId equals c.CategoriaId
                            select new
                            {
                                modelo = a.Modelo,
                                idauto =  r.AutoId,
                                descripcion = c.Descripcion
                             }).ToList();
            var listaId = contexto.Select(r => r.idauto).Distinct().ToList();
            var modelo = new InformeModel();
            foreach (var item in listaId)
            {
                var count = contexto.Where(a => a.idauto == item).Count();
                var Auto = new AutoInforme();
                Auto.NumeroDeReservas = count;
                var AutoSelect = contexto.Where(a => a.idauto == item).FirstOrDefault();
                Auto.Modelo = AutoSelect.modelo;
                Auto.categoriaTop = AutoSelect.descripcion;              
                modelo.Autos.Add(Auto);

            }
            var categoria = modelo.Autos.Select(c => c.categoriaTop).Distinct().FirstOrDefault();
            ViewBag.TopCategorias = categoria;
            modelo.Autos= modelo.Autos.OrderByDescending(a => a.NumeroDeReservas).ToList();
            return View(modelo);

        }

        [AuthorizeUsers]
        public async Task<IActionResult> UsuariosReserve()
        {

            var usuarioId = getUsuarioId();
            var rentCarContext = _context.Reserva.Where(r => r.UsuarioId == usuarioId && r.EsActiva == true)
                                                 .Include(r => r.Auto)
                                                 .Include(r => r.Usuario);
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
            ViewData["AutoId"] = new SelectList(_context.Automovil.Where(a => a.EstaDisponible == true), "AutoId", "Modelo");
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
                    reserva.EsActiva = true;
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
            var EstadoAuto = _context.Automovil.Where(a => a.AutoId == reserva.AutoId).FirstOrDefault();
            EstadoAuto.EstaDisponible = true;
            _context.Update(EstadoAuto);
            _context.Reserva.Remove(reserva);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(UsuariosReserve));
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
            
            var CategoriaAuto =_context.Categoria.Where(c => c.CategoriaId == reserva.Auto.CategoriaId).FirstOrDefault();
           
            var FechaInicial = reserva.FechaInicio;
            var FechaFin = Convert.ToDateTime(DateTime.UtcNow.ToString("yyyy-MM-dd"));
            TimeSpan IntervaloDias = FechaFin - FechaInicial;
            int Numerodias = IntervaloDias.Days;
            ViewBag.IntervaloDias = Numerodias;
            ViewBag.CategoriaAutos = CategoriaAuto.Valor;
            ViewBag.PrecioReserva = CategoriaAuto.Valor * Numerodias;
            

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
