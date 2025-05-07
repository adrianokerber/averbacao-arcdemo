using AverbacaoService.Domain.Averbacoes;
using AverbacaoService.Domain.Averbacoes.EfMapping;
using Microsoft.EntityFrameworkCore;

namespace AverbacaoService.shared.DatabaseDetails;

public class AverbacaoDbContext(DbContextOptions<AverbacaoDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
{
    public DbSet<Averbacao> Averbacoes { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    { 
        modelBuilder.ApplyConfiguration(new AverbacoesEfMapping());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return result;
        }
        catch (DbUpdateException e)
        {
            throw new Exception("Erro ao atualizar o banco de dados.", e);
        }
        catch (Exception ex)
        {
            throw new Exception("Erro inesperado.", ex);
        }
    }
}