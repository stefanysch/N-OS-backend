namespace N_OS.Domain.ValueObjects;

public class Endereco
{
    public string Cep { get; private set; }
    public string Logradouro { get; private set; }
    public string Numero { get; private set; }
    public string? Complemento { get; private set; }
    public string Bairro { get; private set; }
    public string Cidade { get; private set; }
    public string Estado { get; private set; }

    private Endereco()
    {
        Cep = string.Empty;
        Logradouro = string.Empty;
        Numero = string.Empty;
        Bairro = string.Empty;
        Cidade = string.Empty;
        Estado = string.Empty;
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
        Cep = cep.Trim();
        Logradouro = logradouro.Trim();
        Numero = numero.Trim();
        Bairro = bairro.Trim();
        Cidade = cidade.Trim();
        Estado = estado.Trim().ToUpper();
        Complemento = complemento?.Trim();
    }
}