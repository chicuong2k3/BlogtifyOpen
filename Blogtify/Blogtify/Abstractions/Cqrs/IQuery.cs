using MediatR;

namespace Blogtify.Abstractions.Cqrs;

public interface IQuery<TResponse> : IRequest<TResponse>
{
}