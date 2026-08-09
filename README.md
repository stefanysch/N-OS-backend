# N-OS Backend

API do projeto **N-OS**, desenvolvida em **C# com .NET**, seguindo os princípios de **DDD (Domain-Driven Design)** e separação de responsabilidades entre as camadas da aplicação.

---

## 📁 Estrutura do Projeto

| Projeto               | Responsabilidade                                                                          |
| --------------------- | ----------------------------------------------------------------------------------------- |
| `N-OS.Domain`         | Entidades, enums, value objects e conceitos centrais do domínio                           |
| `N-OS.Application`    | DTOs, casos de uso, services e regras relacionadas à aplicação                            |
| `N-OS.Infrastructure` | Persistência de dados, Entity Framework Core, PostgreSQL e implementação dos repositories |
| `N-OS.API`            | Controllers, endpoints HTTP e configuração da aplicação                                   |

---

## 🚀 Tecnologias

* C#
* .NET 10
* ASP.NET Core Web API
* Entity Framework Core
* PostgreSQL
* Npgsql
* Swagger / OpenAPI

---

## ⚙️ Configuração

Crie um arquivo `appsettings.Development.json` no projeto da API com sua connection string local:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=nos_db;Username=postgres;Password=SUA_SENHA"
  }
}
```

---

## 🗄️ Banco de Dados

O projeto utiliza **PostgreSQL** para persistência dos dados e **Entity Framework Core** para mapeamento e gerenciamento das entidades.

As migrations ficam no projeto `N-OS.Infrastructure`.

### Visual Studio — Console do Gerenciador de Pacotes

Para criar uma migration:

```powershell
Add-Migration NomeDaMigration
```

Para aplicar as migrations:

```powershell
Update-Database
```

### VS Code / Terminal — .NET CLI

Para criar uma migration:

```bash
dotnet ef migrations add NomeDaMigration --project N-OS.Infrastructure --startup-project N-OS.API
```

Para aplicar as migrations:

```bash
dotnet ef database update --project N-OS.Infrastructure --startup-project N-OS.API
```

Para listar as migrations:

```bash
dotnet ef migrations list --project N-OS.Infrastructure --startup-project N-OS.API
```

É necessário ter a ferramenta `dotnet-ef` instalada para utilizar os comandos via terminal.

Para instalar:

```bash
dotnet tool install --global dotnet-ef
```

---

## ▶️ API

Certifique-se de que o **PostgreSQL está em execução** antes de iniciar a aplicação.

### VS Code / Terminal

Na raiz da solução:

```bash
dotnet run --project N-OS.API
```

Ou navegue até o projeto da API:

```bash
cd N-OS.API
dotnet run
```

### Visual Studio

Normalmente não é necessário utilizar `dotnet run`, pois a execução é feita pela própria IDE:

* ▶️ **Start / Iniciar**
* `F5` — execução com debug
* `Ctrl + F5` — execução sem debug

O Visual Studio compila e inicia a API automaticamente.

---

## 🔒 Boas Práticas Adotadas

* Separação de responsabilidades entre as camadas
* Organização baseada em DDD
* Utilização de DTOs para entrada e saída de dados
* Services para implementação dos casos de uso
* Repositories para acesso aos dados
* Entity Framework Core para persistência
* Migrations para versionamento do banco de dados
* Value Objects para representar conceitos do domínio
* Configurações sensíveis não versionadas
* Estrutura preparada para manutenção e evolução futura

---

## 📬 Contato

Para dúvidas, feedbacks ou informações adicionais sobre o desenvolvimento deste projeto, entre em contato:

* [stefany@edu.unifil.br](mailto:stefany@edu.unifil.br)
