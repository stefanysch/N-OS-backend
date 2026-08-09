using N_OS.Application.DTOs;
using N_OS.Application.Interfaces;
using N_OS.Domain.Entities;
using N_OS.Domain.Interfaces;

namespace N_OS.Application.Services;

public class VeiculoService : IVeiculoService
{
    private readonly IVeiculoRepository _repository;

    public VeiculoService(IVeiculoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<VeiculoResponseDTO>> Listar()
    {
        var veiculos = await _repository.Listar();

        return veiculos.Select(MapearParaResponse);
    }

    public async Task<VeiculoResponseDTO?> BuscarPorId(int id)
    {
        var veiculo = await _repository.BuscarPorId(id);

        if (veiculo == null)
            return null;

        return MapearParaResponse(veiculo);
    }

    public async Task<VeiculoResponseDTO> Criar(VeiculoCreateDTO input)
    {
        var veiculo = new Veiculo
        {
            ClienteId = input.ClienteId,
            Placa = input.Placa,
            Marca = input.Marca,
            Modelo = input.Modelo,
            Ano = input.Ano,
            Cor = input.Cor,
            Chassi = input.Chassi,
            CriadoEm = DateTime.UtcNow,
            Ativo = true
        };

        await _repository.Criar(veiculo);
        await _repository.SaveChanges();

        return MapearParaResponse(veiculo);
    }

    public async Task<VeiculoResponseDTO?> Atualizar(
        int id,
        VeiculoUpdateDTO input)
    {
        var veiculo = await _repository.BuscarPorId(id);

        if (veiculo == null)
            return null;

        veiculo.ClienteId = input.ClienteId;
        veiculo.Placa = input.Placa;
        veiculo.Marca = input.Marca;
        veiculo.Modelo = input.Modelo;
        veiculo.Ano = input.Ano;
        veiculo.Cor = input.Cor;
        veiculo.Chassi = input.Chassi;

        await _repository.Atualizar(veiculo);
        await _repository.SaveChanges();

        return MapearParaResponse(veiculo);
    }

    public async Task<bool> Inativar(int id)
    {
        var veiculo = await _repository.BuscarPorId(id);

        if (veiculo == null)
            return false;

        veiculo.Ativo = false;

        await _repository.Atualizar(veiculo);
        await _repository.SaveChanges();

        return true;
    }

    public async Task<bool> Reativar(int id)
    {
        var veiculo = await _repository.BuscarPorId(id);

        if (veiculo == null)
            return false;

        veiculo.Ativo = true;

        await _repository.Atualizar(veiculo);
        await _repository.SaveChanges();

        return true;
    }

    private static VeiculoResponseDTO MapearParaResponse(Veiculo veiculo)
    {
        return new VeiculoResponseDTO
        {
            Id = veiculo.Id,
            ClienteId = veiculo.ClienteId,
            Placa = veiculo.Placa,
            Marca = veiculo.Marca,
            Modelo = veiculo.Modelo,
            Ano = veiculo.Ano,
            Cor = veiculo.Cor,
            Chassi = veiculo.Chassi,
            CriadoEm = veiculo.CriadoEm,
            Ativo = veiculo.Ativo
        };
    }
}