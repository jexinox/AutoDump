namespace Guts.Server.CQRS;

public interface IQueryHandler<in TQuery, TReturn>
{
    Task<TReturn> Handle(TQuery command);
}