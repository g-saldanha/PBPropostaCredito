namespace PBPropostaCredito.Messages;

public record ClienteCadastrado(Guid id, string Cpf, string Email);
public record PropostaGerada(Guid ClienteId, Guid PropostaId, int Score, string Status);