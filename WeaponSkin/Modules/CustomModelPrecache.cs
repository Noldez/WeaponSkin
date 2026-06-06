using Microsoft.Extensions.Logging;
using Sharp.Shared.Listeners;

namespace WeaponSkin.Modules;

internal class CustomModelPrecache : IModule, IGameListener
{
    public int ListenerVersion  => IGameListener.ApiVersion;
    public int ListenerPriority => 0;

    private readonly InterfaceBridge            _bridge;
    private readonly ILogger<CustomModelPrecache> _logger;

    private static readonly List<string> CustomModels = new();

    public CustomModelPrecache(InterfaceBridge bridge, ILogger<CustomModelPrecache> logger)
    {
        _bridge = bridge;
        _logger = logger;
    }

    public bool Init()
    {
        _bridge.ModSharp.InstallGameListener(this);
        return true;
    }

    public void Shutdown()
    {
        _bridge.ModSharp.RemoveGameListener(this);
    }

    public static void RegisterModel(string modelPath)
    {
        if (!CustomModels.Contains(modelPath))
            CustomModels.Add(modelPath);
    }

    public void OnResourcePrecache()
    {
        foreach (var model in CustomModels)
        {
            try
            {
                _bridge.ModSharp.PrecacheResource(model);
                _logger.LogInformation("Precached custom model: {model}", model);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to precache {model}: {ex}", model, ex.Message);
            }
        }
    }

}
