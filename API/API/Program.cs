using System.Text;
using API.Applications.Services;
using API.Contexts;
using API.Interfaces;
using API.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// CARREGA O .env
DotNetEnv.Env.Load();

// PEGA A CONNECTION STRING DO .env
string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

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
});

builder.Services.AddScoped<ITipoMetricaRepository, TipoMetricaRepository>();
builder.Services.AddScoped<TipoMetricaService>();

builder.Services.AddOpenApi();

// JWT
//builder.Services.AddScoped<GeradorTokenJwt>();
//builder.Services.AddScoped<AutenticacaoService>();
//builder.Services.AddScoped<GeradorTokenRecuperacaoSenha>();
//builder.Services.AddScoped<ITokenRedefinicaoSenhaRepository, TokenRedefinicaoSenhaRepository>();
//builder.Services.AddScoped<IEmailService, EmailService>();

// Configura o sistema de autenticação da aplicação.
// Aqui estamos dizendo que o tipo de autenticação padrão será JWT Bearer.
// Ou seja: a API vai esperar receber um Token JWT nas requisições.
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)

//    // Adiciona o suporte para autenticação usando JWT.
//    .AddJwtBearer(options =>
//    {
//        // Lê a chave secreta definida no appsettings.json.
//        // Essa chave é usada para ASSINAR o token quando ele é gerado
//        // e também para VALIDAR se o token recebido é verdadeiro.
//        var chave = Environment.GetEnvironmentVariable("JWT_KEY");

//        // Quem emitiu o token (ex: nome da sua aplicação).
//        // Serve para evitar aceitar tokens de outro sistema.
//        var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");

//        // Para quem o token foi criado (normalmente o frontend ou a própria API).
//        // Também ajuda a garantir que o token pertence ao seu sistema.
//        var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

//        // Define as regras que serão usadas para validar o token recebido.
//        soptions.TokenValidationParameters = new TokenValidationParameters
//        {
//            // Verifica se o emissor do token é válido
//            // (se bate com o issuer configurado).
//            ValidateIssuer = true,

//            // Verifica se o destinatário do token é válido
//            // (se bate com o audience configurado).
//            ValidateAudience = true,

//            // Verifica se o token ainda está dentro do prazo de validade.
//            // Se já expirou, a requisição será negada.
//            ValidateLifetime = true,

//            // Verifica se a assinatura do token é válida.
//            // Isso garante que o token não foi alterado.
//            ValidateIssuerSigningKey = true,

//            // Define qual emissor é considerado válido.
//            ValidIssuer = issuer,

//            // Define qual audience é considerado válido.
//            ValidAudience = audience,

//            // Define qual chave será usada para validar a assinatura do token.
//            // A mesma chave usada na geração do JWT deve estar aqui.
//            IssuerSigningKey = new SymmetricSecurityKey(
//                Encoding.UTF8.GetBytes(chave)
//            )
//        };
//    });

var app = builder.Build();

app.UseCors("CorsPolicy");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();