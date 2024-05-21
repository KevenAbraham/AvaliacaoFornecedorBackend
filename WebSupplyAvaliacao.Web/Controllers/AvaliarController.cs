using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebSupplyAvaliacao.Dados.Context;
using WebSupplyAvaliacao.Dominio.Entidade;
using WebSupplyAvaliacao.Web.Models;
using WebSupplyAvaliacao.Web.Models.Enum;
using WebSupplyAvaliacao.Web.ViewModel;

namespace WebSupplyAvaliacao.Web.Controllers;

[Authorize]
[ServiceFilter(typeof(Validacao))]
public class AvaliarController : Controller
{
    private readonly AppDbContext _context;
    private readonly AuditoriaService _auditoriaService;

    public AvaliarController(AppDbContext context, AuditoriaService auditoriaService)
    {
        _context = context;
        _auditoriaService = auditoriaService;
    }

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

            avaliacao.UsuarioId = userID;
            avaliacao.FornecedorId = fornecedorId;
            avaliacao.ServicoAvaliadoId = servicoAvaliadoId;

            _context.Avaliar.Add(avaliacao);
            _context.SaveChanges();

            //AcaoEnum idAcao = AcaoEnum.AvaliarFornecedor;
            //_auditoriaService.RegistrarAuditoria(fornecedorId, (int)userID, idAcao);

            return RedirectToAction("ConclusaoAvaliacao", "Avaliar");
        }

        return View();
    }


    public IActionResult ConclusaoAvaliacao()
    {
        return View();
    }

    public IActionResult ListaAvaliados([FromQuery] DateTime? dataInicio, [FromQuery] DateTime? dataFinal)
    {
        var fornecedores = _context.Fornecedor.ToList();

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

        var viewModel = new List<MediaAvaliacaoViewModel>();

        int fornecedores5Estrelas = 0;
        int fornecedores4Estrelas = 0;
        int fornecedores3Estrelas = 0;
        int fornecedores2Estrelas = 0;
        int fornecedores1Estrela = 0;
        int totalFornecedores = 0;

        foreach (var fornecedor in fornecedores)
        {
            var avaliacoesFornecedor = _context.Avaliar
            .Where(x => x.FornecedorId == fornecedor.ID && x.Data >= dataInicio && x.Data <= dataFinal)
            .ToList();

            double media = 0;
            double mediaRound = 0;

            if (dataInicio != null && dataFinal != null)
            {
                dataInicio = dataInicio?.Date;
                dataFinal = dataFinal?.Date.AddDays(1).AddTicks(-1);
                avaliacoesFornecedor = avaliacoesFornecedor.Where(x => x.Data >= dataInicio && x.Data.Date <= dataFinal).ToList();
            }
            
            if (!avaliacoesFornecedor.Any())
            {
                continue; 
            }

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

            totalFornecedores++;

            viewModel.Add(new MediaAvaliacaoViewModel
            {
                Fornecedores = new List<Fornecedor> { fornecedor },
                MediaAvaliacao = mediaRound
            });
        }

        ViewBag.QtdFornecedor = totalFornecedores;

        ViewBag.Forn1Estrela = fornecedores1Estrela;
        ViewBag.Forn2Estrelas = fornecedores2Estrelas;
        ViewBag.Forn3Estrelas = fornecedores3Estrelas;
        ViewBag.Forn4Estrelas = fornecedores4Estrelas;
        ViewBag.Forn5Estrelas = fornecedores5Estrelas;

        return View(viewModel); 
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
