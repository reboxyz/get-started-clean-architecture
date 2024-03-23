using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using MediatR;

namespace GymManagement.Application.Gyms.Commands.DeleteGym;

public class DeleteGymCommandHandler : IRequestHandler<DeleteGymCommand, ErrorOr<Deleted>>
{
    private readonly ISubscriptionsRepository _subscriptionsRepository;
    private readonly IGymsRepository _gymsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteGymCommandHandler(
        ISubscriptionsRepository subscriptionsRepository,
        IGymsRepository gymsRepository,
        IUnitOfWork unitOfWork
    )
    {
        _subscriptionsRepository = subscriptionsRepository;
        _gymsRepository = gymsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Deleted>> Handle(DeleteGymCommand command, CancellationToken cancellationToken)
    {
        var gym = await _gymsRepository.GetByIdAsync(command.GymId);

        if (gym is null)
        {
            return Error.NotFound(description: "Gym not found");
        }

        var subscription = await _subscriptionsRepository.GetByIdAsync(command.SubscriptionId);

        if (subscription is null)
        {
            return Error.NotFound(description: "Subscription not found");
        }

        if (!subscription.HasGym(gym.Id))        
        {
            return Error.Unexpected(description: "Gym not found");
        }

        subscription.RemoveGym(gym.Id);

        await _subscriptionsRepository.UpdateAsync(subscription);
        await _gymsRepository.RemoveGymAsync(gym);
        await _unitOfWork.CommitChangesAsync();
        
        return Result.Deleted;
    }
}
