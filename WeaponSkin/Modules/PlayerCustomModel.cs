using Microsoft.Extensions.Logging;
using Sharp.Shared.HookParams;
using Sharp.Shared.Units;
using WeaponSkin.Managers;

namespace WeaponSkin.Modules;

internal class PlayerCustomModel : IModule
{
    private readonly InterfaceBridge            _bridge;
    private readonly IPlayerInfoManager         _playerInfo;
    private readonly ILogger<PlayerCustomModel> _logger;
    private readonly string?[]                  _appliedModels = new string?[PlayerSlot.MaxPlayerCount];

    public PlayerCustomModel(InterfaceBridge bridge, IPlayerInfoManager playerInfo, ILogger<PlayerCustomModel> logger)
    {
        _bridge     = bridge;
        _playerInfo = playerInfo;
        _logger     = logger;
    }

    public bool Init()
    {
        _bridge.HookManager.PlayerSpawnPost.InstallForward(OnPlayerSpawnPost);
        _bridge.HookManager.PlayerPostThink.InstallForward(OnPlayerPostThink);
        return true;
    }

    public void Shutdown()
    {
        _bridge.HookManager.PlayerSpawnPost.RemoveForward(OnPlayerSpawnPost);
        _bridge.HookManager.PlayerPostThink.RemoveForward(OnPlayerPostThink);
    }

    private void OnPlayerSpawnPost(IPlayerSpawnForwardParams @params)
    {
        var client = @params.Client;
        if (client.IsFakeClient) return;

        _appliedModels[client.Slot] = null;

        var model = _playerInfo.GetPlayerCustomModel(client);
        if (model is null) return;

        var pawn = @params.Pawn;
        if (pawn is not { IsValidEntity: true }) return;

        pawn.SetModel(model);

        _logger.LogInformation("SetModel (spawn) for {name}: {model}", client.Name, model);
    }

    private void OnPlayerPostThink(IPlayerThinkForwardParams @params)
    {
        var client = @params.Client;
        if (client.IsFakeClient) return;

        var slot  = (int)client.Slot;
        var model = _playerInfo.GetPlayerCustomModel(client);

        if (model == _appliedModels[slot]) return;

        _appliedModels[slot] = model;

        if (model is null) return;

        var pawn = @params.Pawn;
        if (pawn is not { IsValidEntity: true }) return;

        pawn.SetModel(model);

        _logger.LogInformation("SetModel (postthink) for {name}: {model}", client.Name, model);
    }
}
