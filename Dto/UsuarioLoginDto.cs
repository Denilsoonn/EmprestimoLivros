using System.ComponentModel.DataAnnotations;

namespace EmprestimoLivros.Dto
{
    public class UsuarioLoginDto
    {
        [Required(ErrorMessage = "Digite o Email!")]
        public required string Email { get; set; }
        [Required(ErrorMessage = "Digite a senha!")]
        public required string Senha { get; set; }
    }
}
