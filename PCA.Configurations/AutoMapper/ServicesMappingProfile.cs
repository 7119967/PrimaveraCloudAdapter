namespace PCA.Configurations.AutoMapper;

public class ServicesMappingProfile: Profile
{
    public ServicesMappingProfile()
    {
        CreateMap<SubscriptionView, Subscription>();      
        CreateMap<Subscription, SubscriptionView>();
    }
}