using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Trs80.Level1Basic.Command.Commands;
using Trs80.Level1Basic.CommandModels;
using Trs80.Level1Basic.Graphics;
using Trs80.Level1Basic.Services;
using Trs80.Level1Basic.Services.Interpreter;
using Trs80.Level1Basic.Services.Parser;
using Trs80.Level1Basic.Workflow;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using WorkflowCore.Services.DefinitionStorage;

namespace Trs80.Level1Basic.Application;

public sealed class Bootstrapper : IDisposable
{
    private bool _isDisposed;
    private IServiceCollection _services;
    private readonly ILogger _logger;

    public string ApplicationName { get; private set; }
    public ILoggerFactory LogFactory;

    public IServiceProvider ScopedServiceProvider { get; private set; }
    public ServiceProvider RootServiceProvider { get; private set; }
    public IWorkflowHost WorkflowHost => ScopedServiceProvider.GetRequiredService<IWorkflowHost>();
    public ISyncWorkflowRunner WorkflowRunner => ScopedServiceProvider.GetRequiredService<ISyncWorkflowRunner>();
    private IDefinitionLoader WorkflowLoader => ScopedServiceProvider.GetRequiredService<IDefinitionLoader>();



    public Bootstrapper(string workflowFileName)
    {
        _services = new ServiceCollection();
        ConfigureServicesExtensions();

        ConfigureLogging();
        ConfigureServices();
        CreateServiceProvider();

        LoadWorkflow(workflowFileName);

        GetApplicationName();

        _logger = LogFactory.CreateLogger<Bootstrapper>();

        LogConfiguredServices();
    }

    private void LoadWorkflow(string workflowFileName)
    {
        if (string.IsNullOrEmpty(workflowFileName)) return;

        WorkflowLoader.LoadDefinition(File.ReadAllText(workflowFileName), Deserializers.Json);

        WorkflowHost.OnStepError += WorkflowHost_OnStepError;
        WorkflowHost.Start();
    }

    private void WorkflowHost_OnStepError(WorkflowInstance workflow, WorkflowStep step, Exception exception)
    {
        string baseErrorMessage = $"Error in workflow {workflow?.Id}, step {step?.Id}. Aborting.";

        var logContext = new Dictionary<string, object>
        {
            { "WorkflowId", workflow?.Id ?? "[unknown]" },
            { "StepId", step?.Id.ToString() ?? "[unknown]" }
        };

        _logger.LogCritical(exception, baseErrorMessage, logContext);
    }

    private void ConfigureServicesExtensions()
    {
        _services.AddLogging();
        _services.AddWorkflow();
        _services.AddWorkflowDSL();
    }

    private void ConfigureLogging()
    {
        LogFactory = LoggerFactory.Create(
            builder =>
                builder
                    .ClearProviders()
                    .AddDebug()
                    .SetMinimumLevel(LogLevel.Trace)
        );
    }

    private void LogConfiguredServices()
    {
        _logger.LogDebug("Configured services = ");
        if (_services == null || _services.Count == 0)
            _logger.LogDebug("null");
        else
        {
            int penultimateIndex = _services.Count - 1;
            for (int index = 0; index < _services.Count; index++)
            {
                string separator = index < penultimateIndex ? "," : "";
                _logger.LogDebug(
                    $"{_services[index].ServiceType}: {_services[index].ImplementationType ?? _services[index].ImplementationInstance}{separator}");
            }
        }
    }

    private void GetApplicationName()
    {
        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        ApplicationName = assembly.GetName().Name;
    }

    private void CreateServiceProvider()
    {
        RootServiceProvider = _services.BuildServiceProvider(validateScopes: true);
        var mainScope = RootServiceProvider.CreateScope();
        ScopedServiceProvider = mainScope.ServiceProvider;
    }

    private void ConfigureServices()
    {
        ConfigureGeneralServices();
        ConfigureWorkflowSteps();
        ConfigureCommands();
        ConfigureStrategies();
        ConfigureDecorators();
    }

    private void ConfigureDecorators()
    {
        //_services
        //    .Decorate<ICommand<InboundModel>, LogCommandDecorator<InboundModel>>()
        //    .Decorate<ICommand<OutboundModel>, LogCommandDecorator<OutboundModel>>()
        //    .Decorate<ICommand<WorkingConsoleModel>, LogCommandDecorator<WorkingConsoleModel>>();
    }

    private void ConfigureStrategies()
    {
        // no strategies as of yet
    }

    private void ConfigureCommands()
    {
        var commandAssembly = typeof(InterpreterCommand).Assembly;

        var commands =
            from type in commandAssembly.GetTypes()
            where !type.IsAbstract
            where !type.Name.StartsWith("<>")
            where type.Name.EndsWith("Command")
            from iType in type.GetInterfaces()
            where iType.Name.Contains("Command") || iType.Name.Contains("Strategy")
            select new { iType, type };

        foreach (var command in commands)
            _services.AddTransient(command.iType, command.type);

    }

    private void ConfigureWorkflowSteps()
    {
        var workflowAssembly = typeof(InterpreterStep).Assembly;

        var workflowSteps =
            from type in workflowAssembly.GetTypes()
            where !type.IsAbstract
            where !type.Name.StartsWith("<>")
            where !type.Name.Contains("Strategy")
            from iType in type.GetInterfaces()
            where iType.Name == "I" + type.Name
            select new { iType, type };

        foreach (var workflowStep in workflowSteps)
            _services.AddSingleton(workflowStep.iType, workflowStep.type);
    }

    private void ConfigureGeneralServices()
    {
        _services
            .AddSingleton<IScanner, Scanner>()
            .AddSingleton<IParser, Parser>()
            .AddSingleton<IBuiltinFunctions, BuiltinFunctions>()
            .AddSingleton<IBasicInterpreter, BasicInterpreter>()
            .AddSingleton<IBasicEnvironment, BasicEnvironment>()
            .AddTransient<ITrs80Console, Trs80Console>()
            .AddSingleton<IScreen, Screen>()
            .AddSingleton<ISharedDataModel, SharedDataModel>()
            .BuildServiceProvider();
    }

    private void Dispose(bool disposing)
    {
        if (_isDisposed) return;

        if (disposing)
        {
            if (RootServiceProvider != null)
            {
                RootServiceProvider.Dispose();
                RootServiceProvider = null;
            }
            (ScopedServiceProvider as IDisposable)?.Dispose();
            ScopedServiceProvider = null;
        }

        if (_services != null)
        {
            _services.Clear();
            _services = null;
        }

        _isDisposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
    }

    ~Bootstrapper()
    {
        Dispose(false);
    }

}