namespace PCA.Infrastructure.Repositories;

public class CalendarChangeRepository: GenericRepository<CalendarChange>, ICalendarChangeRepository
{
    public CalendarChangeRepository(AppDbContext context) : base(context)
    {

    }
}