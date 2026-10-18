using MassTransit;
using PBPropostaCredito.Data;
using PBPropostaCredito.Messages;

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

        var score = CalcularScore(mensagem.Cpf);
        var status = score > 100 ? "Aprovado" : "Negado";

        var proposta = new Proposta
        {
            Id = Guid.NewGuid(),
            ClienteId = mensagem.id,
            Score = score,
            Status = status,
            DataProcessamento = DateTime.UtcNow,
        };

        _db.Propostas.Add(proposta);
        await _db.SaveChangesAsync();

        await context.Publish(new PropostaGerada(proposta.ClienteId, proposta.Id, proposta.Score, proposta.Status));
    }

    private int CalcularScore(string cpf)
    {
        return new Random().Next(0, 1001);
    }
}