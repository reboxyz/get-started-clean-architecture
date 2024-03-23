using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Gyms;
using MediatR;

namespace GymManagement.Application.Gyms.Queries.ListGyms;

public class ListGymsQueryHandler : IRequestHandler<ListGymsQuery, ErrorOr<List<Gym>>>
{
    private readonly ISubscriptionsRepository _subscriptionsRepository;
    private readonly IGymsRepository _gymsRepository;

    public ListGymsQueryHandler(
        ISubscriptionsRepository subscriptionsRepository,
        IGymsRepository gymsRepository
    )
    {
        _subscriptionsRepository = subscriptionsRepository;
        _gymsRepository = gymsRepository;
    }

    public async Task<ErrorOr<List<Gym>>> Handle(ListGymsQuery request, CancellationToken cancellationToken)
    {
        if (! await _subscriptionsRepository.ExistsAsync(request.SubscriptionId))
        {
            return Error.NotFound(description: "Subscription not found");
        }

        return await _gymsRepository.ListBySubscriptionIdAsync(request.SubscriptionId);
    }
}
