using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using WFConFin.Data;
using WFConFin.Models;

namespace WFConFin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class PessoaController : Controller
    {
        private readonly WFConfinDbContext _context;

        public PessoaController(WFConfinDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetPessoas()
        {
            try
            {
                var result = _context.Pessoa.ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na listagem de pessoas. Exceção: {ex.Message}");

            }

        }
        [HttpPost]
        [Authorize(Roles = "Gerente, Empregado")]
        public async Task<IActionResult> PostPessoa([FromBody] Pessoa pessoa)
        {
            try
            {
                await _context.Pessoa.AddAsync(pessoa);
                var valor = await _context.SaveChangesAsync();
                if (valor == 1)
                {
                    return Ok("Sucesso, pessoa incluída.");
                }
                else
                {
                    return BadRequest($"Erro, pessoa não incluída.");
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na inclusão de pessoa. Exceção: {ex.Message}");

            }

        }

        [HttpPut]
        [Authorize(Roles = "Gerente, Empregado")]
        public async Task<IActionResult> PutPessoa([FromBody] Pessoa pessoa)
        {
            try
            {
                _context.Pessoa.Update(pessoa);
                var valor = await _context.SaveChangesAsync();
                if (valor == 1)
                {
                    return Ok("Sucesso, pessoa alterada.");
                }
                else
                {
                    return BadRequest($"Erro, pessoa não alterado.");
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na alteração de pessoa. Exceção: {ex.Message}");

            }

        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> DeletePessoa([FromRoute] Guid id)
        {
            try
            {
                Pessoa pessoa = await _context.Pessoa.FindAsync(id);
                if (pessoa != null)
                {
                    _context.Pessoa.Remove(pessoa);
                    var valor = await _context.SaveChangesAsync();
                    if (valor == 1)
                    {
                        return Ok("Sucesso, pessoa excluída.");
                    }
                    else
                    {
                        return BadRequest("Erro , pessoa não excluída.");
                    }
                }
                else
                {
                    return NotFound($"Erro, pessoa não existe.");
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na exclusão de pessoa. Exceção: {ex.Message}");

            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPessoa([FromRoute] Guid id)
        {
            try
            {
                Pessoa pessoa = await _context.Pessoa.FindAsync(id);
                if (pessoa != null)
                {
                    return Ok(pessoa);
                }
                else
                {
                    return NotFound("Erro, pessoa não existe.");
                }


            }
            catch (Exception ex)
            {
                return BadRequest($"erro na consulta  de pessoa. Exceção: {ex.Message}");
            }
        }

        [HttpGet("Pesquisa")]
        public async Task<IActionResult> GetPessoaPesquisa([FromQuery] string valor)
        {
            try
            {
                //Query Criteria
                var lista = from o in _context.Pessoa.ToList()
                            where o.Nome.ToUpper().Contains(valor.ToUpper())
                            || o.Telefone.ToUpper().Contains(valor.ToUpper())
                            || o.Email.ToUpper().Contains(valor.ToUpper())
                            select o;

                return Ok(lista);

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro,  pesquisa de pessoa. Exceção: {ex.Message}");
            }
        }

        [HttpGet("Paginacao")]
        public async Task<IActionResult> GetPessoaPaginacao([FromQuery] string? valor, int skip, int take, bool ordemDesc)
        {
            try
            {
                //Query Criteria
                var lista = from o in _context.Pessoa.ToList()
                            select o;
                if (!String.IsNullOrEmpty(valor))
                {
                    lista = from o in lista
                     where o.Nome.ToUpper().Contains(valor.ToUpper())
                           || o.Telefone.ToUpper().Contains(valor.ToUpper())
                           || o.Email.ToUpper().Contains(valor.ToUpper())
                            select o;

                }

                if (ordemDesc)
                {
                    lista = from o in lista
                            orderby o.Nome descending
                            select o;
                }
                else
                {
                    lista = from o in lista
                            orderby o.Nome ascending
                            select o;
                }
                var qtde = lista.Count();

                lista = lista
                    .Skip((skip-1) * take)
                    .Take(take)
                    .ToList();

                var paginacaoResponse = new PaginacaoResponse<Pessoa>(lista, qtde, skip, take);

                return Ok(paginacaoResponse);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro,  pesquisa de pessoa. Exceção: {ex.Message}");
            }
        }
    }
}
