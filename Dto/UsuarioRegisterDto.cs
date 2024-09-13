using System.ComponentModel.DataAnnotations;

namespace EmprestimoLivros.Dto
{
    public class UsuarioRegisterDto
    {
        [Required(ErrorMessage = "Digite o nome!")]
        public required string Nome { get; set; }

        [Required(ErrorMessage = "Digite o sobrenome!")]
        public required string Sobrenome { get; set; }

        [Required(ErrorMessage = "Digite o email!")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Digite a senha!")]
        public required string Senha { get; set; }

        [Required(ErrorMessage = "Digite a confirmação da senha!"),
            Compare("Senha", ErrorMessage = "As senhas não estão iguais")]
        public required string ConfirmaSenha { get; set; }

    }
}
