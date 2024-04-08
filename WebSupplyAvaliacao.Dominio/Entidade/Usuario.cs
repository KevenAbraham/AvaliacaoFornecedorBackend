using System.ComponentModel.DataAnnotations;

namespace WebSupplyAvaliacao.Dominio.Entidade;

public class Usuario
{
    [Key]
    public int ID { get; set; }

    [Required(ErrorMessage = "Nome é obrigatório.")]
    [StringLength(80)]
    [Display(Name = "Nome:")]
    public string Nome { get; set; }

    [EmailAddress(ErrorMessage = "Informe um email válido.")]
    [Required(ErrorMessage = "O E-mail é obrigatório.")]
    [StringLength(100)]
    [Display(Name = "E-mail:")]
    [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "O e-mail precisa ser válido.")] 
    public string Email { get; set; }
    public bool Status { get; set; }

    public Usuario()
    {
        Status = true;
    }
}