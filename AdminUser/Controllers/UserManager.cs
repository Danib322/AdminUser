using AdminUser.Models;
using AdminUser.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminUser.Controllers
{
    public class UserManager
    {
        private RentCarContext context;

        public UserManager(RentCarContext context)
        {
            this.context = context;
        }

        private int GetMaxIdUsuario()
        {
            if (this.context.Usuario.Count() == 0)
            {
                return 1;
            }
            else
            {
                return this.context.Usuario.Max(z => z.UsuarioId) + 1;
            }
        }

        private bool ExisteEmail(string email)
        {
            var consulta = from datos in this.context.Usuario
                           where datos.Email == email
                           select datos;
            if (consulta.Count() > 0)
            {
                //El email existe en la base de datos
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RegistrarUsuario(string email, string password, string nombre, string apellido, string tipo)
        {
            bool ExisteEmail = this.ExisteEmail(email);
            if (ExisteEmail)
            {
                return false;
            }
            else
            {
                
                Usuario usuario = new Usuario();
                usuario.Email = email;
                usuario.Nombre = nombre;
                usuario.Apellido = apellido;
                usuario.Tipo = tipo;
                //GENERAMOS UN SALT ALEATORIO PARA CADA USUARIO
                usuario.Salt = HelperCryptography.GenerateSalt();
                //GENERAMOS SU PASSWORD CON EL SALT
                usuario.Pass = HelperCryptography.EncriptarPassword(password, usuario.Salt);
                usuario.pwd = password;
                this.context.Usuario.Add(usuario);
                this.context.SaveChanges();

                return true;
            }
        }

        public Usuario LogInUsuario(string email, string password)
        {
            Usuario usuario = this.context.Usuario.SingleOrDefault(x => x.Email == email);
            if (usuario == null)
            {
                return null;
            }
            else
            {
                //Debemos comparar con la base de datos el password haciendo de nuevo el cifrado con cada salt de usuario
                byte[] passUsuario = usuario.Pass;
                string salt = usuario.Salt;
              
                bool respuesta = HelperCryptography.ValidarPassword(password, usuario);
                
                if (respuesta == true)
                {
                    return usuario;
                }
                else
                {
                    //Contraseña incorrecta
                    return null;
                }
            }
        }

        public List<Usuario> GetUsuarios()
        {
            var consulta = from datos in this.context.Usuario
                           select datos;
            return consulta.ToList();
        }
    }

}
