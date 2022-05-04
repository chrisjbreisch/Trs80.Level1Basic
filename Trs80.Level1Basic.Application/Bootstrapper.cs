using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NLog.Extensions.Logging;

using Trs80.Level1Basic.Command;
using Trs80.Level1Basic.Command.Commands;
using Trs80.Level1Basic.CommandModels;
using Trs80.Level1Basic.Console;
using Trs80.Level1Basic.Interpreter;
using Trs80.Level1Basic.Interpreter.Interpreter;
using Trs80.Level1Basic.Interpreter.Parser;
using Trs80.Level1Basic.Interpreter.Scanner;
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
                    .AddNLog()
                    .SetMinimumLevel(LogLevel.Debug)
        );
        _services.AddSingleton(LogFactory);
    }

    private void LogConfiguredServices()
    {
        _logger.LogTrace("Configured services = ");
        if (_services == null || _services.Count == 0)
            _logger.LogTrace("null");
        else
        {
            int penultimateIndex = _services.Count - 1;
            for (int index = 0; index < _services.Count; index++)
            {
                string separator = index < penultimateIndex ? "," : "";
                _logger.LogTrace(
                    $"{_services[index].ServiceType}: {_services[index].ImplementationType ?? _services[index].ImplementationInstance}{separator}");
            }
        }
    }

    private void GetApplicationName()
    {
        Assembly assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        ApplicationName = assembly.GetName().Name;
    }

    private void CreateServiceProvider()
    {
        RootServiceProvider = _services.BuildServiceProvider(validateScopes: true);
        IServiceScope mainScope = RootServiceProvider.CreateScope();
        ScopedServiceProvider = mainScope.ServiceProvider;
    }

    private void ConfigureServices()
    {
        ConfigureGeneralServices();
        ConfigureWorkflowSteps();
        ConfigureCommands();
        ConfigureDecorators();
    }

    private void ConfigureDecorators()
    {
        _services
            .Decorate<ICommand<SetupConsoleModel>, LogCommandDecorator<SetupConsoleModel>>()
            .Decorate<ICommand<InterpreterModel>, LogCommandDecorator<InterpreterModel>>()
            .Decorate<ICommand<ShutdownConsoleModel>, LogCommandDecorator<ShutdownConsoleModel>>();
    }

    private void ConfigureCommands()
    {
        Assembly commandAssembly = typeof(InterpreterCommand).Assembly;

        var commands =
            from type in commandAssembly.GetTypes()
            where !type.IsAbstract
            where !type.Name.StartsWith("<>")
            where type.Name.EndsWith("Command")
            from iType in type.GetInterfaces()
            where iType.Name.Contains("Command") || iType.Name.Contains("Strategy")
            select new { iType, type };

        foreach (var command in commands)
            _services.AddSingleton(command.iType, command.type);

    }

    private void ConfigureWorkflowSteps()
    {
        Assembly workflowAssembly = typeof(InterpreterStep).Assembly;

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
            .AddSingleton<IConsole, Console.Console>()
            .AddSingleton<IConsoleDataModel, ConsoleDataModel>()
            .AddSingleton<IProgram, Program>()
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
        GC.SuppressFinalize(this);
    }

    ~Bootstrapper()
    {
        Dispose(false);
    }

}