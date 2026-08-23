using System.Text.RegularExpressions;

namespace N_OS.Domain.ValueObjects;

public class Endereco : ValueObject
{
    private static readonly HashSet<string> UfsValidas = new()
    {
        "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO",
        "MA", "MT", "MS", "MG", "PA", "PB", "PR", "PE", "PI",
        "RJ", "RN", "RS", "RO", "RR", "SC", "SP", "SE", "TO",
    };

    public string? Cep { get; private set; }
    public string? Logradouro { get; private set; }
    public string? Numero { get; private set; }
    public string? Complemento { get; private set; }
    public string? Bairro { get; private set; }
    public string? Cidade { get; private set; }
    public string? Estado { get; private set; }

    private Endereco()
    {
    }

    public Endereco(
        string cep,
        string logradouro,
        string numero,
        string bairro,
        string cidade,
        string estado,
        string? complemento = null)
    {
        cep = cep.Trim();
        logradouro = logradouro.Trim();
        numero = numero.Trim();
        bairro = bairro.Trim();
        cidade = cidade.Trim();
        estado = estado.Trim().ToUpper();

        Validar(cep, logradouro, numero, bairro, cidade, estado);

        Cep = cep;
        Logradouro = logradouro;
        Numero = numero;
        Bairro = bairro;
        Cidade = cidade;
        Estado = estado;
        Complemento = complemento?.Trim();
    }

    private static void Validar(
        string cep,
        string logradouro,
        string numero,
        string bairro,
        string cidade,
        string estado)
    {
        if (string.IsNullOrWhiteSpace(cep) || !Regex.IsMatch(cep, @"^\d{5}-?\d{3}$"))
            throw new ArgumentException("CEP inválido.");

        if (string.IsNullOrWhiteSpace(logradouro))
            throw new ArgumentException("Logradouro é obrigatório.");

        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException("Número é obrigatório.");

        if (string.IsNullOrWhiteSpace(bairro))
            throw new ArgumentException("Bairro é obrigatório.");

        if (string.IsNullOrWhiteSpace(cidade))
            throw new ArgumentException("Cidade é obrigatória.");

        if (!UfsValidas.Contains(estado))
            throw new ArgumentException("Estado (UF) inválido.");
    }

    public override string ToString() =>
        $"{Logradouro}, {Numero} - {Bairro}, {Cidade}/{Estado}";

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Cep;
        yield return Logradouro;
        yield return Numero;
        yield return Complemento;
        yield return Bairro;
        yield return Cidade;
        yield return Estado;
    }
}
