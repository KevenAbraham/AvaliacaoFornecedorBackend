using System.ComponentModel.DataAnnotations;

namespace WebSupplyAvaliacao.Dominio.Entidade;

public class Usuario
{
    [Key]
    public int ID { get; set; }

    [Required(ErrorMessage = "O nome já foi escolhido")]
    [StringLength(80)]
    [Display(Name = "Nome:")]
    public string Nome { get; set; }

    [EmailAddress(ErrorMessage = "Informe um email válido")]
    [StringLength(100)]
    [Display(Name = "E-mail:")]
    public string Email { get; set; }
    public bool Status { get; set; }

    public Usuario()
    {
        Status = true;
    }
}