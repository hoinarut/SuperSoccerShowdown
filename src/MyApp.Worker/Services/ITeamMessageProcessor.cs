namespace MyApp.Worker.Services;

public interface ITeamMessageProcessor
{
    Task ProcessAsync(string messageBody, CancellationToken cancellationToken);
}
