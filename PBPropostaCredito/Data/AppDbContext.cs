using Microsoft.EntityFrameworkCore;

namespace PBPropostaCredito.Data;

public class Proposta
{
    public Guid Id { get; set; }
    public Guid ClienteId { get; set; }
    public int Score { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime DataProcessamento  { get; set; }
}
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}
    public DbSet<Proposta> Propostas => Set<Proposta>();
}