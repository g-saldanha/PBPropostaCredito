using MassTransit;
using Microsoft.EntityFrameworkCore;
using Moq;
using PBPropostaCredito.Consumers;
using PBPropostaCredito.Data;
using PBPropostaCredito.Messages;
using Xunit;

namespace PBPropostaCredito.Tests.Consumers;

public class ClienteCadastradoConsumerTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task Consume_DeveSalvarProposta_E_PublicarEventoPropostaGerada()
    {
        var dbContext = GetDbContext();
        var consumer = new ClienteCadastradoConsumer(dbContext);

        var clienteId = Guid.NewGuid();
        var mensagem = new ClienteCadastrado(clienteId, "12345678900", "teste@email.com");

        var contextMock = new Mock<ConsumeContext<ClienteCadastrado>>();
        contextMock.Setup(x => x.Message).Returns(mensagem);

        await consumer.Consume(contextMock.Object);

        var propostaSalva = await dbContext.Propostas.FirstOrDefaultAsync(p => p.ClienteId == clienteId);
        
        Assert.NotNull(propostaSalva);
        Assert.Equal(clienteId, propostaSalva.ClienteId);
        Assert.True(propostaSalva.Score >= 0 && propostaSalva.Score <= 1000, "O score deve estar entre 0 e 1000");
        Assert.True(propostaSalva.Status is "Aprovado" or "Negado", "O status deve ser Aprovado ou Negado");
        Assert.NotEqual(Guid.Empty, propostaSalva.Id);

        contextMock.Verify(x => 
                x.Publish(It.IsAny<PropostaGerada>(), It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}