using HarmonyLib;
using StardewModdingAPI;
using System.Reflection.Emit;

namespace BigFridge.Compatibility.VPP.Patches
{
    internal static class DayStartHandlerPatches
    {
        internal readonly static IMonitor LogMonitor = ModEntry.LogMonitor;

        internal static IEnumerable<CodeInstruction> OnDayStartedTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                CodeMatcher matcher = new(instructions);

                matcher
                    .MatchStartForward(
                        new CodeMatch(OpCodes.Ldstr, "MiniFridgeBigSpace")
                    )
                    .MatchStartForward(
                        new CodeMatch(OpCodes.Stfld)
                    )
                    .ThrowIfNotMatch("DayStartHandlerPatches.OnDayStartedTranspiler: IL code not found")
                    .Insert(
                        new CodeInstruction(OpCodes.Pop),
                        new CodeInstruction(OpCodes.Ldc_I4_0)
                    )
                ;

                return matcher.InstructionEnumeration();
            }
            catch (Exception ex)
            {
                LogMonitor.Log($"Failed in {nameof(OnDayStartedTranspiler)}:\n{ex}", LogLevel.Error);
                return instructions;
            }
        }
    }
}
