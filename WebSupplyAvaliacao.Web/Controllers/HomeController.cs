using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebSupplyAvaliacao.Dados.Context;
using WebSupplyAvaliacao.Web.Models;

namespace WebSupplyAvaliacao.Web.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(Validacao))]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            int qtdFornecedores = _context.Fornecedor.Count(); //quantidade total de fornecedores
            ViewBag.QtdFornecedor = qtdFornecedores;

            DateTime dataLimite = DateTime.Today.AddMonths(-1);
            int qtdFornecedoresMonth = _context.Fornecedor
                .Count(f => f.Data >= dataLimite); //fornecedores cadastrados no ultimo mes

            ViewBag.QtdFornecedoresUltimoMes = qtdFornecedoresMonth;

            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}