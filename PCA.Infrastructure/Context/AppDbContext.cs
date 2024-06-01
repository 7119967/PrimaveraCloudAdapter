namespace PCA.Infrastructure.Context;

public class AppDbContext : DbContext
{
    public DbSet<Activity> Activities { get; set; } = null!;
    public DbSet<ActivityRelationship> ActivityRelationships { get; set; } = null!;
    public DbSet<CalendarChange> CalendarChanges { get; set; } = null!;
    public DbSet<EventNotification> EventNotifications { get; set; } = null!;
    public DbSet<ObjectType> ObjectTypes { get; set; } = null!;
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<ProjectBudget> ProjectBudgets { get; set; } = null!;
    public DbSet<Resource> Resources { get; set; } = null!;
    public DbSet<ResourceAssignment> ResourceAssignments { get; set; } = null!;
    public DbSet<Subscription> Subscriptions { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;
    public DbSet<Wbs> Wbs { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Activity>()
            .HasOne(a => a.Transaction)
            .WithMany(t => t.Activities)
            .HasForeignKey(a => a.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ActivityRelationship>()
            .HasOne(a => a.Transaction)
            .WithMany(t => t.ActivityRelationships)
            .HasForeignKey(a => a.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CalendarChange>()
            .HasOne(a => a.Transaction)
            .WithMany(t => t.CalendarChanges)
            .HasForeignKey(a => a.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Project>()
            .HasOne(a => a.Transaction)
            .WithMany(t => t.Projects)
            .HasForeignKey(a => a.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProjectBudget>()
            .HasOne(a => a.Transaction)
            .WithMany(t => t.ProjectBudgets)
            .HasForeignKey(a => a.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Resource>()
            .HasOne(a => a.Transaction)
            .WithMany(t => t.Resources)
            .HasForeignKey(a => a.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ResourceAssignment>()
            .HasOne(a => a.Transaction)
            .WithMany(t => t.ResourceAssignments)
            .HasForeignKey(a => a.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);        
        
        modelBuilder.Entity<Wbs>()
            .HasOne(a => a.Transaction)
            .WithMany(t => t.Wbses)
            .HasForeignKey(a => a.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
