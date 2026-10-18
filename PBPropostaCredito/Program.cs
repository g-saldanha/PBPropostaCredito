using Asp.Versioning;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using PBPropostaCredito.Consumers;
using PBPropostaCredito.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("PBPropostasDb"));

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ClienteCadastradoConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", host =>
        {
            host.Username("guest");
            host.Password("guest");
        });

        cfg.ReceiveEndpoint("fila-cliente-cadastrado", e =>
        {
            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(2)));
            e.ConfigureConsumer<ClienteCadastradoConsumer>(context);
        });
    });
});

var app = builder.Build();

var versionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .Build();

var propostasGroup = app.MapGroup("/api/v{version:apiVersion}/propostas")
    .WithApiVersionSet(versionSet);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

propostasGroup.MapGet("/{clienteId}", async (Guid clienteId, AppDbContext db) =>
{
    var proposta = await db.Propostas.FirstOrDefaultAsync(p => p.ClienteId == clienteId);

    return proposta is not null ? Results.Ok(proposta) : Results.NotFound();
});

app.Run();