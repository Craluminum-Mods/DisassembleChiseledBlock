using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

[assembly: ModInfo(name: "Disassemble Chiseled Block", modID: "dischiselblock", Side = "Server")]

namespace DisassembleChiseledBlock;

public class HarmonyPatches : ModSystem
{
    public const string HarmonyID = "craluminum2413.dischiselblock";
    public static Harmony HarmonyInstance { get; set; } = new Harmony(HarmonyID);

    public override void StartServerSide(ICoreServerAPI api)
    {
        base.StartServerSide(api);
        HarmonyInstance.Patch(original: typeof(CollectibleObject).GetMethod(nameof(CollectibleObject.OnConsumedByCrafting)), prefix: typeof(ChiseledBlock_OnConsumedByCrafting_Patch).GetMethod(nameof(ChiseledBlock_OnConsumedByCrafting_Patch.Prefix)));
        api.World.Logger.Event("started '{0}' mod", Mod.Info.Name);
    }

    public override void Dispose()
    {
        HarmonyInstance.Unpatch(original: typeof(CollectibleObject).GetMethod(nameof(CollectibleObject.OnConsumedByCrafting)), type: HarmonyPatchType.All, HarmonyID);
        base.Dispose();
    }
}