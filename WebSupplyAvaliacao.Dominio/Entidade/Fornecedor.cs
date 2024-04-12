using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebSupplyAvaliacao.Dominio.Entidade;

public class Fornecedor
{
    [Key]
    public int ID { get; set; }

    [Required(ErrorMessage = "O campo é obrigatório.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O Nome Fantasia deve ter entre {2} e {1} caracteres.")]
    public string NomeFantasia { get; set; }

    [Required(ErrorMessage = "O campo é obrigatório.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O Nome para Contato deve ter entre {2} e {1} caracteres.")]
    public string NomeContato { get; set; }

    [Required(ErrorMessage = "O campo é obrigatório.")]
    [StringLength(100, ErrorMessage = "O E-mail deve ter menos de {1} caracteres.")]
    [EmailAddress(ErrorMessage = "Informe um email válido.")]
    [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "O e-mail precisa ser válido.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "O campo é obrigatório.")]
    [StringLength(20, ErrorMessage = "O CNPJ deve ter {1} caracteres.")]
    public string CNPJ { get; set; }

    public bool Status { get; set; } = true;

    [Required(ErrorMessage = "O campo é obrigatório.")]
    [StringLength(20, ErrorMessage = "O número de telefone deve conter menos de {1} caracteres.")]
    public string Telefone { get; set; }

    [Required(ErrorMessage = "O campo é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O endereço deve ter entre {2} e {1} caracteres.")]
    public string Endereco { get; set; }

    [Required(ErrorMessage = "O campo é obrigatório.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O complemento deve ter entre {2} e {1} caracteres.")]
    public string Complemento { get; set; }

    [Required(ErrorMessage = "O campo é obrigatório.")]
    [StringLength(10, ErrorMessage = "O numero deve ter conter menos de {1} caracteres.")]
    public string Numero { get; set; }

    [Required(ErrorMessage = "O campo é obrigatório.")]
    [StringLength(20, ErrorMessage = "O CEP deve ter menos de {1} caracteres.")]
    public string CEP { get; set; }

    [Required(ErrorMessage = "O campo é obrigatório.")]
    [StringLength(100, ErrorMessage = "O Bairro deve ter menos de {1} caracteres.")]
    public string Bairro { get; set; } 

    [Required(ErrorMessage = "O campo é obrigatório.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Cidade deve ter entre {2} e {1} caracteres.")]
    public string Cidade { get; set; }

    [Required(ErrorMessage = "O campo é obrigatório.")]
    public string UF { get; set; }

    public DateTime Data { get; set; } = DateTime.Now;

    [ForeignKey("Usuario")]
    public int? UsuarioId { get; set; }

    public virtual Usuario? Usuario { get; set; }

    public ICollection<Especializacao>? Especializacoes { get; set; }
}
