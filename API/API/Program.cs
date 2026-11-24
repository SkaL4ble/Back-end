using API.Applications.Autenticacao;
using API.Applications.Services;
using API.Applications.Services.SonarCube;
using API.Contexts;
using API.Interfaces;
using API.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Reflection;
using System.Security.Claims;
using System.Text;

// CARREGA O .env
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// PEGA A CONNECTION STRING DO .env
string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
    ?? throw new InvalidOperationException("CONNECTION_STRING não definida no .env");

// CONECTA AO BANCO COM A CONNECTION STRING DO .env
builder.Services.AddDbContext<SkaL4bleContext>(options => options.UseSqlServer(connectionString));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

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

// HABILITA JWT NO SWAGGER
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


builder.Services.AddScoped<IProjetoRepository, ProjetoRepository>();
builder.Services.AddScoped<ProjetoService>();

builder.Services.AddScoped<ICargoRepository, CargoRepository>();
builder.Services.AddScoped<CargoService>();

builder.Services.AddScoped<IAtuacaoRepository, AtuacaoRepository>();
builder.Services.AddScoped<AtuacaoService>();

builder.Services.AddScoped<ITecnologiaRepository, TecnologiaRepository>();
builder.Services.AddScoped<TecnologiaService>();

builder.Services.AddScoped<ITipoProjetoRepository, TipoProjetoRepository>();
builder.Services.AddScoped<TipoProjetoService>();

builder.Services.AddScoped<IStatusProjetoRepository, StatusProjetoRepository>();
builder.Services.AddScoped<StatusProjetoService>();

builder.Services.AddScoped<ITipoTicketRepository, TipoTicketRepository>();
builder.Services.AddScoped<TipoTicketService>();

builder.Services.AddScoped<IStatusTicketRepository, StatusTicketRepository>();
builder.Services.AddScoped<StatusTicketService>();

builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<TicketService>();

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<UsuarioService>();

builder.Services.AddScoped<ITipoUsuarioRepository, TipoUsuarioRepository>();
builder.Services.AddScoped<TipoUsuarioService>();

builder.Services.AddScoped<GeradorTokenJwt>();
builder.Services.AddScoped<GeradorTokenRecuperacaoSenha>();
builder.Services.AddScoped<AutenticacaoService>();
builder.Services.AddScoped<ITokenRedefinicaoSenhaRepository, TokenRedefinicaoSenhaRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();


//======================= SONARQUBE =======================
builder.Services.AddDataProtection();
builder.Services.AddScoped<IProjetoSonarRepository, ProjetoSonarRepository>();
builder.Services.AddScoped<ProjetoSonarService>();
builder.Services.AddHttpClient<SonarQubeService>();
builder.Services.AddScoped<CriptografiaService>();


builder.Services.AddOpenApi();

var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

if (string.IsNullOrWhiteSpace(jwtKey) ||
    string.IsNullOrWhiteSpace(jwtIssuer) ||
    string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new InvalidOperationException(
        "As configurações JWT estão incompletas no .env.");
}

var jwtKeyBytes = Encoding.UTF8.GetBytes(jwtKey);

if (jwtKeyBytes.Length < 32)
{
    throw new InvalidOperationException(
        "JWT_KEY precisa ter pelo menos 32 bytes.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(jwtKeyBytes),

            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.Name
        };
    });

builder.Services.AddAuthorization();


var app = builder.Build();


// Configure the HTTP request pipeline.
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