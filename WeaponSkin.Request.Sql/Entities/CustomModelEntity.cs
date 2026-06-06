using SqlSugar;

namespace WeaponSkin.Request.Sql.Entities;

[SugarTable("ws_custom_models")]
[SugarIndex($"unique_{{table}}_{nameof(SteamId)}_{nameof(ItemId)}",
    nameof(SteamId), OrderByType.Asc,
    nameof(ItemId), OrderByType.Asc, IsUnique = true)]
public class CustomModelEntity
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    [SugarColumn(IsNullable = false)]
    public ulong SteamId { get; set; }

    [SugarColumn(IsNullable = false)]
    public int ItemId { get; set; }

    [SugarColumn(IsNullable = false, Length = 512)]
    public string ModelPath { get; set; } = string.Empty;
}
