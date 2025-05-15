using AverbacaoService.Infrastructure.EfContextConfiguration.Interfaces;

namespace AverbacaoService.Infrastructure.EfContextConfiguration
{
    [Obsolete]
    public sealed class AverbacaoDbContextAccessor: IEfDbContextAccessor<AverbacaoDbContext>
    {
        private AverbacaoDbContext _contexto = null!;
        private bool _disposed = false;

        public AverbacaoDbContext Get()
        {
            return _contexto ?? throw new InvalidOperationException("Contexto deve ser registrado!");
        }

        public void Register(AverbacaoDbContext context)
        {
            _disposed = false;
            _contexto = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Clear()
        {
            Dispose(true);
        }

        public void Dispose()
        {
            Dispose(true);
            // ReSharper disable once GCSuppressFinalizeForTypeWithoutDestructor
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                _contexto?.Dispose();

            _contexto = null!;
            _disposed = true;
        }
    }
}