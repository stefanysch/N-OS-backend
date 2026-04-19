\# N-OS Backend



API do projeto \*\*N-OS\*\* desenvolvida em \*\*C# com .NET\*\*, seguindo os princípios de \*\*DDD (Domain-Driven Design)\*\* para garantir organização, manutenção e separação clara de responsabilidades.



\---



\## 📁 Estrutura do Projeto



| Projeto | Responsabilidade |

|---|---|

| `N-OS.Domain` | Entidades e regras de negócio |

| `N-OS.Application` | Casos de uso |

| `N-OS.Infrastructure` | Persistência de dados e integrações |

| `N-OS.API` | Controllers e Endpoints HTTP |



\---



\## 🚀 Tecnologias



\- .NET

\- C#

\- Entity Framework Core

\- PostgreSQL

\- pgAdmin



\---



\## ⚙️ Configuração



Crie um arquivo `appsettings.Development.json` no projeto da API com sua connection string local:



```json

{

&#x20; "ConnectionStrings": {

&#x20;   "DefaultConnection": "Host=localhost;Port=5432;Database=nos\_db;Username=postgres;Password=SUA\_SENHA"

&#x20; }

}

```



> ⚠️ Este arquivo não deve ser versionado. Certifique-se de que está listado no `.gitignore`.



\---



\## Banco de Dados



As migrations podem ser executadas de duas formas:



\### Visual Studio — Console do Gerenciador de Pacotes



```powershell

Add-Migration NomeDaMigration

Update-Database

```



\### VS Code / Terminal — .NET CLI



```bash

dotnet ef migrations add NomeDaMigration

dotnet ef database update

```



> É necessário ter a ferramenta `dotnet-ef` instalada para uso via terminal.



\---



\## API



Certifique-se de que o PostgreSQL está em execução antes de iniciar a aplicação.



\### VS Code



Abra a pasta do backend e rode no terminal integrado:



```bash

dotnet run

```



\### Prompt / PowerShell



Navegue até a pasta onde está o `.csproj` da API e execute:



```bash

cd N-OS

dotnet run

```



\### Visual Studio



Normalmente não é necessário usar `dotnet run`, pois a execução é feita pela própria IDE:



\- ▶️ \*\*Start / Iniciar\*\*

\- `F5` — execução com debug

\- `Ctrl + F5` — execução sem debug



O Visual Studio compila e sobe a API automaticamente.



\---



\## 🔒 Boas Práticas Adotadas



\- Configurações sensíveis não versionadas

\- Estrutura em camadas com DDD

\- Separação de responsabilidades

\- Organização voltada para escalabilidade futura



\---



\## 📬 Contato



Para dúvidas, feedbacks ou informações adicionais sobre o desenvolvimento deste projeto, sinta-se à vontade para entrar em contato:



\- stefany@edu.unifil.br

