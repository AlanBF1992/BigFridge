using HarmonyLib;
using StardewModdingAPI;
using System.Reflection.Emit;

namespace BigFridge.Compatibility.VPP.Patches
{
    internal static class ModEntryPatches
    {
        internal readonly static IMonitor LogMonitor = ModEntry.LogMonitor;

        internal static IEnumerable<CodeInstruction> OnButtonPressedTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                CodeMatcher matcher = new(instructions);

                matcher
                    .MatchStartForward(
                        new CodeMatch(OpCodes.Ldstr, "(BC)216")
                    )
                    .MatchStartForward(
                        new CodeMatch(OpCodes.Ldloc_S)
                    )
                    .ThrowIfNotMatch("ModEntryPatches.OnButtonPressedTranspiler: IL code not found")
                    .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_0))
                ;

                return matcher.InstructionEnumeration();
            }
            catch (Exception ex)
            {
                LogMonitor.Log($"Failed in {nameof(OnButtonPressedTranspiler)}:\n{ex}", LogLevel.Error);
                return instructions;
            }
        }
    }
}
