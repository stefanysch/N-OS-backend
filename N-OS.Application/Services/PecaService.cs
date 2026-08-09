using N_OS.Application.DTOs;
using N_OS.Application.Interfaces;
using N_OS.Domain.Entities;
using N_OS.Domain.Interfaces;

namespace N_OS.Application.Services;

public class PecaService : IPecaService
{
    private readonly IPecaRepository _repository;

    public PecaService(IPecaRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Peca>> Listar()
    {
        return await _repository.Listar();
    }

    public async Task<Peca?> BuscarPorId(int id)
    {
        return await _repository.BuscarPorId(id);
    }

    public async Task<Peca> Criar(PecaCreateDTO input)
    {
        var peca = new Peca
        {
            Nome = input.Nome,
            Descricao = input.Descricao,
            Valor = input.Valor,
            CriadoEm = DateTime.UtcNow,
            Ativo = true
        };

        await _repository.Criar(peca);
        await _repository.SaveChanges();

        return peca;
    }

    public async Task<Peca?> Atualizar(
        int id,
        PecaUpdateDTO input)
    {
        var peca = await _repository.BuscarPorId(id);

        if (peca == null)
            return null;

        peca.Nome = input.Nome;
        peca.Descricao = input.Descricao;
        peca.Valor = input.Valor;

        await _repository.Atualizar(peca);
        await _repository.SaveChanges();

        return peca;
    }

    public async Task<bool> Inativar(int id)
    {
        var peca = await _repository.BuscarPorId(id);

        if (peca == null)
            return false;

        peca.Ativo = false;

        await _repository.Atualizar(peca);
        await _repository.SaveChanges();

        return true;
    }

    public async Task<bool> Reativar(int id)
    {
        var peca = await _repository.BuscarPorId(id);

        if (peca == null)
            return false;

        peca.Ativo = true;

        await _repository.Atualizar(peca);
        await _repository.SaveChanges();

        return true;
    }
}