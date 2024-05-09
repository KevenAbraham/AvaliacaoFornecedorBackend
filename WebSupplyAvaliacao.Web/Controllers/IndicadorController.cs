using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using WebSupplyAvaliacao.Dados.Context;
using WebSupplyAvaliacao.Web.Models;

namespace WebSupplyAvaliacao.Web.Controllers;

[Authorize]
[ServiceFilter(typeof(Validacao))]
public class IndicadorController : Controller
{
    private readonly AppDbContext _context;

    public IndicadorController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var qp = _context.Avaliar.Where(x => x.ServicoAvaliadoId == 1).Count();
        var qs = _context.Avaliar.Where(x => x.ServicoAvaliadoId == 2).Count();
        var dt = _context.Avaliar.Where(x => x.ServicoAvaliadoId == 3).Count();

        int qps1 = 0;
        int qps2 = 0;
        int qps3 = 0;
        int qps4 = 0;
        int qps5 = 0;

        for (int i = 0; i < qp; i++)
        {
            var star = _context.Avaliar.Where(x => x.ServicoAvaliadoId == 1).Skip(i).Select(x => x.Nota).FirstOrDefault();

            switch (star) 
            {
                case 1:
                    qps1++;
                    break;
                case 2:
                    qps2++;
                    break;
                case 3:
                    qps3++;
                    break;
                case 4: 
                    qps4++;
                    break;
                case 5:
                    qps5++;
                    break;
                default:
                    throw new InvalidOperationException("Invalid method");
            }
        }

        ViewBag.qps1 = qps1;
        ViewBag.qps2 = qps2;
        ViewBag.qps3 = qps3;
        ViewBag.qps4 = qps4;
        ViewBag.qps5 = qps5;

        int qss1 = 0;
        int qss2 = 0;
        int qss3 = 0;
        int qss4 = 0;
        int qss5 = 0;

        for (int i = 0; i < qs; i++)
        {
            var star = _context.Avaliar.Where(x => x.ServicoAvaliadoId == 2).Skip(i).Select(x => x.Nota).FirstOrDefault();

            switch (star)
            {
                case 1:
                    qss1++;
                    break;
                case 2:
                    qss2++;
                    break;
                case 3:
                    qss3++;
                    break;
                case 4:
                    qss4++;
                    break;
                case 5:
                    qss5++;
                    break;
                default:
                    throw new InvalidOperationException("Invalid method");
            }
        }

        ViewBag.qss1 = qss1;
        ViewBag.qss2 = qss2;
        ViewBag.qss3 = qss3;
        ViewBag.qss4 = qss4;
        ViewBag.qss5 = qss5;

        int dts1 = 0;
        int dts2 = 0;
        int dts3 = 0;
        int dts4 = 0;
        int dts5 = 0;

        for (int i = 0; i < dt; i++)
        {
            var star = _context.Avaliar.Where(x => x.ServicoAvaliadoId == 3).Skip(i).Select(x => x.Nota).FirstOrDefault();

            switch (star)
            {
                case 1:
                    dts1++;
                    break;
                case 2:
                    dts2++;
                    break;
                case 3:
                    dts3++;
                    break;
                case 4:
                    dts4++;
                    break;
                case 5:
                    dts5++;
                    break;
                default:
                    throw new InvalidOperationException("Invalid method");
            }
        }

        ViewBag.dts1 = dts1;
        ViewBag.dts2 = dts2;
        ViewBag.dts3 = dts3;
        ViewBag.dts4 = dts4;
        ViewBag.dts5 = dts5;

        return View();
    }
}
