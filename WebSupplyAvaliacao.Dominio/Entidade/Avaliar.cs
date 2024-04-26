using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebSupplyAvaliacao.Dominio.Entidade;

public class Avaliar
{
    [Key]
    public int ID { get; set; }
    
    public DateTime Data { get; set; } = DateTime.Now;

    [Required]
    public int Nota { get; set; }

    [Required]
    public string Detalhes { get; set; }

    [ForeignKey("Usuario")]
    public int? UsuarioId { get; set; }

    public virtual Usuario? Usuario { get; set; }

    [ForeignKey("Fornecedor")]
    public int? FornecedorId { get; set; }

    public virtual Fornecedor? Fornecedor { get; set; }

    [ForeignKey("ServicoAvaliado")]
    public int? ServicoAvaliadoId { get; set; }

    public virtual ServicoAvaliado? ServicoAvaliado { get; set; }
}
