using System.ComponentModel.DataAnnotations;

namespace WebSupplyAvaliacao.Dominio.Entidade;

public class Usuario
{
    [Key]
    public int ID { get; set; }
    
    [StringLength(80)]
    public string Nome { get; set; }

    [StringLength(100)]
    public string Email { get; set; }
    public bool Status { get; set; }
}
