using MediatR;

namespace Blogtify.Abstractions.Cqrs;
public interface ICommandBase { }

public interface ICommand : IRequest, ICommandBase
{
}

public interface ICommand<TResponse> : IRequest<TResponse>, ICommandBase
{
}
