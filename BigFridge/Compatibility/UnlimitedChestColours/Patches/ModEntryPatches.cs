using HarmonyLib;
using StardewModdingAPI;
using StardewValley.Objects;
using System.Reflection;
using System.Reflection.Emit;

namespace BigFridge.Compatibility.UnlimitedChestColours.Patches
{
    internal static class ModEntryPatches
    {
        internal readonly static IMonitor LogMonitor = ModEntry.LogMonitor;

        internal static IEnumerable<CodeInstruction> OpenColourPickerTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                CodeMatcher matcher = new(instructions);

                MethodInfo setChestDisplayedInfo = AccessTools.Method(typeof(ModEntryPatches), nameof(setChestDisplayed));

                matcher
                    .MatchStartForward(
                        new CodeMatch(OpCodes.Ldc_I4_4)
                    )
                    .ThrowIfNotMatch("ModEntryPatches.OpenColourPickerTranspiler: IL code not found")
                ;

                while (matcher.Opcode != OpCodes.Stloc_1)
                {
                    matcher.RemoveInstruction();
                }

                matcher
                    .Insert(
                        new CodeInstruction(OpCodes.Ldloc_0),
                        new CodeInstruction(OpCodes.Call, setChestDisplayedInfo)
                    )
                ;

                return matcher.InstructionEnumeration();
            }
            catch (Exception ex)
            {
                LogMonitor.Log($"Failed in {nameof(OpenColourPickerTranspiler)}:\n{ex}", LogLevel.Error);
                return instructions;
            }
        }

        internal static IEnumerable<CodeInstruction> drawChestTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                CodeMatcher matcher = new(instructions);

                MethodInfo setIndexesInfo = AccessTools.Method(typeof(ModEntryPatches), nameof(setIndexes));

                matcher
                    .MatchStartForward(
                        new CodeMatch(OpCodes.Stloc_3)
                    )
                    .ThrowIfNotMatch("ModEntryPatches.drawChestTranspiler: IL code not found")
                    .Advance(1)
                    .Insert(
                        new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(OpCodes.Ldloca_S, 1),
                        new CodeInstruction(OpCodes.Ldloca_S, 2),
                        new CodeInstruction(OpCodes.Ldloca_S, 3),
                        new CodeInstruction(OpCodes.Call, setIndexesInfo)
                    )
                ;

                return matcher.InstructionEnumeration();
            }
            catch (Exception ex)
            {
                LogMonitor.Log($"Failed in {nameof(OpenColourPickerTranspiler)}:\n{ex}", LogLevel.Error);
                return instructions;
            }
        }

        private static List<Chest> setChestDisplayed(Chest chest)
        {
            if (chest.QualifiedItemId == "(BC)AlanBF.BigFridge" || chest.QualifiedItemId == "(BC)216")
            {
                return ModEntry.Config.ReskinMiniFridge
                    ? [new(true, "216"), new(true, "AlanBF.BigFridge")]
                    : [new(true, "AlanBF.BigFridge")];
            }

            return
            [
                new(true),
                new(true, "232"),
                new(true, "BigChest"),
                new(true, "BigStoneChest")
            ];
        }

        private static void setIndexes(Chest chest, ref int drawIndex, ref int overlayIndex, ref int coloredLidIndex)
        {
            if (chest.QualifiedItemId != "(BC)AlanBF.BigFridge" && chest.QualifiedItemId != "(BC)216")
            {
                return;
            }

            drawIndex = 3;
            overlayIndex = 6;
            coloredLidIndex = 4;
        }
    }
}
