using AdminUser.Models;
using AdminUser.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AdminUser.Controllers
{
    public class HomeController : Controller
    {
        private UserManager repo;

        private readonly ILogger<HomeController> _logger;

        public HomeController(UserManager repo)
        {
            this.repo = repo;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Registro(string email, string password, string nombre, string apellidos, string tipo)
        {
            bool registrado = this.repo.RegistrarUsuario(email, password, nombre, apellidos, tipo);
            if (registrado)
            {
                ViewData["MENSAJE"] = "Usuario registrado con exito";
            }
            else
            {
                ViewData["MENSAJE"] = "Error al registrar el usuario";
            }
            return View();
        }
        [AuthorizeUsers]
        public IActionResult PaginaProtegida()
        {
            return View();
        }

        [AuthorizeUsers(Policy = "ADMINISTRADORES")]
        public IActionResult AdminUsuarios()
        {
            List<Usuario> usuarios = this.repo.GetUsuarios();
            return View(usuarios);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
