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
    public class CidadeController : Controller
    {
        private readonly WFConfinDbContext _context;

        public CidadeController(WFConfinDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCidades()
        {
            try
            {
                var result = _context.Cidade.ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"erro na listagem de cidades. Exceção: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Gerente, Empregado")]
        public async Task<IActionResult> PostCidades([FromBody] Cidade cidade)
        {
            try
            {
                await _context.Cidade.AddAsync(cidade);
                var valor = await _context.SaveChangesAsync();
                if (valor == 1)
                {
                    return Ok("Sucesso, cidade incluída.");
                }
                else
                {
                    return BadRequest("Erro, cidade não incluída.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"erro na inclusão  de cidade. Exceção: {ex.Message}");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Gerente, Empregado")]
        public async Task<IActionResult> PutCidades([FromBody] Cidade cidade)
        {
            try
            {
                _context.Cidade.Update(cidade);
                var valor = await _context.SaveChangesAsync();
                if (valor == 1)
                {
                    return Ok("Sucesso, cidade alterada.");
                }
                else
                {
                    return BadRequest("Erro, cidade não alterada.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"erro na alteração  de cidade. Exceção: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> DeleteCidade([FromRoute] Guid id)
        {
            try
            {
                Cidade cidade = await _context.Cidade.FindAsync(id);
                if (cidade != null)
                {
                    _context.Cidade.Remove(cidade);
                    var valor = await _context.SaveChangesAsync();
                    if (valor == 1)
                    {
                        return Ok("Sucesso, cidade excluída.");
                    }
                    else
                    {
                        return BadRequest("Erro, cidade não excluída.");
                    }

                }
                else
                {
                    return NotFound("Erro, cidade não existe.");
                }


            }
            catch (Exception ex)
            {
                return BadRequest($"erro na axclusão  de cidade. Exceção: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCidade([FromRoute] Guid id)
        {
            try
            {
                Cidade cidade = await _context.Cidade.FindAsync(id);
                if (cidade != null)
                {
                    return Ok(cidade);
                }
                else
                {
                    return NotFound("Erro, cidade não existe.");
                }


            }
            catch (Exception ex)
            {
                return BadRequest($"erro na consulta  de cidade. Exceção: {ex.Message}");
            }
        }

        [HttpGet("Pesquisa")]
        public async Task<IActionResult> GetCidadePesquisa([FromQuery] string valor)
        {
            try
            {
                //Query Criteria
                var lista = from o in _context.Cidade.ToList()
                            where o.Nome.ToUpper().Contains(valor.ToUpper())
                            || o.EstadoSigla.ToUpper().Contains(valor.ToUpper())
                            select o;

                return Ok(lista);

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro,  pesquisa de cidade. Exceção: {ex.Message}");
            }
        }

        [HttpGet("Paginacao")]
        public async Task<IActionResult> GetCidadePaginacao([FromQuery] string? valor, int skip, int take, bool ordemDesc)
        {
            try
            {
                //Query Criteria
                var lista = from o in _context.Cidade.ToList()
                            select o;

                if (!String.IsNullOrEmpty(valor))
                {
                    lista = from o in lista
                            where o.Nome.ToUpper().Contains(valor.ToUpper())
                                    || o.EstadoSigla.ToUpper().Contains(valor.ToUpper())
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
                    .Skip((skip - 1) * take)
                    .Take(take)
                    .ToList();

                var paginacaoResponse = new PaginacaoResponse<Cidade>(lista, qtde, skip, take);

                return Ok(paginacaoResponse);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro,  pesquisa de cidade. Exceção: {ex.Message}");
            }
        }
    }
}
