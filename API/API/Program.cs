
using API.Applications.Autenticacao;
using API.Applications.Auxiliares;
using API.Applications.Services;
using API.Applications.Services.SonarCube;
using API.Applications.Services.Status;
using API.Applications.Services.Tipo;
using API.Applications.Validacoes;
using API.Contexts;
using API.Interfaces;
using API.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using System.Reflection;


// CARREGA O .env
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// PEGA A CONNECTION STRING DO .env
string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
    ?? throw new InvalidOperationException("CONNECTION_STRING não definida no .env");

builder.Services.AddDbContext<SkaL4bleContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddControllers();


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });

    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ======================= SWAGGER =======================
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Digite o token JWT."
    });

    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });

    // Permite adicionar comentários de documentação aos métodos da API,
    // exibindo descrições e respostas no Swagger.
    // Configura o Swagger para ler os comentários XML gerados pelo projeto.
    // No .csproj, GenerateDocumentationFile habilita a geração do XML.
    // NoWarn 1591 desativa avisos sobre métodos sem comentários de documentação.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// ======================= VALIDACAO =======================
builder.Services.AddScoped<IValidacoesRepository, ValidacoesRepository>();
builder.Services.AddScoped<ValidacoesGerais>();

// ======================= PROJETOS =======================
builder.Services.AddScoped<IProjetoRepository, ProjetoRepository>();
builder.Services.AddScoped<ProjetoService>();

// ======================= CARGOS =======================
builder.Services.AddScoped<ICargoRepository, CargoRepository>();
builder.Services.AddScoped<CargoService>();

// ======================= ATUAÇÕES =======================
builder.Services.AddScoped<IAtuacaoRepository, AtuacaoRepository>();
builder.Services.AddScoped<AtuacaoService>();

// ======================= TECNOLOGIAS =======================
builder.Services.AddScoped<ITecnologiaRepository, TecnologiaRepository>();
builder.Services.AddScoped<TecnologiaService>();

// ======================= TIPOS DE PROJETO =======================
builder.Services.AddScoped<ITipoProjetoRepository, TipoProjetoRepository>();
builder.Services.AddScoped<TipoProjetoService>();

// ======================= STATUS DE PROJETO =======================
builder.Services.AddScoped<IStatusProjetoRepository, StatusProjetoRepository>();
builder.Services.AddScoped<StatusProjetoService>();

// ======================= TIPOS DE TICKET =======================
builder.Services.AddScoped<ITipoTicketRepository, TipoTicketRepository>();
builder.Services.AddScoped<TipoTicketService>();

// ======================= STATUS DE TICKET =======================
builder.Services.AddScoped<IStatusTicketRepository, StatusTicketRepository>();
builder.Services.AddScoped<StatusTicketService>();

// ======================= TICKETS =======================
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<TicketService>();

// ======================= USUÁRIOS =======================
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<UsuarioService>();

// ======================= TIPOS DE USUÁRIO =======================
builder.Services.AddScoped<ITipoUsuarioRepository, TipoUsuarioRepository>();
builder.Services.AddScoped<TipoUsuarioService>();


// ======================= AUTENTICAÇÃO =======================
builder.Services.AddScoped<GeradorTokenJwt>();
builder.Services.AddScoped<GeradorTokenRecuperacaoSenha>();
builder.Services.AddScoped<AutenticacaoService>();
builder.Services.AddScoped<ITokenRedefinicaoSenhaRepository, TokenRedefinicaoSenhaRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();


// ======================= MÉTRICAS =======================
builder.Services.AddScoped<IMetricaRepository, MetricaRepository>();
builder.Services.AddScoped<MetricaService>();


// ======================= SONARQUBE =======================
builder.Services.AddDataProtection();
builder.Services.AddScoped<IProjetoSonarRepository, ProjetoSonarRepository>();
builder.Services.AddScoped<ProjetoSonarService>();
builder.Services.AddHttpClient<SonarQubeService>();
builder.Services.AddScoped<CriptografiaService>();

// ====================== GAMEFICACAO ======================
builder.Services.AddScoped<IGameficacaoRepository, GameficacaoRepository>();
builder.Services.AddScoped<GameficacaoService>();
builder.Services.AddScoped<CriarRecomendacaoTreinamento>();

builder.Services.AddOpenApi();



var app = builder.Build();


// ======================= PIPELINE HTTP =======================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}


app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
