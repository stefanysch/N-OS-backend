using N_OS.Application.DTOs;
using N_OS.Application.Interfaces;
using N_OS.Domain.Entities;
using N_OS.Domain.Enums;
using N_OS.Domain.Interfaces;

namespace N_OS.Application.Services;

public class OrdemDeServicoService : IOrdemDeServicoService
{
    private readonly IOrdemDeServicoRepository _repository;
    private readonly IPecaRepository _pecaRepository;
    private readonly IServicoRepository _servicoRepository;
    private readonly IVeiculoRepository _veiculoRepository;

    public OrdemDeServicoService(
        IOrdemDeServicoRepository repository,
        IPecaRepository pecaRepository,
        IServicoRepository servicoRepository,
        IVeiculoRepository veiculoRepository)
    {
        _repository = repository;
        _pecaRepository = pecaRepository;
        _servicoRepository = servicoRepository;
        _veiculoRepository = veiculoRepository;
    }

    public async Task<IEnumerable<OrdemDeServicoResponseDTO>> Listar()
    {
        var ordens = await _repository.Listar();

        return ordens.Select(MapearResponse);
    }

    public async Task<OrdemDeServicoResponseDTO?> BuscarPorId(int id)
    {
        var ordemDeServico = await _repository.BuscarPorId(id);

        if (ordemDeServico == null)
            return null;

        return MapearResponse(ordemDeServico);
    }

    public async Task<OrdemDeServicoResponseDTO> Criar(
        OrdemDeServicoCreateDTO input)
    {
        await ValidarVeiculo(input.VeiculoId);

        ValidarItens(input.Itens);

        var ordemDeServico = new OrdemDeServico
        {
            VeiculoId = input.VeiculoId,
            DescricaoProblema = input.DescricaoProblema,
            Observacoes = input.Observacoes ?? string.Empty,
            Status = StatusOS.Aguardando,
            ValorTotal = 0,
            DataAbertura = DateTime.UtcNow,
            Ativo = true
        };

        foreach (var inputItem in input.Itens)
        {
            var item = await CriarItem(inputItem);

            ordemDeServico.ItensOS.Add(item);
        }

        ordemDeServico.ValorTotal =
            ordemDeServico.ItensOS.Sum(item => item.Subtotal);

        await _repository.Criar(ordemDeServico);
        await _repository.SaveChanges();

        return MapearResponse(ordemDeServico);
    }

    public async Task<OrdemDeServicoResponseDTO?> Atualizar(
        int id,
        OrdemDeServicoUpdateDTO input)
    {
        var ordemDeServico =
            await _repository.BuscarPorId(id);

        if (ordemDeServico == null)
            return null;
        ordemDeServico.DescricaoProblema =
            input.DescricaoProblema;

        ordemDeServico.Observacoes =
            input.Observacoes ?? string.Empty;

        // no update, novos itens são adicionados, os itens existentes não são removidos.
        if (input.Itens != null && input.Itens.Count > 0)
        {
            ValidarItens(input.Itens);

            foreach (var inputItem in input.Itens)
            {
                var item = await CriarItem(inputItem);

                ordemDeServico.ItensOS.Add(item);
            }
        }

        // recalcula o valor considerando todos os itens
        ordemDeServico.ValorTotal =
            ordemDeServico.ItensOS.Sum(item => item.Subtotal);

        await _repository.Atualizar(ordemDeServico);
        await _repository.SaveChanges();

        return MapearResponse(ordemDeServico);
    }

    public async Task<OrdemDeServicoResponseDTO?> AlterarStatus(
        int id,
        OrdemDeServicoStatusDTO input)
    {
        var ordemDeServico =
            await _repository.BuscarPorId(id);

        if (ordemDeServico == null)
            return null;

        ordemDeServico.Status = input.Status;

        await _repository.Atualizar(ordemDeServico);
        await _repository.SaveChanges();

        return MapearResponse(ordemDeServico);
    }

    public async Task<bool> Inativar(int id)
    {
        var ordemDeServico =
            await _repository.BuscarPorId(id);

        if (ordemDeServico == null)
            return false;

        ordemDeServico.Ativo = false;

        await _repository.Atualizar(ordemDeServico);
        await _repository.SaveChanges();

        return true;
    }

    public async Task<bool> Reativar(int id)
    {
        var ordemDeServico =
            await _repository.BuscarPorId(id);

        if (ordemDeServico == null)
            return false;

        ordemDeServico.Ativo = true;

        await _repository.Atualizar(ordemDeServico);
        await _repository.SaveChanges();

        return true;
    }

    private async Task ValidarVeiculo(int veiculoId)
    {
        if (veiculoId <= 0)
        {
            throw new ArgumentException(
                "O veículo é obrigatório.");
        }

        var veiculo =
            await _veiculoRepository.BuscarPorId(veiculoId);

        if (veiculo == null)
        {
            throw new ArgumentException(
                $"Veículo com ID {veiculoId} não encontrado.");
        }
    }

    private static void ValidarItens(
        List<ItemOSCreateDTO>? itens)
    {
        if (itens == null || itens.Count == 0)
        {
            throw new ArgumentException(
                "A ordem de serviço deve possuir pelo menos um item.");
        }

        foreach (var item in itens)
        {
            if (item.Quantidade <= 0)
            {
                throw new ArgumentException(
                    "A quantidade do item deve ser maior que zero.");
            }

            if (!item.PecaId.HasValue &&
                !item.ServicoId.HasValue)
            {
                throw new ArgumentException(
                    "Cada item deve possuir pelo menos uma peça ou um serviço.");
            }
        }
    }

    private async Task<ItemOS> CriarItem(
        ItemOSCreateDTO input)
    {
        decimal valorAplicado = 0;

        var item = new ItemOS
        {
            Quantidade = input.Quantidade
        };

        if (input.PecaId.HasValue)
        {
            var peca =
                await _pecaRepository.BuscarPorId(
                    input.PecaId.Value);

            if (peca == null)
            {
                throw new ArgumentException(
                    $"Peça com ID {input.PecaId.Value} não encontrada.");
            }

            item.PecaId = peca.Id;

            valorAplicado += peca.Valor;
        }

        if (input.ServicoId.HasValue)
        {
            var servico =
                await _servicoRepository.BuscarPorId(
                    input.ServicoId.Value);

            if (servico == null)
            {
                throw new ArgumentException(
                    $"Serviço com ID {input.ServicoId.Value} não encontrado.");
            }

            item.ServicoId = servico.Id;

            valorAplicado += servico.Valor;
        }

        item.ValorAplicado = valorAplicado;

        item.Subtotal =
            valorAplicado * input.Quantidade;

        return item;
    }

    private static OrdemDeServicoResponseDTO MapearResponse(
        OrdemDeServico ordemDeServico)
    {
        return new OrdemDeServicoResponseDTO
        {
            Id = ordemDeServico.Id,
            VeiculoId = ordemDeServico.VeiculoId,
            Status = (int)ordemDeServico.Status,
            DescricaoProblema = ordemDeServico.DescricaoProblema,
            Observacoes = ordemDeServico.Observacoes,
            ValorTotal = ordemDeServico.ValorTotal,
            DataAbertura = ordemDeServico.DataAbertura,
            Ativo = ordemDeServico.Ativo,

            Itens = ordemDeServico.ItensOS.Select(item =>
                new ItemOSResponseDTO
                {
                    Id = item.Id,
                    Quantidade = item.Quantidade,
                    ValorAplicado = item.ValorAplicado,
                    Subtotal = item.Subtotal,

                    PecaNome = item.Peca?.Nome,
                    ServicoNome = item.Servico?.Nome
                }).ToList()
        };
    }
}