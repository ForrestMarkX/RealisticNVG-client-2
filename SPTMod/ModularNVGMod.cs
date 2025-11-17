using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;

namespace ModularNVG_4_0;

public record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; init; } = "com.Borkel.ModularNVG";
    public override string Name { get; init; } = "ModularNVG";
    public override string Author { get; init; } = "Borkel";
    public override List<string>? Contributors { get; init; }
    public override SemanticVersioning.Version Version { get; init; } = new("1.0.0");
    public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.0");
    public override List<string>? Incompatibilities { get; init; }
    public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public override string? Url { get; init; }
    public override bool? IsBundleMod { get; init; }
    public override string License { get; init; } = "None";
}

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class AfterDBLoadHook(
    ISptLogger<AfterDBLoadHook> logger,
    DatabaseService databaseService)
    : IOnLoad
{
    public Task OnLoad()
    {
        const string N15_ADAPTER_ID = "5c0695860db834001b735461";
        const string PNV10T_NVG_ID = "5c066e3a0db834001b7353f0";

        var itemsDB = databaseService.GetTables().Templates.Items;
        if( itemsDB.ContainsKey(N15_ADAPTER_ID) )
        {
            var n15Adapter = itemsDB[N15_ADAPTER_ID];
            var slots = n15Adapter.Properties?.Slots;
            if( slots?.Any() == true )
            {
                var firstSlot = slots.FirstOrDefault(); 
                var slotProperties = firstSlot?.Properties;
                var slotFilterCollection = slotProperties?.Filters?.FirstOrDefault();

                if( slotFilterCollection != null )
                {
                    if (!slotFilterCollection.Filter?.Contains(PNV10T_NVG_ID) ?? false)
                    {
                        slotFilterCollection.Filter?.Add(PNV10T_NVG_ID);
                        logger.Info($"[SBNV]: Your N-15s can also be mounted on helmets now!");
                    }
                }
                else
                {
                    logger.Error($"[SBNV]: Failed to find slot filter collection on adapter {N15_ADAPTER_ID}.");
                }
            }
            else
            {
                logger.Error($"[SBNV]: Adapter {N15_ADAPTER_ID} has no slots.");
            }
        }
        else
        {
            logger.Error($"[SBNV]: N-15 Adapter item {N15_ADAPTER_ID} not found in database.");
        }
            
        return Task.CompletedTask;
    }
}