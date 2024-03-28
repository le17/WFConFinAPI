using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using System.Threading.Tasks;
using WFConFin.Data;
using WFConFin.Models;
using WFConFin.Services;
using Microsoft.AspNetCore.Authorization;

namespace WFConFin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsuarioController : Controller
    {
        private readonly WFConfinDbContext _context;
        private readonly TokenService _service;

        public UsuarioController(WFConfinDbContext context, TokenService service)
        {
            _service = service;
            _context = context;
        }

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UsuarioLogin usuarioLogin)
        {
            var usuario = _context.Usuario.Where(x => x.Login == usuarioLogin.Login).FirstOrDefault();
            if (usuario == null)
            {
                return NotFound("Usuário inválido.");
            }

            var passwordHash = MD5Hash.CalcHash(usuarioLogin.Password);

            if(usuario.Password != passwordHash)
            {
                return BadRequest("Senha inválida.");
            }

            var token = _service.GerarToken(usuario);

            usuario.Password = "";

            var result = new UsuarioResponse()
            {
                Usuario = usuario,
                Token = token
            };
            return Ok(result);
            
        }

        [HttpGet]
        public async Task<IActionResult> GetUsuario()
        {
            try
            {
                var result = _context.Usuario.ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na listagem de usuários. Exceção: {ex.Message}");

            }

        }

        [HttpPost]
        [Authorize(Roles = "Gerente, Empregado")]
        public async Task<IActionResult> PostUsuario([FromBody] Usuario usuario)
        {
            try
            {
                var listUsuario = _context.Usuario.Where(x => x.Login == usuario.Login).ToList();
                if(listUsuario.Count > 0)
                {
                    return BadRequest("Erro, informação de login inválido.");
                }

                string passwordHash = MD5Hash.CalcHash(usuario.Password);

                usuario.Password = passwordHash;


                await _context.Usuario.AddAsync(usuario);
                var valor = await _context.SaveChangesAsync();
                if (valor == 1)
                {
                    return Ok("Sucesso, usuário incluído.");
                }
                else
                {
                    return BadRequest($"Erro, usuário não incluído.");
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na inclusão de usuário. Exceção: {ex.Message}");

            }

        }

        [HttpPut]
        [Authorize(Roles = "Gerente, Empregado")]
        public async Task<IActionResult> PutUsuario([FromBody] Usuario usuario)
        {
            try
            {
                string passwordHash = MD5Hash.CalcHash(usuario.Password);

                usuario.Password = passwordHash;

                _context.Usuario.Update(usuario);
                var valor = await _context.SaveChangesAsync();
                if (valor == 1)
                {
                    return Ok("Sucesso, usuário alterado.");
                }
                else
                {
                    return BadRequest($"Erro, usuário não alterado.");
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na alteração de usuário. Exceção: {ex.Message}");

            }

        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> DeleteUsuario([FromRoute] Guid id)
        {
            try
            {
                Usuario usuario = await _context.Usuario.FindAsync(id);
                if (usuario != null)
                {
                    _context.Usuario.Remove(usuario);
                    var valor = await _context.SaveChangesAsync();
                    if (valor == 1)
                    {
                        return Ok("Sucesso, usuário excluído.");
                    }
                    else
                    {
                        return BadRequest("Erro , usuário não excluído.");
                    }
                }
                else
                {
                    return NotFound($"Erro, usuário não existe.");
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na exclusão de usuário. Exceção: {ex.Message}");

            }

        }
    }
}
