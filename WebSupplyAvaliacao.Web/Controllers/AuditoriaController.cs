using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebSupplyAvaliacao.Dados.Context;
using WebSupplyAvaliacao.Web.Models;
using WebSupplyAvaliacao.Web.ViewModel;

namespace WebSupplyAvaliacao.Web.Controllers;

[Authorize]
[ServiceFilter(typeof(Validacao))]
public class AuditoriaController : Controller
{
    private readonly AppDbContext _context;

    public AuditoriaController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index([FromQuery] DateTime? dataInicio, [FromQuery] DateTime? dataFinal)
    {
        int mesAtual = DateTime.Now.Month;
        int anoAtual = DateTime.Now.Year;

        DateTime dataInicioMes = new DateTime(anoAtual, mesAtual, 1);
        DateTime dataFinalMes = DateTime.Now;

        if (dataInicio == null)
        {
            dataInicio = dataInicioMes;
        }

        if (dataFinal == null)
        {
            dataFinal = dataFinalMes;
        }
        else
        {
            // Para incluir a data final até o final do dia
            dataFinal = dataFinal.Value.AddDays(1).AddTicks(-1);
        }

        ViewBag.MesAtual = dataInicio?.ToString("yyyy-MM-dd") ?? dataInicioMes.ToString("yyyy-MM-dd");
        ViewBag.AnoAtual = dataFinal?.ToString("yyyy-MM-dd") ?? dataFinalMes.ToString("yyyy-MM-dd");

        var auditoria = await _context.Auditoria
                .Include(a => a.Usuario)
                .Include(a => a.Acao)
                .Where(a => a.Data >= dataInicio && a.Data <= dataFinal)
                .Select(a => new AuditoriaViewModel
                {
                    Data = a.Data,
                    NomeUsuario = a.Usuario.Nome,
                    Acao = a.Acao.Descricao,
                    Chave = a.Chave
                })
                .ToListAsync();

        return View(auditoria);
    }
}
