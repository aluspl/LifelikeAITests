using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AiAgent.Api.Infrastructure.CQRS.Services;

public class Mediator(IServiceProvider serviceProvider) : IMediator
{
    public async Task SendAsync(ICommand command)
    {
        var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
        dynamic handler = serviceProvider.GetRequiredService(handlerType);
        await handler.HandleAsync((dynamic)command);
    }

    public async Task<TResult> SendAsync<TResult>(ICommand<TResult> command)
    {
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));
        dynamic handler = serviceProvider.GetRequiredService(handlerType);
        return await handler.HandleAsync((dynamic)command);
    }

    public async Task<TResult> QueryAsync<TResult>(IQuery<TResult> query)
    {
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
        dynamic handler = serviceProvider.GetRequiredService(handlerType);
        return await handler.HandleAsync((dynamic)query);
    }
}