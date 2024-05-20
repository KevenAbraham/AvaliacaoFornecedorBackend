using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebSupplyAvaliacao.Dados.Context;
using WebSupplyAvaliacao.Web.Models;

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

    public IActionResult Index([FromQuery] DateTime? dataInicio, [FromQuery] DateTime? dataFinal)
    {
        var auditoria = _context.Auditoria.ToList();

        int mesAtual = DateTime.Now.Month;
        int anoAtual = DateTime.Now.Year;

        DateTime dataInicioMes = new DateTime(anoAtual, mesAtual, 1);
        DateTime dataFinalMes = DateTime.Now;


        if (dataInicio == null && dataFinal == null)
        {
            dataInicio = dataInicioMes;
            dataFinal = dataFinalMes;
        }

        ViewBag.MesAtual = dataInicio?.ToString("yyyy-MM-dd") ?? dataInicioMes.ToString("yyyy-MM-dd");
        ViewBag.AnoAtual = dataFinal?.ToString("yyyy-MM-dd") ?? dataFinalMes.ToString("yyyy-MM-dd");

        return View(auditoria);
    }
}
