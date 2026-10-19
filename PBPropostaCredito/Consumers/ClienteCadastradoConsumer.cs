using MassTransit;
using PBPropostaCredito.Data;
using ParanaBanco.Messages;

namespace PBPropostaCredito.Consumers;

public class ClienteCadastradoConsumer : IConsumer<ClienteCadastrado>
{
    private readonly AppDbContext _db;

    public ClienteCadastradoConsumer(AppDbContext db)
    {
        _db = db;
    }

    public async Task Consume(ConsumeContext<ClienteCadastrado> context)
    {
        var mensagem = context.Message;
        Console.WriteLine($"\n[MS2] ---> RECEBEU evento ClienteCadastrado! ClienteID: {mensagem.ClienteId}");

        var score = CalcularScore(mensagem.Cpf);
        var status = score > 100 ? "Aprovado" : "Negado";

        var proposta = new Proposta
        {
            Id = Guid.NewGuid(),
            ClienteId = mensagem.ClienteId,
            Score = score,
            Status = status,
            DataProcessamento = DateTime.UtcNow,
        };

        _db.Propostas.Add(proposta);
        await _db.SaveChangesAsync();
        Console.WriteLine($"[MS2] Proposta gerada na base. Score: {score} | Status: {status}");
        Console.WriteLine($"[MS2] Preparando para enviar evento PropostaGerada...");
        await context.Publish(new PropostaGerada(proposta.ClienteId, proposta.Id, proposta.Score, proposta.Status));
        Console.WriteLine($"[MS2] <--- Evento PropostaGerada enviado com sucesso!\n");
    }

    private int CalcularScore(string cpf)
    {
        return new Random().Next(0, 1001);
    }
}