using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebSupplyAvaliacao.Dados.Context;
using WebSupplyAvaliacao.Dominio.Entidade;
using WebSupplyAvaliacao.Web.Models;

namespace WebSupplyAvaliacao.Web.Controllers;

// Controlador responsável por lidar com as ações relacionadas aos usuários do sistema.
public class UsuarioController : Controller
{
    private readonly AppDbContext _context; // Contexto do banco de dados para interação com as entidades de usuário.
    private readonly Validacao _validacao; // Serviço de validação utilizado por este controlador.

    // Construtor do controlador, injetando o contexto do banco de dados e o serviço de validação.
    public UsuarioController(AppDbContext context, Validacao validacao)
    {
        _context = context;
        _validacao = validacao;
    }

    [Authorize]
    [ServiceFilter(typeof(Validacao))]
    public IActionResult Cadastrar() // Ação para exibir a página de cadastro de usuário.
    {
        return View();
    }

    [HttpPost]
    [Authorize]
    [ServiceFilter(typeof(Validacao))]
    public IActionResult Cadastrar(Usuario usuario) // Ação para cadastrar um novo usuário.
    {
        // Verifica se o modelo de dados é válido antes de prosseguir.
        if (ModelState.IsValid)
        {
            // Verifica se o email do usuário já está cadastrado no banco de dados.
            var emailExistente = _context.Usuario.Any(x => x.Email == usuario.Email);

            // Condição para exibir mensagem de erro caso o e-mail já esteja cadastrado. 
            if (emailExistente)
            {
                TempData["ErrorMessage"] = "Este email já está cadastrado.";
                return RedirectToAction("Cadastrar");
            }

            // Adiciona o novo usuário ao contexto do banco de dados e salva as alterações.
            _context.Usuario.Add(usuario);
            _context.SaveChanges();
            return RedirectToAction("Confirmacao", "Usuario");
        }
        return View(usuario);
    }

    [Authorize]
    [ServiceFilter(typeof(Validacao))]
    // Ação para exibir a página de confirmação de cadastro.
    public IActionResult Confirmacao()
    {
        return View();
    }

    [Authorize]
    [ServiceFilter(typeof(Validacao))]
    // Ação para listar todos os usuários cadastrados.
    public IActionResult Listar()
    {
        // Recupera todos os usuários cadastrados, ordenados por ID, e passa para a view.
        var usuarios = _context.Usuario.OrderByDescending(x => x.ID).ToList(); // Este código pode ser substituído pelo `ordering: true` na configuração do DataTables.
        return View(usuarios);
    }

    [Authorize]
    [ServiceFilter(typeof(Validacao))]
    // Ação para exibir a página de edição de um usuário específico.
    public IActionResult Editar(int id)
    {
        // Busca o usuário pelo ID e o passa para a view de edição.
        var usuario = _context.Usuario.SingleOrDefault(user => user.ID == id);
        return View(usuario);
    }

    [HttpPost]
    [Authorize]
    [ServiceFilter(typeof(Validacao))]
    // Ação para editar as informações de um usuário.
    [HttpPost]
    public IActionResult Editar(int id, Usuario usuario)
    {
        // Verifica se o email do usuário já está cadastrado para outro usuário.
        var emailExiste = _context.Usuario.FirstOrDefault(x => x.Email == usuario.Email && x.ID != id);

        // Verifica se o usuário não está tentando inserir valores nulos nos campos.
        if (usuario.Nome == null || usuario.Email == null)
        {
            TempData["ErrorMessage"] = "Os campos não podem ser nulos.";
            return RedirectToAction("Editar");
        }

        // Verifica se as informações do usuário não foram alteradas.
        var checkUser = _context.Usuario.SingleOrDefault(x => x.ID == id);
        if (usuario.Nome == checkUser.Nome && usuario.Email == checkUser.Email && usuario.Status == checkUser.Status)
        {
            TempData["AlertMessage"] = "Essas informações já estão cadastradas.";
            return RedirectToAction("Editar");
        }

        // Caso o email já esteja cadastrado para outro usuário, exibe mensagem de erro.
        if (emailExiste != null)
        {
            TempData["ErrorMessage"] = "Este email já está cadastrado.";
            return RedirectToAction("Editar");
        }
        // Caso contrário, atualiza as informações do usuário no banco de dados e exibe mensagem de sucesso.
        else if (ModelState.IsValid)
        {
            _context.Entry(checkUser).State = EntityState.Detached;
            _context.Usuario.Update(usuario);
            _context.SaveChanges();
            TempData["InfoMessage"] = "Usuário alterado com sucesso.";
            return RedirectToAction("Editar");
        }
        return View(usuario);
    }

    // Ação para exibir a página de acesso negado.
    public IActionResult Negado()
    {
        return View();
    }
}
