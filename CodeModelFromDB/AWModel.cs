namespace CodeModelFromDB
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class AWModel : DbContext
    {
        public AWModel()
            : base("name=AWModel")
        {
        }

        public virtual DbSet<CreditRisk> CreditRisks { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Inventory> Inventories { get; set; }
        public virtual DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CreditRisk>()
                .Property(e => e.Timestamp)
                .IsFixedLength();

            modelBuilder.Entity<Customer>()
                .Property(e => e.Timestamp)
                .IsFixedLength();

            modelBuilder.Entity<Inventory>()
                .Property(e => e.Timestamp)
                .IsFixedLength();

            modelBuilder.Entity<Inventory>()
                .HasMany(e => e.Orders)
                .WithRequired(e => e.Inventory)
                .HasForeignKey(e => e.CarId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Order>()
                .Property(e => e.Timestamp)
                .IsFixedLength();
        }
    }
}
