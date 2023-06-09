﻿using MelonLoader;
using HarmonyLib;
using Il2Cpp;
using buffed_recarm_06;
using Il2Cppnewdata_H;

[assembly: MelonInfo(typeof(BuffedRecarm06), "Buffed Recarm/Revival Bead (ver. 0.6)", "1.0.0", "Matthiew Purple")]
[assembly: MelonGame("アトラス", "smt3hd")]

namespace buffed_recarm_06;
public class BuffedRecarm06 : MelonMod
{
    private static bool usingRecarm; // Is true when the last used skill/item during battle was Recarm/Revival Bead

    // After applying a skill effect outside of battle
    [HarmonyPatch(typeof(datCalc), nameof(datCalc.datExecSkill))]
    private class Patch
    {
        public static void Postfix(ref int nskill, ref datUnitWork_t d)
        {
            // If using Recarm/Revival Bead then change the target's HP to half of its maximum
            if (nskill == 49) d.hp = (ushort)(d.maxhp / 2);
        }
    }

    // After getting a skill description
    [HarmonyPatch(typeof(datSkillHelp_msg), nameof(datSkillHelp_msg.Get))]
    private class Patch2
    {
        public static void Postfix(ref int id, ref string __result)
        {
            // If using Recarm then change the returned description
            if (id == 49) __result = "Revives one ally \nwith half HP.";
        }
    }

    // After getting an item description
    [HarmonyPatch(typeof(datItemHelp_msg), nameof(datItemHelp_msg.Get))]
    private class Patch3
    {
        public static void Postfix(ref int id, ref string __result)
        {
            // If using a Revival Bead then change the returned description
            if (id == 13) __result = "Revives one ally \nwith half HP.";
        }
    }

    // After intiating a skill during battle
    [HarmonyPatch(typeof(nbActionProcess), nameof(nbActionProcess.MAKE_SKILL_SE01))]
    private class Patch4
    {
        public static void Postfix(ref int x)
        {
            // Memorize if that skill was Recarm (or Revival Bead as items are actually skills)
            usingRecarm = x == 49;
        }
    }

    // Before adding HP
    [HarmonyPatch(typeof(datCalc), nameof(datCalc.datAddHp))]
    private class Patch5
    {
        public static void Prefix(ref datUnitWork_t work, ref int hp)
        {
            // If using Recarm/Revival Bead
            if (usingRecarm)
            {
                // Change the target's HP to half of its maximum
                hp = work.maxhp / 2;

                // Forget Recarm/Revival Bead were used
                usingRecarm = false;
            }
        }
    }
}
