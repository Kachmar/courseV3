using Models.Models;

namespace DataAccess.EF
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    public class UniversityContext : DbContext
    {
        private readonly IOptions<RepositoryOptions> options;

        public UniversityContext(IOptions<RepositoryOptions> options)
        {
            this.options = options;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(options.Value.DefaultConnectionString);
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }


        public DbSet<Student> Students { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<HomeTask> HomeTasks { get; set; }

    }
}