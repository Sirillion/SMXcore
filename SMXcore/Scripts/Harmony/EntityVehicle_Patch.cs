using HarmonyLib;
using Quartz;
using SMXcore.HarmonyPatches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SMXcore.HarmonyPatches
{

    [HarmonyPatch(typeof(EntityVehicle))]
    public class EntityVehicle_Patch
    {

        [HarmonyPrefix]
        [HarmonyPatch("getStorageSize")]
        public static bool getStorageSize(EntityVehicle __instance, ref Vector2i __result)
        {
            if (__instance.storageModCount < 1)
            {
                return true;
            }

            ItemValue[] modifications = __instance.vehicle.itemValue.Modifications;
            if (modifications == null)
            {
                return true;
            }

            __result = LootContainer.GetLootContainer(__instance.GetLootList(), true).size;

            foreach (ItemValue itemValue in modifications)
            {
                if (itemValue != null && itemValue.ItemClass is ItemClassModifier itemClassModifier
                    && itemClassModifier.ItemTags.Test_AnySet(EntityVehicle.StorageModifierTags))
                {
                    int rowIncrease = 1;
                    itemClassModifier.Properties.ParseInt("containerRowIncrease", ref rowIncrease);

                    __result.y += rowIncrease;
                }
            }

            return false;
        }
    }
}
