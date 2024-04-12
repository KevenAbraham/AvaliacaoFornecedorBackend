using System.ComponentModel.DataAnnotations;

namespace WebSupplyAvaliacao.Dominio.Entidade;

public class Especializacao
{
    [Key]
    public int ID { get; set; }

    [Required(ErrorMessage = "Precisa definir o tipo de fornecedor")]
    public string Tipo { get; set; }

    public ICollection<Fornecedor>? Fornecedores { get; set; }
}
