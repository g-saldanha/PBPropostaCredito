namespace ParanaBanco.Messages;
public record ClienteCadastrado(Guid ClienteId, string Cpf, string Email);
public record PropostaGerada(Guid ClienteId, Guid PropostaId, int Score, string Status);