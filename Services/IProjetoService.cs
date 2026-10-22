using GerenciadorAcademico.Models;
using GerenciadorAcademico.ViewModels;

namespace GerenciadorAcademico.Services;

public interface IProjetoService
{
    List<Projeto> Listar();
    List<Projeto> PesquisarPorTitulo(string? titulo);
    Projeto? ObterPorId(int id);
    List<Professor> ListarProfessores();
    void Adicionar(NovoProjetoViewModel model);
    bool Atualizar(EditarProjetoViewModel model);
    bool Excluir(int id);
}
