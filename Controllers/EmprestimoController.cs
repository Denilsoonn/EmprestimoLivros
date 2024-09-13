using ClosedXML.Excel;
using EmprestimoLivros.Data;
using EmprestimoLivros.Models;
using EmprestimoLivros.Services.SessaoService;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;

namespace EmprestimoLivros.Controllers
{
    public class EmprestimoController : Controller
    {

        readonly private ApplicationDbContext _db;
        readonly private ISessaoInterface _sessaoInterface;

        public EmprestimoController(ApplicationDbContext db, ISessaoInterface sessaoInterface)
        {
            _db = db;
            _sessaoInterface = sessaoInterface;
        }

        public IActionResult Index()
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Login", "Login");
            }

            IEnumerable<EmprestimosModel> emprestimos = _db.Emprestimos;

            return View(emprestimos);
        }

        [HttpGet]
        public IActionResult Cadastrar()
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Login", "Login");
            }

            return View();
        }

        [HttpGet]
        //Pegar os dados selecionados para editar
        public IActionResult Editar(int? id)
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Login", "Login");
            }

            if (id == null || id == 0)
            {
                return NotFound();
            }

            EmprestimosModel emprestimo = _db.Emprestimos.FirstOrDefault(x => x.Id == id);

            if (emprestimo == null)
            {
                return NotFound();
            }

            return View(emprestimo);
        }

        [HttpGet]

        public IActionResult Excluir(int? id)
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Login", "Login");
            }

            if (id == null || id == 0)
            {
                return NotFound();
            }

            EmprestimosModel emprestimo = _db.Emprestimos.FirstOrDefault(x => x.Id == id);

            if(emprestimo == null) 
            {
                return NotFound();
            }

            return View(emprestimo);

        }

        [HttpGet]

        public IActionResult Exportar() 
        {
            var dados = GetDados();

            using (XLWorkbook workbook = new XLWorkbook()) 
            {
                workbook.AddWorksheet(dados, "Dados Emprestimo");

                using (MemoryStream ms = new MemoryStream())
                {
                    workbook.SaveAs(ms);
                    return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spredsheethtml.sheet", "Emprestimo.Xls");
                }
            }
        }

        private DataTable GetDados() 
        {
            DataTable dataTable = new DataTable();

            dataTable.TableName = "Dados empréstimos";

            dataTable.Columns.Add("Id", typeof(int));
            dataTable.Columns.Add("Recebedor", typeof(string));
            dataTable.Columns.Add("Fornecedor", typeof(string));
            dataTable.Columns.Add("Livro", typeof(string));
            dataTable.Columns.Add("dataUltimaAtualizacao", typeof(DateTime));

            var dados = _db.Emprestimos.ToList();

            if (dados.Count > 0)
            {
                dados.ForEach(emprestimo =>
                {
                    dataTable.Rows.Add(emprestimo.Id, emprestimo.Recebedor, emprestimo.Fornecedor, emprestimo.LivroEmprestado, emprestimo.dataUltimaAtualizacao);
                    
                });
            }

            return dataTable;   
        }

        //Registrar os dados no banco
        [HttpPost]
        public IActionResult Cadastrar(EmprestimosModel emprestimo)
        {
            if (ModelState.IsValid)
            {
                emprestimo.dataUltimaAtualizacao = DateTime.Now;


                _db.Emprestimos.Add(emprestimo);
                _db.SaveChanges();

                TempData["MensagemSucesso"] = "Cadastro realizado com sucesso";

                return RedirectToAction("Index");
            }

            //TempData["MensagemErro"] = "Algum erro ocorreu ao realizar a edição";

            return View();
        }

        [HttpPost]
        public IActionResult Editar(EmprestimosModel emprestimo) 
        { 
            if (ModelState.IsValid)
            {
                var emprestimoDB = _db.Emprestimos.Find(emprestimo.Id);


                emprestimoDB.LivroEmprestado = emprestimo.LivroEmprestado;
                emprestimoDB.Recebedor = emprestimo.Recebedor;
                emprestimoDB.Fornecedor = emprestimo.Fornecedor;

                // Isso dá algum erro
                _db.Emprestimos.Update(emprestimoDB);
                _db.SaveChanges();

                TempData["MensagemSucesso"] = "Edição realizada com sucesso";

                return RedirectToAction("Index");
            }
            
            TempData["MensagemErro"] = "Ocorreu algum erro no momento da edição!";
            return View(emprestimo); 

        }

        [HttpPost]

        public IActionResult Excluir(EmprestimosModel emprestimo)

        {
            if(emprestimo == null) 
            {
                return NotFound();
            }

            _db.Emprestimos.Remove(emprestimo);
            _db.SaveChanges();


            TempData["MensagemSucesso"] = "Exclusão realizada com sucesso";

            return RedirectToAction("Index");

        }
    }
}
