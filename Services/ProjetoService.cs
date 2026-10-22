using GerenciadorAcademico.Models;
using GerenciadorAcademico.ViewModels;

namespace GerenciadorAcademico.Services;

public class ProjetoService : IProjetoService
{
    // Dados de exemplo. Todos são reiniciados quando a aplicação é encerrada.
    private readonly List<Professor> _professores =
    [
        new Professor { Id = 1, Nome = "Ana Souza", Email = "ana.souza@academico.com" },
        new Professor { Id = 2, Nome = "Carlos Mendes", Email = "carlos.mendes@academico.com" },
        new Professor { Id = 3, Nome = "Beatriz Lima", Email = "beatriz.lima@academico.com" }
    ];

    private readonly List<Projeto> _projetos =
    [
        new Projeto
        {
            Id = 1,
            Titulo = "Sistema de Biblioteca Escolar",
            Descricao = "Aplicação para organizar o cadastro de livros e os empréstimos da biblioteca escolar.",
            CargaHoraria = 80,
            ProfessorId = 1
        },
        new Projeto
        {
            Id = 2,
            Titulo = "Robótica Educacional",
            Descricao = "Construção de protótipos de robótica para atividades de aprendizagem em sala de aula.",
            CargaHoraria = 120,
            ProfessorId = 2
        },
        new Projeto
        {
            Id = 3,
            Titulo = "Inclusão Digital",
            Descricao = "Oficinas de informática para aproximar a comunidade das ferramentas digitais.",
            CargaHoraria = 60,
            ProfessorId = 3
        }
    ];

    public List<Professor> ListarProfessores()
    {
        return _professores.ToList();
    }

    private Professor? ObterProfessorPorId(int professorId)
    {
        return _professores.FirstOrDefault(professor => professor.Id == professorId);
    }

    private Projeto VincularProfessor(Projeto projeto)
    {
        projeto.Professor = ObterProfessorPorId(projeto.ProfessorId);
        return projeto;
    }

    public List<Projeto> Listar()
    {
        return _projetos.Select(VincularProfessor).ToList();
    }

    public Projeto? ObterPorId(int id)
    {
        var projeto = _projetos.FirstOrDefault(projeto => projeto.Id == id);
        return projeto is null ? null : VincularProfessor(projeto);
    }

    public List<Projeto> PesquisarPorTitulo(string? titulo)
    {
        var projetos = Listar();

        if (string.IsNullOrWhiteSpace(titulo))
            return projetos;

        var termo = titulo.Trim();
        return projetos
            .Where(projeto => projeto.Titulo.Contains(
                termo, StringComparison.CurrentCultureIgnoreCase))
            .ToList();
    }

    public void Adicionar(NovoProjetoViewModel model)
    {
        var professor = ObterProfessorPorId(model.ProfessorId)
            ?? throw new ArgumentException("Professor não encontrado.", nameof(model));

        var novoProjeto = new Projeto
        {
            Id = GerarNovoId(),
            Titulo = model.Titulo.Trim(),
            Descricao = model.Descricao.Trim(),
            CargaHoraria = model.CargaHoraria,
            ProfessorId = professor.Id,
            Professor = professor
        };

        _projetos.Add(novoProjeto);
    }

    public bool Atualizar(EditarProjetoViewModel model)
    {
        var projeto = ObterPorId(model.Id);
        if (projeto is null)
            return false;

        var professor = ObterProfessorPorId(model.ProfessorId);
        if (professor is null)
            return false;

        projeto.Titulo = model.Titulo.Trim();
        projeto.Descricao = model.Descricao.Trim();
        projeto.CargaHoraria = model.CargaHoraria;
        projeto.ProfessorId = professor.Id;
        projeto.Professor = professor;

        return true;
    }

    public bool Excluir(int id)
    {
        var projeto = _projetos.FirstOrDefault(projeto => projeto.Id == id);
        if (projeto is null)
            return false;

        _projetos.Remove(projeto);
        return true;
    }

    private int GerarNovoId()
    {
        return _projetos.Count == 0 ? 1 : _projetos.Max(projeto => projeto.Id) + 1;
    }
}
