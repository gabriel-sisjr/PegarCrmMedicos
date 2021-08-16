using Microsoft.EntityFrameworkCore;

namespace PegarCrmMedicos
{
    public class ContextDB : DbContext
    {
        public DbSet<Dados> Dados { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=CrmMedicos;Trusted_Connection=True;");

    }
}
