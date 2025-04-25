using Averbacao.modules.Inss.Domain.Propostas;
using Averbacao.modules.Inss.Domain.Propostas.EfMapping;
using Microsoft.EntityFrameworkCore;

namespace Averbacao.modules.Inss.Infraestructure.DbContext;

public class AverbacaoDbContext(DbContextOptions<AverbacaoDbContext> options) : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<Proposta> Propostas { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    { 
        modelBuilder.ApplyConfiguration(new PropostasEfMapping());
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