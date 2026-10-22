using GerenciadorAcademico.Services;
using GerenciadorAcademico.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GerenciadorAcademico.Controllers;

public class ProjetosController : Controller
{
    private readonly IProjetoService _projetoService;

    public ProjetosController(IProjetoService projetoService)
    {
        _projetoService = projetoService;
    }

    [HttpGet]
    public IActionResult Index(string? textoPesquisa, string? ordenarPor)
    {
        var projetos = _projetoService.PesquisarPorTitulo(textoPesquisa);

        projetos = ordenarPor switch
        {
            "titulo_desc" => projetos.OrderByDescending(p => p.Titulo, StringComparer.CurrentCultureIgnoreCase).ToList(),
            "carga" => projetos.OrderBy(p => p.CargaHoraria).ToList(),
            "carga_desc" => projetos.OrderByDescending(p => p.CargaHoraria).ToList(),
            "professor" => projetos.OrderBy(p => p.Professor?.Nome, StringComparer.CurrentCultureIgnoreCase).ToList(),
            "professor_desc" => projetos.OrderByDescending(p => p.Professor?.Nome, StringComparer.CurrentCultureIgnoreCase).ToList(),
            _ => projetos.OrderBy(p => p.Titulo, StringComparer.CurrentCultureIgnoreCase).ToList()
        };

        var model = new ProjetosIndexViewModel
        {
            Projetos = projetos,
            TextoPesquisa = textoPesquisa,
            OrdenarPor = ordenarPor,
            QuantidadeTotal = _projetoService.Listar().Count
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult Detalhes(int id)
    {
        var projeto = _projetoService.ObterPorId(id);
        return projeto is null ? NotFound() : View(projeto);
    }

    [HttpGet]
    public IActionResult Cadastrar()
    {
        var model = new NovoProjetoViewModel
        {
            Professores = ObterProfessoresSelectList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Cadastrar(NovoProjetoViewModel model)
    {
        ValidarProfessor(model.ProfessorId);

        if (!ModelState.IsValid)
        {
            model.Professores = ObterProfessoresSelectList();
            return View(model);
        }

        _projetoService.Adicionar(model);
        TempData["Mensagem"] = "Projeto cadastrado com sucesso!";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Editar(int id)
    {
        var projeto = _projetoService.ObterPorId(id);
        if (projeto is null)
            return NotFound();

        var model = new EditarProjetoViewModel
        {
            Id = projeto.Id,
            Titulo = projeto.Titulo,
            Descricao = projeto.Descricao,
            CargaHoraria = projeto.CargaHoraria,
            ProfessorId = projeto.ProfessorId,
            Professores = ObterProfessoresSelectList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Editar(EditarProjetoViewModel model)
    {
        if (_projetoService.ObterPorId(model.Id) is null)
            return NotFound();

        ValidarProfessor(model.ProfessorId);

        if (!ModelState.IsValid)
        {
            model.Professores = ObterProfessoresSelectList();
            return View(model);
        }

        if (!_projetoService.Atualizar(model))
            return NotFound();

        TempData["Mensagem"] = "Projeto atualizado com sucesso!";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Excluir(int id)
    {
        var projeto = _projetoService.ObterPorId(id);
        return projeto is null ? NotFound() : View(projeto);
    }

    [HttpPost, ActionName("Excluir")]
    [ValidateAntiForgeryToken]
    public IActionResult ExcluirConfirmado(int id)
    {
        if (!_projetoService.Excluir(id))
            return NotFound();

        TempData["Mensagem"] = "Projeto excluído com sucesso!";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Erro()
    {
        return View();
    }

    private List<SelectListItem> ObterProfessoresSelectList()
    {
        return _projetoService.ListarProfessores()
            .OrderBy(professor => professor.Nome)
            .Select(professor => new SelectListItem
            {
                Value = professor.Id.ToString(),
                Text = professor.Nome
            })
            .ToList();
    }

    private void ValidarProfessor(int professorId)
    {
        if (professorId > 0 && !_projetoService.ListarProfessores().Any(p => p.Id == professorId))
        {
            ModelState.AddModelError(nameof(ProjetoFormularioViewModel.ProfessorId),
                "O professor selecionado não foi encontrado.");
        }
    }
}
