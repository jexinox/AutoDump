namespace AutoDump.Server.CQRS;

public interface ICommandHandler<in TCommand, TReturn>
{
    Task<TReturn> Handle(TCommand command);
}