namespace EmprestimoLivros.Models
{
    public class UsuarioModel
    {

        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Sobrenome { get; set; }
        public required string Email { get; set; }
        public required byte[] SenhaHash { get; set; }
        public required byte[] SenhaSalt { get; set; }

    }
}
