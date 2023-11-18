using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;

namespace DisassembleChiseledBlock;

[HarmonyPatch(typeof(CollectibleObject), nameof(CollectibleObject.OnConsumedByCrafting))]
public static class ChiseledBlock_OnConsumedByCrafting_Patch
{
    public static bool Prefix(CollectibleObject __instance, ItemSlot[] allInputSlots, ItemSlot stackInSlot, GridRecipe gridRecipe, CraftingRecipeIngredient fromIngredient, IPlayer byPlayer, int quantity)
    {
        ItemStack blockChiseled = stackInSlot?.Itemstack;
        if (blockChiseled?.Collectible is not BlockMicroBlock)
        {
            return true;
        }

        bool toolHammer = allInputSlots.Any(x => x?.Itemstack?.Collectible is ItemHammer);
        bool toolChisel = allInputSlots.Any(x => x?.Itemstack?.Collectible is ItemChisel);

        if (blockChiseled == null || !toolHammer || !toolChisel)
        {
            return true;
        }

        if (blockChiseled.Attributes["materials"] is not IntArrayAttribute materials || materials?.value?.Length == 0)
        {
            return true;
        }

        List<ItemStack> blocksToDrop = new();
        blocksToDrop.AddRange(materials.value.Select(blockid => new ItemStack(byPlayer.Entity.World.GetBlock(blockid))));

        foreach (var itemstack in blocksToDrop)
        {
            if (!byPlayer.InventoryManager.TryGiveItemstack(itemstack, slotNotifyEffect: true))
            {
                byPlayer.Entity.World.SpawnItemEntity(itemstack, byPlayer.Entity.Pos.XYZ);
            }
        }

        return true;
    }
}