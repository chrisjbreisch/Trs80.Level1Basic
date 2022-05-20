using Microsoft.Extensions.Logging;
using Trs80.Level1Basic.Common.Extensions;

namespace Trs80.Level1Basic.Command.Commands;

public class LogCommandDecorator<TPo> : ICommand<TPo>
{
    private readonly ICommand<TPo> _command;
    private readonly ILogger _logger;

    public LogCommandDecorator(
        ILoggerFactory logFactory,
        ICommand<TPo> command)
    {
        _command = command ?? throw new ArgumentNullException(nameof(command));
        if (logFactory == null) throw new ArgumentNullException(nameof(logFactory));

        _logger = logFactory.CreateLogger<LogCommandDecorator<TPo>>();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Usage", 
        "CA2254:Template should be a static expression", 
        Justification = "Minimal performance gain not worth effort")]
    public void Execute(TPo parameterObject)
    {
        try
        {
            _logger.LogInformation(
                $"\r\nExecuting {_command.GetType().Name} ()");
            _command.Execute(parameterObject);
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"\r\nCommand failure for {_command.GetType().Name}.", ex);
            throw;
        }
    }
}
