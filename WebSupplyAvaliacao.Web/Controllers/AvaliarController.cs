using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebSupplyAvaliacao.Dados.Context;
using WebSupplyAvaliacao.Dominio.Entidade;
using WebSupplyAvaliacao.Web.Models;
using WebSupplyAvaliacao.Web.ViewModel;

namespace WebSupplyAvaliacao.Web.Controllers;

[Authorize]
[ServiceFilter(typeof(Validacao))]
public class AvaliarController : Controller
{
    private readonly AppDbContext _context;

    public AvaliarController(AppDbContext context)
    {
        _context = context;
    }

    //Avaliar o Fornecedor
    public IActionResult ListaAvaliar()
    {
        var forn = _context.Fornecedor.OrderByDescending(x => x.ID).ToList();
        return View(forn);
    }

    public IActionResult AvaliarFornecedor(int fornecedorId)
    {
        var userIdentity = User.Identity?.Name;

        var userID = _context.Usuario.FirstOrDefault(x => x.Email == userIdentity)?.ID;

        var userName = _context.Usuario.Where(x => x.ID == userID).Select(x => x.Nome).FirstOrDefault();

        var fornID = _context.Fornecedor.FirstOrDefault(x => x.ID == fornecedorId);

        ViewBag.NomeUsuario = userName;
        ViewBag.FornecedorID = fornID;

        var fornecedor = _context.Fornecedor.FirstOrDefault(f => f.ID == fornecedorId);
        var servicosAvaliados = _context.ServicoAvaliado.ToList();

        var viewModel = new AvaliarFornecedorViewModel
        {
            Fornecedores = fornecedor,
            ServicosAvaliados = servicosAvaliados
        };
        return View(viewModel);
    }

    [HttpPost]
    public IActionResult AvaliarFornecedor(int fornecedorId, int servicoAvaliadoId, Avaliar avaliacao)
    {
        if (servicoAvaliadoId == 0 || avaliacao.Detalhes == null || avaliacao.Nota == 0)
        {
            TempData["NullMessage"] = "Por favor, preencha todos os campos.";
            var fornecedor = _context.Fornecedor.FirstOrDefault(f => f.ID == fornecedorId);
            var servicosAvaliados = _context.ServicoAvaliado.ToList();

            var userIdentity = User.Identity?.Name;

            var userID = _context.Usuario.FirstOrDefault(x => x.Email == userIdentity)?.ID;

            var userName = _context.Usuario.Where(x => x.ID == userID).Select(x => x.Nome).FirstOrDefault();

            var fornID = _context.Fornecedor.FirstOrDefault(x => x.ID == fornecedorId);

            ViewBag.NomeUsuario = userName;
            ViewBag.FornecedorID = fornID;

            var viewModel = new AvaliarFornecedorViewModel
            {
                Fornecedores = fornecedor,
                ServicosAvaliados = servicosAvaliados
            };

            return View("AvaliarFornecedor", viewModel);
        }


        if (ModelState.IsValid)
        {
            var userIdentity = User.Identity?.Name;
            var userID = _context.Usuario.FirstOrDefault(x => x.Email == userIdentity)?.ID;

            // Preencha as informações da avaliação
            avaliacao.UsuarioId = userID;
            avaliacao.FornecedorId = fornecedorId;
            avaliacao.ServicoAvaliadoId = servicoAvaliadoId;

            // Salve a avaliação no banco de dados
            _context.Avaliar.Add(avaliacao);
            _context.SaveChanges();

            // Redirecione para alguma página após salvar a avaliação
            return RedirectToAction("ConclusaoAvaliacao", "Avaliar");
        }

        return View();
    }

    public IActionResult ConclusaoAvaliacao()
    {
        return View();
    }







    public IActionResult ListaAvaliados()
    {
        var fornecedores = _context.Fornecedor.ToList();
        var viewModel = new List<MediaAvaliacaoViewModel>();

        // Inicializa as contagens de estrelas fora do loop
        int fornecedores5Estrelas = 0;
        int fornecedores4Estrelas = 0;
        int fornecedores3Estrelas = 0;
        int fornecedores2Estrelas = 0;
        int fornecedores1Estrela = 0;

        foreach (var fornecedor in fornecedores)
        {
            // Filtra as avaliações apenas para o fornecedor atual
            var avaliacoesFornecedor = _context.Avaliar.Where(a => a.FornecedorId == fornecedor.ID).ToList();

            var ultimaAvaliacao = _context.Avaliar
                .Where(a => a.FornecedorId == fornecedor.ID)
                .OrderByDescending(a => a.Data)
                .FirstOrDefault();

            // Inicializa a data da última avaliação como null por padrão
            DateTime? dataUltimaAval = null;

            // Se houver uma última avaliação, define a data
            if (ultimaAvaliacao != null)
            {
                dataUltimaAval = ultimaAvaliacao.Data;
            }

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

            viewModel.Add(new MediaAvaliacaoViewModel
            {
                Fornecedores = new List<Fornecedor> { fornecedor }, // Define apenas o fornecedor atual
                DataUltimaAval = dataUltimaAval,
                MediaAvaliacao = mediaRound
            });
        }

        int qtdFornecedores = _context.Fornecedor.Count(); //quantidade total de fornecedores
        ViewBag.QtdFornecedor = qtdFornecedores;

        // Define a data de início do mês atual
        DateTime primeiroDiaMesAtual = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

        // Define a data de início do próximo mês
        DateTime primeiroDiaProximoMes = primeiroDiaMesAtual.AddMonths(1);

        int qtdFornecedoresMonth = _context.Fornecedor
            .Count(f => f.Data >= primeiroDiaMesAtual && f.Data < primeiroDiaProximoMes); // fornecedores cadastrados neste mês

        ViewBag.QtdFornecedoresUltimoMes = qtdFornecedoresMonth;

        // Define as ViewBag fora do loop
        ViewBag.Forn1Estrela = fornecedores1Estrela;
        ViewBag.Forn2Estrelas = fornecedores2Estrelas;
        ViewBag.Forn3Estrelas = fornecedores3Estrelas;
        ViewBag.Forn4Estrelas = fornecedores4Estrelas;
        ViewBag.Forn5Estrelas = fornecedores5Estrelas;

        return View(viewModel); // Passa a lista de MediaAvaliacaoViewModel para a View
    }


    [HttpGet("Avaliar/HistoricoAvaliacao/{id}")]
    public IActionResult HistoricoAvaliacao(int id)
    {
        var fornecedor = _context.Fornecedor.FirstOrDefault(x => x.ID == id);

        ViewBag.NomeFornecedor = fornecedor.NomeFantasia;
        ViewBag.FornID = fornecedor.ID;

        var avaliacoes = _context.Avaliar
            .Include(a => a.ServicoAvaliado)
            .Include(a => a.Usuario)
            .Where(a => a.FornecedorId == id).ToList();

        return View(avaliacoes);
    }


    public IActionResult VisualizarAvaliacao(int id)
    {
        var avaliacoes = _context.Avaliar
            .Include(a => a.ServicoAvaliado)
            .Include(a => a.Usuario)
            .SingleOrDefault(a => a.ID == id);

        return View(avaliacoes);
    }
}
