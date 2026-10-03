using N_OS.Application.DTOs;
using N_OS.Application.Interfaces;
using N_OS.Domain.Entities;
using N_OS.Domain.Interfaces;
using N_OS.Domain.Utils;

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

        // documento e telefone são gravados só com dígitos;
        // a máscara fica por conta da exibição
        var nome = input.Nome.Trim();
        var documento = DigitosOuNulo(input.Documento);
        var telefone = DigitosOuNulo(input.Telefone);
        var email = VazioParaNulo(input.Email);
        var endereco = VazioParaNulo(input.Endereco);

        if (empresa == null)
        {
            empresa = new Empresa
            {
                Nome = nome,
                Documento = documento,
                Telefone = telefone,
                Email = email,
                Endereco = endereco,
            };

            await _repository.Criar(empresa);
        }
        else
        {
            empresa.Nome = nome;
            empresa.Documento = documento;
            empresa.Telefone = telefone;
            empresa.Email = email;
            empresa.Endereco = endereco;

            await _repository.Atualizar(empresa);
        }

        await _repository.SaveChanges();

        return MapearParaResponse(empresa);
    }

    private static string? DigitosOuNulo(string? valor)
    {
        var digitos = Digitos.Extrair(valor);

        return digitos.Length == 0 ? null : digitos;
    }

    private static string? VazioParaNulo(string? valor)
    {
        var texto = valor?.Trim();

        return string.IsNullOrEmpty(texto) ? null : texto;
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
