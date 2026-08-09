using N_OS.Application.DTOs;
using N_OS.Application.Interfaces;
using N_OS.Domain.Entities;
using N_OS.Domain.Interfaces;

namespace N_OS.Application.Services;

public class ServicoService : IServicoService
{
    private readonly IServicoRepository _repository;

    public ServicoService(IServicoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Servico>> Listar()
    {
        return await _repository.Listar();
    }

    public async Task<Servico?> BuscarPorId(int id)
    {
        return await _repository.BuscarPorId(id);
    }

    public async Task<Servico> Criar(ServicoCreateDTO input)
    {
        var servico = new Servico
        {
            Nome = input.Nome,
            Descricao = input.Descricao,
            Valor = input.Valor,
            CriadoEm = DateTime.UtcNow,
            Ativo = true
        };

        await _repository.Criar(servico);
        await _repository.SaveChanges();

        return servico;
    }

    public async Task<Servico?> Atualizar(
        int id,
        ServicoUpdateDTO input)
    {
        var servico = await _repository.BuscarPorId(id);

        if (servico == null)
            return null;

        servico.Nome = input.Nome;
        servico.Descricao = input.Descricao;
        servico.Valor = input.Valor;

        await _repository.Atualizar(servico);
        await _repository.SaveChanges();

        return servico;
    }

    public async Task<bool> Inativar(int id)
    {
        var servico = await _repository.BuscarPorId(id);

        if (servico == null)
            return false;

        servico.Ativo = false;

        await _repository.Atualizar(servico);
        await _repository.SaveChanges();

        return true;
    }

    public async Task<bool> Reativar(int id)
    {
        var servico = await _repository.BuscarPorId(id);

        if (servico == null)
            return false;

        servico.Ativo = true;

        await _repository.Atualizar(servico);
        await _repository.SaveChanges();

        return true;
    }
}