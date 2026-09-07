using N_OS.Application.DTOs;
using N_OS.Application.Interfaces;
using N_OS.Domain.Entities;
using N_OS.Domain.Interfaces;

namespace N_OS.Application.Services;

public class EmpresaService : IEmpresaService
{
    private readonly IEmpresaRepository _repository;

    public EmpresaService(IEmpresaRepository repository)
    {
        _repository = repository;
    }

    public async Task<EmpresaResponseDTO> Obter()
    {
        var empresa = await _repository.Obter();

        if (empresa == null)
        {
            return new EmpresaResponseDTO
            {
                Id = 0,
                Nome = string.Empty,
                Configurada = false,
            };
        }

        return MapearParaResponse(empresa);
    }

    public async Task<EmpresaResponseDTO> Atualizar(EmpresaUpdateDTO input)
    {
        var empresa = await _repository.Obter();

        if (empresa == null)
        {
            empresa = new Empresa
            {
                Nome = input.Nome,
                Documento = input.Documento,
                Telefone = input.Telefone,
                Email = input.Email,
                Endereco = input.Endereco,
            };

            await _repository.Criar(empresa);
        }
        else
        {
            empresa.Nome = input.Nome;
            empresa.Documento = input.Documento;
            empresa.Telefone = input.Telefone;
            empresa.Email = input.Email;
            empresa.Endereco = input.Endereco;

            await _repository.Atualizar(empresa);
        }

        await _repository.SaveChanges();

        return MapearParaResponse(empresa);
    }

    private static EmpresaResponseDTO MapearParaResponse(Empresa empresa)
    {
        return new EmpresaResponseDTO
        {
            Id = empresa.Id,
            Nome = empresa.Nome,
            Documento = empresa.Documento,
            Telefone = empresa.Telefone,
            Email = empresa.Email,
            Endereco = empresa.Endereco,
            Configurada = true,
        };
    }
}
