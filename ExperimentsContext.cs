using System.Data.Entity;

namespace Experiments
{
    public class ExperimentsContext : DbContext 
    {
        public ExperimentsContext() : base("DbConnection")  
        {
        }

        public DbSet<Data>? Data { get; set; }  
        public DbSet<Experiment>? Experiments { get; set; }  
    }
}
