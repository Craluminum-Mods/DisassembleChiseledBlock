using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace DisassembleChiseledBlock;

public class Core : ModSystem
{
    public override void StartServerSide(ICoreServerAPI api)
    {

        Mod.Logger.Event("started '{0}' mod", Mod.Info.Name);
    }
}
