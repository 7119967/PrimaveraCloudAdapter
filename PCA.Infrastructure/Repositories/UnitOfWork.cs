namespace PCA.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _dbContext;
    private readonly IActivityRelationshipRepository _activityRelationshipRepository = null!;
    private readonly IActivityRepository _activityRepository = null!;
    private readonly ICalendarChangeRepository _calendarChangeRepository = null!;
    private readonly IEventNotificationRepository _eventNotificationRepository = null!;
    private readonly IObjectTypeRepository _objectTypeRepository = null!;
    private readonly IProjectRepository _projectRepository = null!;
    private readonly IProjectBudgetRepository _projectBudgetRepository = null!;
    private readonly IResourceAssignmentRepository _resourceAssignmentRepository = null!;
    private readonly IResourceRepository _resourceRepository = null!;
    private readonly ISubscriptionRepository _subscriptionRepository = null!;
    private readonly ITransactionRepository _transactionRepository = null!;
    private readonly IWbsRepository _wbsRepository = null!;

    public UnitOfWork()
    {
        
    }
    
    public UnitOfWork(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IActivityRelationshipRepository ActivityRelationshipRepository => _activityRelationshipRepository ?? new ActivityRelationshipRepository(_dbContext);
    public IActivityRepository ActivityRepository => _activityRepository ?? new ActivityRepository(_dbContext);
    public ICalendarChangeRepository CalendarChangeRepository => _calendarChangeRepository ?? new CalendarChangeRepository(_dbContext);
    public IEventNotificationRepository EventNotificationRepository => _eventNotificationRepository ?? new EventNotificationRepository(_dbContext);
    public IObjectTypeRepository ObjectTypeRepository => _objectTypeRepository ?? new ObjectTypeRepository(_dbContext);
    public IProjectRepository ProjectRepository => _projectRepository ?? new ProjectRepository(_dbContext);
    public IProjectBudgetRepository ProjectBudgetRepository => _projectBudgetRepository ?? new ProjectBudgetRepository(_dbContext);
    public IResourceAssignmentRepository ResourceAssignmentRepository => _resourceAssignmentRepository ?? new ResourceAssignmentRepository(_dbContext);
    public IResourceRepository ResourceRepository => _resourceRepository ?? new ResourceRepository(_dbContext);
    public ISubscriptionRepository SubscriptionRepository => _subscriptionRepository ?? new SubscriptionRepository(_dbContext);
    public ITransactionRepository TransactionRepository => _transactionRepository ?? new TransactionRepository(_dbContext);
    public IWbsRepository WbsRepository => _wbsRepository ?? new WbsRepository(_dbContext);


    public void Commit()
    {
        try
        {
            _dbContext.SaveChanges();
        }

        catch (DbUpdateConcurrencyException e)
        {
            Console.WriteLine($"{e.GetType().FullName}: {e.Message}");
        }
    }

    public async Task CommitAsync()
    {
        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.GetType().FullName}: {e.Message}");
        }
    }
}