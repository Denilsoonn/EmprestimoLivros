using EmprestimoLivros.Data;
using EmprestimoLivros.Dto;
using EmprestimoLivros.Models;
using EmprestimoLivros.Services.SenhaService;
using EmprestimoLivros.Services.SessaoService;

namespace EmprestimoLivros.Services.LoginServices
{
    public class LoginService : ILoginInterface
    {
        private readonly ApplicationDbContext _context;
        private readonly ISenhaInterface _senhaInterface;
        private readonly ISessaoInterface _sessaoInterface;

        public LoginService(ApplicationDbContext context,
                            ISenhaInterface senhaInterface,
                            ISessaoInterface sessaoInterface)
        {
            _context = context;
            _senhaInterface = senhaInterface;
            _sessaoInterface = sessaoInterface;
        }


        public async Task<ResponseModel<UsuarioModel>> Login(UsuarioLoginDto usuarioLoginDto)
        {
            ResponseModel<UsuarioModel> response = new ResponseModel<UsuarioModel>();

            try
            {
                var usuario = _context.Usuarios.FirstOrDefault(x => x.Email == usuarioLoginDto.Email);
                if (usuario == null)
                {
                    response.Mensagem = "Email inválido!";
                    response.Status = false;
                    return response;                
                }

                if (!_senhaInterface.VerificaSenha(usuarioLoginDto.Senha, usuario.SenhaHash, usuario.SenhaSalt))
                {
                    response.Mensagem = "Senha inválida!";
                    response.Status = false;
                    return response;
                }

                //Criar sessão

                _sessaoInterface.CriarSessao(usuario);


                response.Mensagem = "Usuário logado com sucesso!";

                return response;

            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                response.Status = false;
                return response;
            }
        }

        public async Task<ResponseModel<UsuarioModel>> RegistrarUsuario(UsuarioRegisterDto usuarioregisterDto)
        {

            ResponseModel<UsuarioModel> response = new ResponseModel<UsuarioModel>();


            try
            {
                if (VerificarSeEmailExiste(usuarioregisterDto))
                {
                    response.Mensagem = "Email já cadastrado!";
                    response.Status = false;
                    return response;
                }

                _senhaInterface.CriarSenhaHash(usuarioregisterDto.Senha, out byte[] senhaHash, out byte[] senhaSalt);

                var usuario = new UsuarioModel()
                {
                    Nome = usuarioregisterDto.Nome,
                    Sobrenome = usuarioregisterDto.Sobrenome,
                    Email = usuarioregisterDto.Email,
                    SenhaHash = senhaHash,
                    SenhaSalt = senhaSalt
                };

                _context.Add(usuario);
                await _context.SaveChangesAsync();

                response.Mensagem = "Usuário cadastrado com sucesso!";

                return response;

            }
            catch (Exception ex)
            { 
                response.Mensagem = ex.Message;
                response.Status = false;
                return response;
            }
        }

        private bool VerificarSeEmailExiste(UsuarioRegisterDto usuarioRegisterDto)
        {
            var usuario = _context.Usuarios.FirstOrDefault(x => x.Email == usuarioRegisterDto.Email);

            if (usuario == null)
            {
                return false;
            }
            return true;
        }
    }
}
