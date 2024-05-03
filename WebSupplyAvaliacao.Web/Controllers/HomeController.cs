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

            DateTime primeiroDiaMesAtual = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            DateTime primeiroDiaProximoMes = primeiroDiaMesAtual.AddMonths(1);

            int qtdFornecedoresMonth = _context.Fornecedor
                .Count(f => f.Data >= primeiroDiaMesAtual && f.Data < primeiroDiaProximoMes); // fornecedores cadastrados neste mês

            ViewBag.QtdFornecedoresUltimoMes = qtdFornecedoresMonth;

            // Inicializa as contagens de estrelas fora do loop
            int fornecedores5Estrelas = 0;
            int fornecedores4Estrelas = 0;
            int fornecedores3Estrelas = 0;
            int fornecedores2Estrelas = 0;
            int fornecedores1Estrela = 0;

            foreach (var fornecedor in _context.Fornecedor.ToList())
            {
                // Filtra as avaliações apenas para o fornecedor atual
                var avaliacoesFornecedor = _context.Avaliar.Where(a => a.FornecedorId == fornecedor.ID).ToList();

                double media = 0;
                double mediaRound = 0;

                if (avaliacoesFornecedor.Any())
                {
                    media = avaliacoesFornecedor.Average(a => a.Nota);
                    mediaRound = Math.Round(media, 2);

                    if (mediaRound >= 4.5)
                    {
                        fornecedores5Estrelas++;
                    }
                    else if (mediaRound >= 3.5)
                    {
                        fornecedores4Estrelas++;
                    }
                    else if (mediaRound >= 2.5)
                    {
                        fornecedores3Estrelas++;
                    }
                    else if (mediaRound >= 1.5)
                    {
                        fornecedores2Estrelas++;
                    }
                    else
                    {
                        fornecedores1Estrela++;
                    }
                }
            }

            // Define as ViewBag com as contagens de estrelas
            ViewBag.Forn1Estrela = fornecedores1Estrela;
            ViewBag.Forn2Estrelas = fornecedores2Estrelas;
            ViewBag.Forn3Estrelas = fornecedores3Estrelas;
            ViewBag.Forn4Estrelas = fornecedores4Estrelas;
            ViewBag.Forn5Estrelas = fornecedores5Estrelas;

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