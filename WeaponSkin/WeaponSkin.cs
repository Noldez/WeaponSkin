using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sharp.Extensions.CommandManager;
using Sharp.Shared;
using Sharp.Shared.Abstractions;
using WeaponSkin.Managers;
using WeaponSkin.Modules;

[assembly: DisableRuntimeMarshalling]

namespace WeaponSkin;

public class WeaponSkin : IModSharpModule
{
    string IModSharpModule.DisplayName   => "WeaponSkin";
    string IModSharpModule.DisplayAuthor => "Nukoooo";

    private readonly ISharedSystem        _shared;
    private readonly ServiceProvider      _serviceProvider;
    private readonly ILogger<WeaponSkin> _logger;

    public WeaponSkin(
        ISharedSystem  sharedSystem,
        string         dllPath,
        string         sharpPath,
        Version        version,
        IConfiguration configuration,
        bool           hotReload)
    {
        _shared = sharedSystem;
        var factory = sharedSystem.GetLoggerFactory();
        _logger = factory.CreateLogger<WeaponSkin>();

        var bridge = new InterfaceBridge(dllPath,
                                         sharpPath,
                                         version,
                                         sharedSystem,
                                         hotReload,
                                         sharedSystem.GetModSharp()
                                                     .HasCommandLine("-debug"));

        var services = new ServiceCollection();
        services.AddSingleton(bridge);
        services.AddSingleton(factory);
        services.AddSingleton(sharedSystem);
        services.AddLogging();
        services.AddCommandManager(sharedSystem);

        ConfigureServices(services);

        _serviceProvider = services.BuildServiceProvider();
    }

    public bool Init()
    {
        foreach (var service in _serviceProvider.GetServices<IManager>())
        {
            if (service.Init())
            {
                _logger.LogInformation("{service} Initialized", service.GetType().FullName);

                continue;
            }

            _logger.LogError("Failed to init {service}!", service.GetType().FullName);

            return false;
        }

        foreach (var service in _serviceProvider.GetServices<IModule>())
        {
            if (service.Init())
            {
                _logger.LogInformation("{service} Initialized", service.GetType().FullName);

                continue;
            }

            _logger.LogError("Failed to init {service}!", service.GetType().FullName);

            return false;
        }

        foreach (var service in _serviceProvider.GetServices<IManager>())
        {
            try
            {
                service.OnPostInit(_serviceProvider);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error when calling PostInit for {service}", service.GetType().FullName);
            }
        }

        foreach (var service in _serviceProvider.GetServices<IModule>())
        {
            try
            {
                service.OnPostInit(_serviceProvider);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error when calling PostInit for {service}", service.GetType().FullName);
            }
        }

        _serviceProvider.LoadAllSharpExtensions();

        RegisterCustomModels();

        return true;
    }

    private void RegisterCustomModels()
    {
        try
        {
            var bridge = _serviceProvider.GetRequiredService<InterfaceBridge>();
            var configPath = System.IO.Path.Combine(bridge.SharpPath, "configs", "custom_models.json");

            _logger.LogInformation("Looking for custom_models.json at: {path}", configPath);

            if (!System.IO.File.Exists(configPath))
            {
                _logger.LogWarning("custom_models.json not found at: {path}", configPath);
                return;
            }

            var json = System.IO.File.ReadAllText(configPath);
            var models = System.Text.Json.JsonSerializer.Deserialize<string[]>(json);
            if (models is null) return;

            foreach (var model in models)
                CustomModelPrecache.RegisterModel(model);

            _logger.LogInformation("Registered {count} custom model(s) for precaching", models.Length);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Failed to load custom_models.json: {ex}", ex.Message);
        }
    }

    public void Shutdown()
    {
        foreach (var service in _serviceProvider.GetServices<IManager>())
        {
            try
            {
                service.Shutdown();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error when calling Shutdown for {service}", service.GetType().FullName);
            }
        }

        foreach (var service in _serviceProvider.GetServices<IModule>())
        {
            try
            {
                service.Shutdown();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error when calling Shutdown for {service}", service.GetType().FullName);
            }
        }

        _serviceProvider.ShutdownAllSharpExtensions();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddManagerDi();
        services.AddModuleDi();
    }
}
