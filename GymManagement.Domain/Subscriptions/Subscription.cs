using Throw;
using ErrorOr;
using GymManagement.Domain.Gyms;

namespace GymManagement.Domain.Subscriptions;

// Note! In the Domain Driven Design, creation of Subscription schema is delegated to another class which is "SubscriptionConfiguration".
// If you adhere to DDD, then this is the case. But if DDD is not strictly followed, you may used the Domain object as the Entity schema.
public class Subscription
{
    private readonly List<Guid> _gymIds = new();
    private readonly int _maxGyms;
    public Guid AdminId { get; }
    public Guid Id { get; private set; }
    public SubscriptionType SubscriptionType { get; private set; } = null!;

    public Subscription(SubscriptionType subscriptionType,
        Guid adminId,
        Guid? id = null)
    {
        SubscriptionType = subscriptionType;
        AdminId = adminId;
        Id = id ?? Guid.NewGuid();

        _maxGyms = GetMaxGyms();
    }

    public ErrorOr<Success> AddGym(Gym gym)
    {
        _gymIds.Throw().IfContains(gym.Id);

        if (_gymIds.Count >= _maxGyms)
        {
            return SubscriptionErrors.CannotHaveMoreGymsThanSubscriptionAllows;
        }

        return Result.Success;
    }

    public int GetMaxGyms() => SubscriptionType.Name switch
    {
        nameof(SubscriptionType.Free) => 1,
        nameof(SubscriptionType.Starter) => 1,
        nameof(SubscriptionType.Pro) => 3,
        _ => throw new InvalidOperationException()
    };

    public int GetMaxRooms() => SubscriptionType.Name switch
    {
        nameof(SubscriptionType.Free) => 1,
        nameof(SubscriptionType.Starter) => 3,
        nameof(SubscriptionType.Pro) => int.MaxValue,
        _ => throw new InvalidOperationException()
    };

    public int GetMaxDailySessions() => SubscriptionType.Name switch
    {
        nameof(SubscriptionType.Free) => 4,
        nameof(SubscriptionType.Starter) => int.MaxValue,
        nameof(SubscriptionType.Pro) => int.MaxValue,
        _ => throw new InvalidOperationException()
    };

    public bool HasGym(Guid gymId)
    {
        return _gymIds.Contains(gymId);
    }

    public void RemoveGym(Guid gymId)
    {
        _gymIds.Throw().IfNotContains(gymId);
        _gymIds.Remove(gymId);
    }

    // Note! "Private" is to ensure that Domains are DB agnostics.
    private Subscription()
    {

    }
}
