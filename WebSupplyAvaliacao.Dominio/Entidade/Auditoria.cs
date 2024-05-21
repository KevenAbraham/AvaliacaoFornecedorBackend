using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebSupplyAvaliacao.Dominio.Entidade;

public class Auditoria
{
    [Key]
    public int ID { get; set; }

    public DateTime Data { get; set; } = DateTime.Now;

    [Required]
    public int? Chave { get; set; }

    [ForeignKey("Usuario")]
    public int? UsuarioId { get; set; }

    [ForeignKey("Acao")]
    public int? AcaoID { get; set; }

    public virtual Usuario? Usuario { get; set; }

    public virtual Acao? Acao { get; set; }
}
