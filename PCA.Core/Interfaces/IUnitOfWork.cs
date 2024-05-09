namespace PCA.Core.Interfaces;

public interface IUnitOfWork
{
    IActivityRelationshipRepository ActivityRelationshipRepository { get; }
    IActivityRepository ActivityRepository { get; }
    ICalendarChangeRepository CalendarChangeRepository { get; }
    IEventNotificationRepository EventNotificationRepository { get; }
    IObjectTypeRepository ObjectTypeRepository { get; }
    IProjectRepository ProjectRepository { get; }
    IProjectBudgetRepository ProjectBudgetRepository { get; }
    IResourceAssignmentRepository ResourceAssignmentRepository { get; }
    IResourceRepository ResourceRepository { get; }
    ISubscriptionRepository SubscriptionRepository { get; }
    ITransactionRepository TransactionRepository { get; }
    IWbsRepository WbsRepository { get; }

    void Commit();
    Task CommitAsync();
}
