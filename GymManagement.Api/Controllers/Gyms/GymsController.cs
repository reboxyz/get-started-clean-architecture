using GymManagement.Application.Gyms.Commands.AddTrainer;
using GymManagement.Application.Gyms.Commands.CreateGym;
using GymManagement.Application.Gyms.Commands.DeleteGym;
using GymManagement.Application.Gyms.Queries.GetGym;
using GymManagement.Application.Gyms.Queries.ListGyms;
using GymManagement.Contracts.Gyms;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.Api.Controllers.Gyms;

[Route("subscriptions/{subscriptionId:guid}/gyms")]
public class GymsController: ApiController
{
    private readonly ISender _mediator;

    public GymsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateGym
    (
        CreateGymRequest request,
        Guid subscriptionId
    )
    {
        var command = new CreateGymCommand(request.Name, subscriptionId);

        var createGymResult = await _mediator.Send(command);

        return createGymResult.Match<IActionResult>
        (
            gym => CreatedAtAction(
                nameof(GetGym),
                new {subscriptionId, gymId = gym.Id},
                new GymResponse(gym.Id, gym.Name)),
            errors => Problem(errors)  
        );
    }

    [HttpGet("{gymId:guid}")]
    public async Task<IActionResult> GetGym(Guid subscriptionId, Guid gymId)
    {
        var command = new GetGymQuery(subscriptionId, gymId);

        var getGymResult = await _mediator.Send(command);

        return getGymResult.Match<IActionResult>(
            gym => Ok(new GymResponse(gym.Id, gym.Name)),
            errors => Problem(errors)
        );
    }

    [HttpDelete("{gymId:guid}")]
    public async Task<IActionResult> DeleteGym(Guid subscriptionId, Guid gymId)
    {
        var command = new DeleteGymCommand(subscriptionId, gymId);

        var deleteGymResult = await _mediator.Send(command);

        return deleteGymResult.Match<IActionResult>(
            _ => NoContent(),
            errors => Problem(errors)
        );
    }

    [HttpGet]
    public async Task<IActionResult> ListGyms(Guid subscriptionId)
    {
        var command = new ListGymsQuery(subscriptionId);

        var listGymsResult = await _mediator.Send(command);

        return listGymsResult.Match<IActionResult>(
            gyms => Ok(gyms.ConvertAll(gym => new GymResponse(gym.Id, gym.Name))),
            errors => Problem(errors)
        );
    }

    [HttpPost("{gymId:guid}/trainers")]
    public async Task<IActionResult> AddTrainer(AddTrainerRequest request, Guid subscriptionId, Guid gymId)
    {
        var command = new AddTrainerCommand(subscriptionId, gymId, request.TrainerId);

        var addTrainerResult = await _mediator.Send(command);

        return addTrainerResult.MatchFirst<IActionResult>(
            success => Ok(),
            errors => Problem(errors)
        );
    }

}
