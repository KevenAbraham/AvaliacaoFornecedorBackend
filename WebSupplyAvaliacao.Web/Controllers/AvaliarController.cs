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

    /// <summary>
    /// Exibe uma lista de fornecedores para avaliação.
    /// </summary>
    /// <returns>Uma IActionResult representando a exibição da lista de fornecedores.</returns>
    public IActionResult ListaAvaliar()
    {
        // Obtém uma lista de fornecedores ordenada por ID em ordem decrescente.
        var forn = _context.Fornecedor.OrderByDescending(x => x.ID).ToList();

        // Retorna a lista de fornecedores para a view correspondente.
        return View(forn);
    }

    /// <summary>
    /// Exibe a página de avaliação para um fornecedor específico.
    /// </summary>
    /// <param name="fornecedorId">O ID do fornecedor a ser avaliado.</param>
    /// <returns>Uma IActionResult representando a exibição da página de avaliação.</returns>
    public IActionResult AvaliarFornecedor(int fornecedorId)
    {
        // Obtém o nome do usuário atualmente logado.
        var userIdentity = User.Identity?.Name;

        // Obtém o ID do usuário atualmente logado.
        var userID = _context.Usuario.FirstOrDefault(x => x.Email == userIdentity)?.ID;

        // Obtém o nome do usuário com base no ID.
        var userName = _context.Usuario.Where(x => x.ID == userID).Select(x => x.Nome).FirstOrDefault();

        // Obtém as informações do fornecedor com base no ID fornecido.
        var fornID = _context.Fornecedor.FirstOrDefault(x => x.ID == fornecedorId);

        // Define as informações a serem passadas para a view.
        ViewBag.NomeUsuario = userName;
        ViewBag.FornecedorID = fornID;

        // Obtém o fornecedor e os serviços avaliados relacionados.
        var fornecedor = _context.Fornecedor.FirstOrDefault(f => f.ID == fornecedorId);
        var servicosAvaliados = _context.ServicoAvaliado.ToList();

        // Cria uma instância do ViewModel para passar para a view.
        var viewModel = new AvaliarFornecedorViewModel
        {
            Fornecedores = fornecedor,
            ServicosAvaliados = servicosAvaliados
        };

        // Retorna a página de avaliação com os dados necessários.
        return View(viewModel);
    }

    /// <summary>
    /// Avalia um fornecedor com base nos dados fornecidos pelo usuário.
    /// </summary>
    /// <param name="fornecedorId">O ID do fornecedor a ser avaliado.</param>
    /// <param name="servicoAvaliadoId">O ID do serviço avaliado.</param>
    /// <param name="avaliacao">Um objeto Avaliar contendo os detalhes da avaliação.</param>
    /// <returns>Uma IActionResult representando o resultado da avaliação.</returns>
    [HttpPost]
    public IActionResult AvaliarFornecedor(int fornecedorId, int servicoAvaliadoId, Avaliar avaliacao)
    {
        // Verifica se os campos obrigatórios foram preenchidos
        if (servicoAvaliadoId == 0 || avaliacao.Detalhes == null || avaliacao.Nota == 0)
        {
            // Define uma mensagem temporária para exibir na view
            TempData["NullMessage"] = "Por favor, preencha todos os campos.";

            // Obtém as informações necessárias para preencher a view
            var fornecedor = _context.Fornecedor.FirstOrDefault(f => f.ID == fornecedorId);
            var servicosAvaliados = _context.ServicoAvaliado.ToList();
            var userIdentity = User.Identity?.Name;
            var userID = _context.Usuario.FirstOrDefault(x => x.Email == userIdentity)?.ID;
            var userName = _context.Usuario.Where(x => x.ID == userID).Select(x => x.Nome).FirstOrDefault();
            var fornID = _context.Fornecedor.FirstOrDefault(x => x.ID == fornecedorId);

            // Define as informações a serem passadas para a view
            ViewBag.NomeUsuario = userName;
            ViewBag.FornecedorID = fornID;

            // Cria uma instância do ViewModel para passar para a view
            var viewModel = new AvaliarFornecedorViewModel
            {
                Fornecedores = fornecedor,
                ServicosAvaliados = servicosAvaliados
            };

            // Retorna a página de avaliação com os dados necessários
            return View("AvaliarFornecedor", viewModel);
        }

        // Verifica se o modelo é válido
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


    //Tela de conclusão de avaliação.
    public IActionResult ConclusaoAvaliacao()
    {
        return View();
    }

    /// <summary>
    /// Retorna uma lista de fornecedores avaliados, juntamente com informações sobre suas avaliações.
    /// </summary>
    /// <param name="filtroData">Opcional. Uma data para filtrar as avaliações por data.</param>
    /// <returns>Uma IActionResult representando a lista de fornecedores avaliados.</returns>
    public IActionResult ListaAvaliados([FromQuery] DateTime? dataInicio, [FromQuery] DateTime? dataFinal)
    {
        // Obtém a lista de todos os fornecedores
        var fornecedores = _context.Fornecedor.ToList();

        // Obtém o mês e o ano atual
        int mesAtual = DateTime.Now.Month;
        int anoAtual = DateTime.Now.Year;

        // Define as datas de início e final para o mês atual
        DateTime dataInicioMes = new DateTime(anoAtual, mesAtual, 1);
        DateTime dataFinalMes = DateTime.Now;


        //Se não foram fornecidas datas de início e final, definimos para o mês atual
        if (dataInicio == null && dataFinal == null)
        {
            dataInicio = dataInicioMes;
            dataFinal = dataFinalMes;
        }

        ViewBag.MesAtual = dataInicio?.ToString("yyyy-MM-dd") ?? dataInicioMes.ToString("yyyy-MM-dd");
        ViewBag.AnoAtual = dataFinal?.ToString("yyyy-MM-dd") ?? dataFinalMes.ToString("yyyy-MM-dd");

        // Inicializa uma lista de ViewModel para armazenar informações sobre as avaliações
        var viewModel = new List<MediaAvaliacaoViewModel>();

        // Inicializa as contagens de estrelas fora do loop
        int fornecedores5Estrelas = 0;
        int fornecedores4Estrelas = 0;
        int fornecedores3Estrelas = 0;
        int fornecedores2Estrelas = 0;
        int fornecedores1Estrela = 0;
        int totalFornecedores = 0;

        // Loop através de todos os fornecedores
        foreach (var fornecedor in fornecedores)
        {
            // Filtra as avaliações apenas para o fornecedor atual
            var avaliacoesFornecedor = _context.Avaliar
            .Where(x => x.FornecedorId == fornecedor.ID && x.Data >= dataInicio && x.Data <= dataFinal)
            .ToList();

            // Calcula a média das avaliações para o fornecedor atual
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
                continue; // Pula para o próximo fornecedor se não houver avaliações
            }

            media = avaliacoesFornecedor.Average(a => a.Nota);
            mediaRound = Math.Round(media, 2);

            // Incrementa a contagem de fornecedores com base na média de avaliação
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

            //Quantidade total de fornecedores
            totalFornecedores++;

            // Adiciona as informações do fornecedor atual ao ViewModel
            viewModel.Add(new MediaAvaliacaoViewModel
            {
                Fornecedores = new List<Fornecedor> { fornecedor }, // Define apenas o fornecedor atual
                MediaAvaliacao = mediaRound
            });
        }

        // Define o número total de fornecedores como uma ViewBag para acesso na View
        ViewBag.QtdFornecedor = totalFornecedores;

        // Define as ViewBag fora do loop
        ViewBag.Forn1Estrela = fornecedores1Estrela;
        ViewBag.Forn2Estrelas = fornecedores2Estrelas;
        ViewBag.Forn3Estrelas = fornecedores3Estrelas;
        ViewBag.Forn4Estrelas = fornecedores4Estrelas;
        ViewBag.Forn5Estrelas = fornecedores5Estrelas;

        return View(viewModel); // Passa a lista de MediaAvaliacaoViewModel para a View
    }

    
    /// <summary>
    /// Retorna o histórico de avaliações de um fornecedor específico.
    /// </summary>
    /// <param name="id">O ID do fornecedor para o qual o histórico de avaliações será recuperado.</param>
    /// <returns>Uma IActionResult representando o histórico de avaliações do fornecedor.</returns>
    [HttpGet("Avaliar/HistoricoAvaliacao/{id}")]
    public IActionResult HistoricoAvaliacao(int id)
    {
        // Obtém o fornecedor com base no ID fornecido
        var fornecedor = _context.Fornecedor.FirstOrDefault(x => x.ID == id);

        // Define o nome do fornecedor e o ID como ViewBag para acesso na View
        ViewBag.NomeFornecedor = fornecedor.NomeFantasia;
        ViewBag.FornID = fornecedor.ID;

        // Obtém as avaliações associadas ao fornecedor, incluindo informações sobre o serviço avaliado e o usuário que fez a avaliação
        var avaliacoes = _context.Avaliar
            .Include(a => a.ServicoAvaliado)
            .Include(a => a.Usuario)
            .Where(a => a.FornecedorId == id).ToList();

        // Retorna a View com a lista de avaliações
        return View(avaliacoes);
    }

    /// <summary>
    /// Retorna os detalhes de uma avaliação específica com base em seu ID.
    /// </summary>
    /// <param name="id">O ID da avaliação a ser visualizada.</param>
    /// <returns>Uma IActionResult representando os detalhes da avaliação.</returns>
    public IActionResult VisualizarAvaliacao(int id)
    {
        // Obtém os detalhes da avaliação com base no ID fornecido, incluindo informações sobre o serviço avaliado e o usuário que fez a avaliação
        var avaliacoes = _context.Avaliar
            .Include(a => a.ServicoAvaliado)
            .Include(a => a.Usuario)
            .SingleOrDefault(a => a.ID == id);

        // Retorna a View com os detalhes da avaliação
        return View(avaliacoes);
    }
}
