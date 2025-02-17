using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace DisassembleChiseledBlock;

public class Core : ModSystem
{
    public override void StartServerSide(ICoreServerAPI api)
    {
        IChatCommand command = api.ChatCommands.GetOrCreate("dischisel")
            .RequiresPlayer()
            .RequiresPrivilege(Privilege.buildblocks)
            .HandleWith(HandleDisassembleChiseledBlock);

        Mod.Logger.Event("started '{0}' mod", Mod.Info.Name);
    }

    private TextCommandResult HandleDisassembleChiseledBlock(TextCommandCallingArgs args)
    {
        IPlayer player = args.Caller.Player;
        IWorldAccessor world = player.Entity.World;
        BlockSelection blockSel = player.CurrentBlockSelection;
        ItemSlot rightSlot = player.Entity.RightHandItemSlot;
        ItemSlot leftSlot = player.Entity.LeftHandItemSlot;

        if (rightSlot?.Itemstack?.Collectible is not ItemChisel || leftSlot?.Itemstack?.Collectible is not ItemHammer)
        {
            return TextCommandResult.Error(Lang.Get("dischiselblock:command-error-no-required-tools"), "dischiselblock:command-error-no-required-tools");
        }

        if (blockSel == null || blockSel.Block is not BlockMicroBlock)
        {
            return TextCommandResult.Error(Lang.Get("dischiselblock:command-error-no-chiseled-block"), "dischiselblock:command-error-no-chiseled-block");
        }

        ItemStack chiseledStack = blockSel.Block.OnPickBlock(world, blockSel.Position);
        if (chiseledStack.Attributes["materials"] is not IntArrayAttribute materials || materials?.value?.Length == 0)
        {
            return TextCommandResult.Deferred;
        }

        List<ItemStack> giveStacks = new();
        giveStacks.AddRange(materials.value.Select(blockid => new ItemStack(world.GetBlock(blockid))));

        foreach (ItemStack itemstack in giveStacks)
        {
            if (!player.InventoryManager.TryGiveItemstack(itemstack, slotNotifyEffect: true))
            {
                player.Entity.World.SpawnItemEntity(itemstack, player.Entity.Pos.XYZ);
            }
        }

        world.BlockAccessor.SetBlock(0, blockSel.Position);
        world.BlockAccessor.TriggerNeighbourBlockUpdate(blockSel.Position);
        return TextCommandResult.Deferred;
    }
}