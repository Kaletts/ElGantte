using ElGantte.Models;
using Microsoft.EntityFrameworkCore;

namespace ElGantte.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios => Set<Usuario>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }

}
