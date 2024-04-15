using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebSupplyAvaliacao.Dominio.Entidade;

public class Fornecedor
{
    [Key]
    public int ID { get; set; }

    [Required(ErrorMessage = "O campo é obrigatório.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome fantasia é inválido")]
    public string NomeFantasia { get; set; }

    [Required(ErrorMessage = "O campo é obrigatório.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O Nome para Contato é inválido.")]
    public string NomeContato { get; set; }

    [Required(ErrorMessage = "O campo é obrigatório.")]
    [StringLength(100, ErrorMessage = "O E-mail é inválido")]
    [EmailAddress(ErrorMessage = "Informe um email válido.")]
    [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "O e-mail é inválido.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "O campo é obrigatório.")]
    [RegularExpression(@"^[0-9]{14}$", ErrorMessage = "O CNPJ é inválido.")]
    [StringLength(30, MinimumLength = 14, ErrorMessage = "O CNPJ é inválido.")]
    public string CNPJ { get; set; }

    public bool Status { get; set; } = true;

    [Required(ErrorMessage = "O campo é obrigatório.")]
    [StringLength(30, MinimumLength = 9, ErrorMessage = "O telefoneX é inválido.")]
    public string Telefone { get; set; }

    [Required(ErrorMessage = "O campo é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O endereço é inválido.")]
    public string Endereco { get; set; }

    [Required(ErrorMessage = "O campo é obrigatório.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O complemento é inválido.")]
    public string Complemento { get; set; }

    [Required(ErrorMessage = "O campo é obrigatório.")]
    [StringLength(10, MinimumLength = 2, ErrorMessage = "O número é inválido.")]
    public string Numero { get; set; }

    [Required(ErrorMessage = "O campo é obrigatório.")]
    [StringLength(20, ErrorMessage = "O CEP é inválido.")]
    public string CEP { get; set; }

    [Required(ErrorMessage = "O campo é obrigatório.")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "O Bairro é inválido.")]
    public string Bairro { get; set; } 

    [Required(ErrorMessage = "O campo é obrigatório.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Cidade é inválida.")]
    public string Cidade { get; set; }

    [Required(ErrorMessage = "O campo é obrigatório.")]
    public string UF { get; set; }

    public DateTime Data { get; set; } = DateTime.Now;

    [ForeignKey("Usuario")]
    public int? UsuarioId { get; set; }

    public virtual Usuario? Usuario { get; set; }

    public ICollection<Especializacao>? Especializacoes { get; set; }
    public ICollection<Documento>? Documentos { get; set; }
}
