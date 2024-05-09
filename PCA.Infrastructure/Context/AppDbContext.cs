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

    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    { 
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }
}
