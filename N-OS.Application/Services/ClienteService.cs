using N_OS.Application.DTOs;
using N_OS.Application.Interfaces;
using N_OS.Domain.Entities;
using N_OS.Domain.Interfaces;
using N_OS.Domain.ValueObjects;

namespace N_OS.Application.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _repository;
    private readonly IOrdemDeServicoRepository _ordemDeServicoRepository;

    public ClienteService(
        IClienteRepository repository,
        IOrdemDeServicoRepository ordemDeServicoRepository)
    {
        _repository = repository;
        _ordemDeServicoRepository = ordemDeServicoRepository;
    }

    public async Task<IEnumerable<ClienteResponseDTO>> Listar()
    {
        var clientes = await _repository.Listar();

        return clientes.Select(MapearParaResponse);
    }

    public async Task<ClienteResponseDTO?> BuscarPorId(int id)
    {
        var cliente = await _repository.BuscarPorId(id);

        if (cliente == null)
            return null;

        return MapearParaResponse(cliente);
    }

    public async Task<ClienteResponseDTO> Criar(ClienteCreateDTO input)
    {
        var cliente = new Cliente
        {
            Nome = input.Nome,
            Telefone = input.Telefone,
            Email = input.Email ?? string.Empty,
            CriadoEm = DateTime.UtcNow,
            Ativo = true,

            Documento = new Documento(
                input.TipoDocumento,
                input.Documento),

            Endereco = new Endereco(
                input.Cep,
                input.Logradouro,
                input.Numero,
                input.Bairro,
                input.Cidade,
                input.Estado,
                input.Complemento)
        };

        await _repository.Criar(cliente);
        await _repository.SaveChanges();

        return MapearParaResponse(cliente);
    }

    public async Task<ClienteResponseDTO?> Atualizar(
        int id,
        ClienteUpdateDTO input)
    {
        var cliente = await _repository.BuscarPorId(id);

        if (cliente == null)
            return null;

        cliente.Nome = input.Nome;
        cliente.Telefone = input.Telefone;
        cliente.Email = input.Email ?? string.Empty;

        cliente.Documento = new Documento(
            input.TipoDocumento,
            input.Documento);

        cliente.Endereco = new Endereco(
            input.Cep,
            input.Logradouro,
            input.Numero,
            input.Bairro,
            input.Cidade,
            input.Estado,
            input.Complemento);

        await _repository.Atualizar(cliente);
        await _repository.SaveChanges();

        return MapearParaResponse(cliente);
    }


    public async Task<bool> Inativar(int id)
    {
        var cliente = await _repository.BuscarPorId(id);

        if (cliente == null)
            return false;

        var placasDeVeiculosComOSAtiva =
                await _repository.PlacasDeVeiculosComOSAtiva(id);

        if (placasDeVeiculosComOSAtiva.Any())
        {
            var placas = string.Join(", ", placasDeVeiculosComOSAtiva);
            throw new InvalidOperationException(
                $"Não é possível inativar o cliente porque o(s) veículo(s) [{placas}] " +
                $"possui(em) ordem(ns) de serviço ativa(s).");
        }

        cliente.Ativo = false;

        var veiculos = await _repository.ListarVeiculos(id);

        foreach (var veiculo in veiculos)
        {
            veiculo.Ativo = false;
        }

        await _repository.SaveChanges();

        return true;
    }


    public async Task<bool> Reativar(int id)
    {
        var cliente = await _repository.BuscarPorId(id);

        if (cliente == null)
            return false;

        var veiculos = await _repository.ListarVeiculos(id);

        cliente.Ativo = true;

        foreach (var veiculo in veiculos)
        {
            veiculo.Ativo = true;
        }

        await _repository.Atualizar(cliente);
        await _repository.SaveChanges();

        return true;
    }

    private static ClienteResponseDTO MapearParaResponse(Cliente cliente)
    {
        return new ClienteResponseDTO
        {
            Id = cliente.Id,
            Nome = cliente.Nome,

            Documento = cliente.Documento.Numero,

            Telefone = cliente.Telefone,
            Email = cliente.Email,

            Cep = cliente.Endereco.Cep,
            Logradouro = cliente.Endereco.Logradouro,
            Numero = cliente.Endereco.Numero,
            Complemento = cliente.Endereco.Complemento,
            Bairro = cliente.Endereco.Bairro,
            Cidade = cliente.Endereco.Cidade,
            Estado = cliente.Endereco.Estado,

            CriadoEm = cliente.CriadoEm,
            Ativo = cliente.Ativo,

            Veiculos = cliente.Veiculos
                .Select(veiculo => new VeiculoResponseDTO
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
                })
                .ToList()
        };
    }
}