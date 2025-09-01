namespace AiAgent.Api.Infrastructure.CQRS.Interfaces;

public interface IMediator
{
    Task SendAsync(ICommand command);
    Task<TResult> SendAsync<TResult>(ICommand<TResult> command);
    Task<TResult> QueryAsync<TResult>(IQuery<TResult> query);
}