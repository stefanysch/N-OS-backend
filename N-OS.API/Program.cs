using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using N_OS.API.Handlers;
using N_OS.API.Seguranca;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using N_OS.Application.DTOs;
using N_OS.Application.Interfaces;
using N_OS.Application.Services;
using N_OS.Domain.Interfaces;
using N_OS.Infrastructure.Data;
using N_OS.Infrastructure.Queries;
using N_OS.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
);

builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IVeiculoRepository, VeiculoRepository>();
builder.Services.AddScoped<IPecaRepository, PecaRepository>();
builder.Services.AddScoped<IServicoRepository, ServicoRepository>();
builder.Services.AddScoped<IOrdemDeServicoRepository, OrdemDeServicoRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddSingleton<CookiesSessao>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IEmpresaRepository, EmpresaRepository>();

builder.Services.AddScoped<IClienteQueries, ClienteQueries>();
builder.Services.AddScoped<IVeiculoQueries, VeiculoQueries>();
builder.Services.AddScoped<IPecaQueries, PecaQueries>();
builder.Services.AddScoped<IServicoQueries, ServicoQueries>();
builder.Services.AddScoped<IOrdemDeServicoQueries, OrdemDeServicoQueries>();

builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IVeiculoService, VeiculoService>();
builder.Services.AddScoped<IPecaService, PecaService>();
builder.Services.AddScoped<IServicoService, ServicoService>();
builder.Services.AddScoped<IOrdemDeServicoService, OrdemDeServicoService>();
builder.Services.AddScoped<IEmpresaService, EmpresaService>();
builder.Services.AddScoped<IPerfilService, PerfilService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key não configurada.");

// HS256 pede uma chave de pelo menos 256 bits (32 bytes)
if (Encoding.UTF8.GetByteCount(jwtKey) < 32)
{
    throw new InvalidOperationException(
        "Jwt:Key muito curta: use ao menos 32 caracteres aleatórios.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            // o navegador manda o token no cookie HttpOnly; quem não é navegador
            // (testes, scripts) ainda pode usar o cabeçalho Authorization: Bearer
            OnMessageReceived = contexto =>
            {
                if (!contexto.Request.Headers.ContainsKey("Authorization"))
                {
                    contexto.Token = contexto.Request.Cookies[CookiesSessao.Acesso];
                }

                return Task.CompletedTask;
            },

            // um token assinado e dentro da validade ainda precisa de um usuário
            // ativo e da mesma versão: inativar a conta ou trocar a senha
            // derruba os tokens antigos na hora
            OnTokenValidated = async contexto =>
            {
                var principal = contexto.Principal;

                var idTexto =
                    principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
                    principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var versaoTexto =
                    principal?.FindFirst(AuthService.ClaimVersaoToken)?.Value;

                if (!int.TryParse(idTexto, out var usuarioId) ||
                    !int.TryParse(versaoTexto, out var versao))
                {
                    contexto.Fail("Token inválido.");
                    return;
                }

                var usuarios = contexto.HttpContext.RequestServices
                    .GetRequiredService<IUsuarioRepository>();

                var usuario = await usuarios.BuscarPorId(usuarioId);

                if (usuario is null || !usuario.Ativo || usuario.VersaoToken != versao)
                {
                    contexto.Fail("Sessão encerrada.");
                }
            },
        };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
            RequireExpirationTime = true,
            RequireSignedTokens = true,

            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddProblemDetails(options =>
{
    // toda resposta de erro leva "mensagem" e "traceId"
    options.CustomizeProblemDetails = context =>
    {
        var problema = context.ProblemDetails;

        problema.Extensions.TryAdd("mensagem", problema.Detail ?? problema.Title);
        problema.Extensions["traceId"] =
            Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
    };
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services
    .AddControllers(options =>
    {
        options.Filters.Add(new AuthorizeFilter());
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        // falha de validação dos DTOs: 400 com "errors" por campo
        // e "mensagem" com todos os erros juntos
        options.InvalidModelStateResponseFactory = context =>
        {
            var problema = new ValidationProblemDetails(context.ModelState)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Erro de validação",
            };

            problema.Extensions["mensagem"] = string.Join(
                "\n",
                problema.Errors.Values.SelectMany(erros => erros));

            problema.Extensions["traceId"] =
                Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;

            return new BadRequestObjectResult(problema)
            {
                ContentTypes = { "application/problem+json" },
            };
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "N-OS API",
        Version = "v1",
        Description =
            "API do projeto N-OS. " +
            "Faça login em `POST /api/auth/login`: a sessão fica em cookies " +
            "HttpOnly e o próprio navegador os envia nas demais rotas. " +
            "Erros seguem o padrão ProblemDetails com o campo extra `mensagem`.",
    });

    var xml = Path.Combine(
        AppContext.BaseDirectory,
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");

    options.IncludeXmlComments(xml);
});

// origens liberadas no CORS (em produção: o endereço do front, via configuração).
// Sem configuração, só o ambiente de desenvolvimento assume o Vite local;
// em produção a API se recusa a subir em vez de liberar uma origem por engano.
var origensCors = builder.Configuration
    .GetSection("Cors:Origens")
    .Get<string[]>();

if (origensCors is null || origensCors.Length == 0)
{
    if (!builder.Environment.IsDevelopment())
    {
        throw new InvalidOperationException(
            "Cors:Origens não configurado: informe o endereço do front (ex.: Cors__Origens__0).");
    }

    origensCors = ["http://localhost:5173"];
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy.WithOrigins(origensCors)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// limite de tentativas de login por IP (a trava por conta fica no AuthService)
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("login", contexto => LimitePorIp(contexto, 10));

    options.AddPolicy("refresh", contexto => LimitePorIp(contexto, 30));

    options.OnRejected = async (contexto, cancellationToken) =>
    {
        contexto.HttpContext.Response.Headers.RetryAfter = "60";

        await contexto.HttpContext.Response.WriteAsJsonAsync(
            new
            {
                status = StatusCodes.Status429TooManyRequests,
                title = "Muitas requisições",
                mensagem = "Muitas tentativas. Aguarde um minuto e tente novamente.",
            },
            cancellationToken);
    };
});

// atrás de proxy reverso (nginx, Azure, etc.) o IP real vem no X-Forwarded-For.
// só habilitar quando a API NÃO estiver exposta direto à internet.
if (builder.Configuration.GetValue<bool>("Seguranca:ConfiarEmProxy"))
{
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders =
            ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        options.KnownIPNetworks.Clear();
        options.KnownProxies.Clear();
    });
}

var app = builder.Build();

if (builder.Configuration.GetValue<bool>("Seguranca:ConfiarEmProxy"))
{
    app.UseForwardedHeaders();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

if (builder.Configuration.GetValue<bool>("Seguranca:RedirecionarParaHttps"))
{
    app.UseHttpsRedirection();
}

app.Use(async (contexto, proximo) =>
{
    var cabecalhos = contexto.Response.Headers;

    cabecalhos["X-Content-Type-Options"] = "nosniff";
    cabecalhos["X-Frame-Options"] = "DENY";
    cabecalhos["Referrer-Policy"] = "no-referrer";

    // respostas da API (token, dados de cliente) não devem ficar em cache
    if (contexto.Request.Path.StartsWithSegments("/api"))
    {
        cabecalhos["Cache-Control"] = "no-store";

        // a API só devolve JSON: nada dela deve ser executado ou embutido num frame
        cabecalhos["Content-Security-Policy"] = "default-src 'none'; frame-ancestors 'none'";
    }

    await proximo();
});

app.UseCors("frontend");

// CSRF: com a sessão em cookie, requisições que alteram dados só valem se vierem do front
// (object): sem o cast o array seria aberto em vários argumentos do construtor
app.UseMiddleware<ValidacaoDeOrigemMiddleware>((object)origensCors);

app.UseExceptionHandler();
// dá corpo ProblemDetails a respostas vazias (401, 403, 404 de rota...)
app.UseStatusCodePages();

app.MapGet("/", () => "API rodando, é N-OS! 🚀")
    .WithSummary("Verifica se a API está no ar.");

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// primeiro acesso: como o cadastro exige login, o primeiro usuário vem da
// configuração (variáveis Bootstrap__Nome / Bootstrap__Email / Bootstrap__Senha).
// Só age com a tabela de usuários vazia; depois de criar, remova a senha da configuração.
await CriarPrimeiroUsuarioSeConfigurado(app);

app.Run();

static RateLimitPartition<string> LimitePorIp(HttpContext contexto, int limitePorMinuto) =>
    RateLimitPartition.GetFixedWindowLimiter(
        contexto.Connection.RemoteIpAddress?.ToString() ?? "desconhecido",
        _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = limitePorMinuto,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0,
        });

static async Task CriarPrimeiroUsuarioSeConfigurado(WebApplication app)
{
    var configuracao = app.Configuration.GetSection("Bootstrap");

    var email = configuracao["Email"];
    var senha = configuracao["Senha"];

    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
        return;

    using var escopo = app.Services.CreateScope();

    var logger = escopo.ServiceProvider
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger("Bootstrap");

    try
    {
        var auth = escopo.ServiceProvider.GetRequiredService<IAuthService>();

        var criado = await auth.CriarPrimeiroUsuario(new RegistrarUsuarioDTO
        {
            Nome = configuracao["Nome"] ?? "Administrador",
            Email = email,
            Senha = senha,
        });

        if (criado)
        {
            logger.LogWarning(
                "Primeiro usuário {Email} criado. Remova Bootstrap:Senha da configuração.",
                email);
        }
    }
    catch (ValidationException erro)
    {
        logger.LogError("Bootstrap ignorado: {Mensagem}", erro.Message);
    }
    catch (Exception erro)
    {
        logger.LogError(erro, "Não foi possível criar o primeiro usuário.");
    }
}