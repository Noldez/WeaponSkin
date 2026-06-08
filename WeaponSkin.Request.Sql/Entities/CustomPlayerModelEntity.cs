using SqlSugar;

namespace WeaponSkin.Request.Sql.Entities;

[SugarTable("ws_custom_player_models")]
[SugarIndex($"unique_{{table}}_{nameof(SteamId)}",
    nameof(SteamId), OrderByType.Asc, IsUnique = true)]
public class CustomPlayerModelEntity
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    [SugarColumn(IsNullable = false)]
    public ulong SteamId { get; set; }

    [SugarColumn(IsNullable = false, Length = 512)]
    public string ModelPath { get; set; } = string.Empty;
}
