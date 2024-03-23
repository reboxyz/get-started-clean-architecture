using ErrorOr;
using GymManagement.Application.Subscriptions.Commands.CreateSubscription;
using GymManagement.Application.Subscriptions.Queries.GetSubscription;
using GymManagement.Contracts.Subscriptions;
using GymManagement.Domain.Subscriptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using DomainSubscriptionType = GymManagement.Domain.Subscriptions.SubscriptionType;       // alias syntax
using ContractSubscriptionType = GymManagement.Contracts.Subscriptions.SubscriptionType;  // alias syntax
using GymManagement.Application.Subscriptions.Commands.DeleteSubscription;  

namespace GymManagement.Api.Controllers.Subscriptions;

[Route("[controller]")]
public class SubscriptionsController: ApiController
{
    private readonly ISender _mediator; // IMediator
    

    public SubscriptionsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSubscription(CreateSubscriptionRequest request)
    {
        if (!DomainSubscriptionType.TryFromName(request.SubscriptionType.ToString(), out var subscriptionType))
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: "Invalid subscription type");
        }

        var command = new CreateSubscriptionCommand(
            subscriptionType,
            request.AdminId
        );

        ErrorOr<Subscription> subscriptionResult = await _mediator.Send(command);

        //if (subscriptionResult.IsError)
        //{
        //    return Problem();
        //}

        //var response = new SubscriptionResponse(subscriptionResult.Value, request.SubscriptionType);

        //return Ok(response);
        
        return subscriptionResult.MatchFirst(
            subscription => CreatedAtAction(
                nameof(GetSubscription),
                new { subscriptionId = subscription.Id },
                new SubscriptionResponse(
                    subscription.Id,
                    ToDto(subscription.SubscriptionType))),
            errors => Problem(errors)  
        );
    }

    [HttpGet("{subscriptionId:guid}")]
    public async Task<IActionResult> GetSubscription(Guid subscriptionId)
    {
        var query = new GetSubscriptionQuery(subscriptionId);

        var getSubscriptionResult = await _mediator.Send(query);

        return getSubscriptionResult.MatchFirst(
            subscription => Ok(new SubscriptionResponse(
                subscription.Id, 
                ToDto(subscription.SubscriptionType)
                )),
            errors => Problem(errors)  
        );
    }

    [HttpDelete("{subscriptionId:guid}")]
    public async Task<IActionResult> DeleteSubscription(Guid subscriptionId)
    {
        var command = new DeleteSubscriptionCommand(subscriptionId);

        var deleteSubscriptionResult = await _mediator.Send(command);

        return deleteSubscriptionResult.Match<IActionResult>(
            _ => NoContent(),
            errors => Problem(errors)  
        );
    }

    private static ContractSubscriptionType ToDto(DomainSubscriptionType subscriptionType)
    {
        return subscriptionType.Name switch
        {
            nameof(DomainSubscriptionType.Free) => ContractSubscriptionType.Free,
            nameof(DomainSubscriptionType.Starter) => ContractSubscriptionType.Starter,
            nameof(DomainSubscriptionType.Pro) => ContractSubscriptionType.Pro,
            _ => throw new InvalidOperationException(),
        };
    }
}
