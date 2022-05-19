using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Trs80.Level1Basic.CommandModels;
using Trs80.Level1Basic.VirtualMachine.Machine;

namespace Trs80.Level1Basic.Application;

public class ConsoleApp
{
    private IServiceProvider _serviceProvider;
    private Bootstrapper _bootstrapper;
    private ILogger _logger;
    private ITrs80 _trs80;

    public void Run(string workflowFileName, string workflow)
    {
        try
        {
            Startup(workflowFileName);
            RunWorkflow(workflow);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, ex.Message);
            HandleException(ex);
        }
        finally
        {
            Shutdown();
        }
    }

    private void Shutdown()
    {
        _logger.LogInformation($"{_bootstrapper.ApplicationName} exiting.");
        (_serviceProvider as IDisposable)?.Dispose();
    }

    private void HandleException(Exception ex)
    {
        _trs80.WriteLine(ex.Message);
        _trs80.WriteLine(ex.StackTrace);
    }

    private void Startup(string workflow)
    {
        try
        {
            _bootstrapper = new Bootstrapper();
            _bootstrapper.LoadWorkflow(workflow);
            _serviceProvider = _bootstrapper.ScopedServiceProvider;
            _logger = _bootstrapper.LogFactory.CreateLogger<ConsoleApp>();
            _trs80 = _serviceProvider.GetRequiredService<ITrs80>();

            _logger.LogInformation($"{_bootstrapper.ApplicationName} started.");

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private const int WorkflowTimeoutHours = 24;

    private string GetWorkflow(string workflow)
    {
        _logger.LogTrace($"({workflow})");

        if (string.IsNullOrEmpty(workflow))
            workflow = "Interpreter";

        _logger.LogTrace($"Loading Workflow: {workflow}");

        return workflow;
    }
    private void RunWorkflow(string workflow)
    {
        _logger.LogDebug($"({workflow})");

        _bootstrapper
            .WorkflowRunner
            .RunWorkflowSync(
                GetWorkflow(workflow),
                1,
                _bootstrapper.ScopedServiceProvider.GetService<WorkflowDataModel>(),
                null,
                new TimeSpan(WorkflowTimeoutHours, 0, 0)
            ).ConfigureAwait(false).GetAwaiter().GetResult();

        _bootstrapper.WorkflowHost.Stop();
    }
}