using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using Oxide.Plugins;

namespace OxideNoLimit
{
    [HarmonyPatch]
    internal class OxideReferenceTweakOne
    {
        internal static MethodBase TargetMethod()
        {
            var compilation = AccessTools.TypeByName("Oxide.Plugins.Compilation, Oxide.CSharp");
            if (compilation == null)
            {
                FileLog.Log("[OxideNoLimit] Can't find Compilation type. Not patched.");
                return null;
            }
            var target = AccessTools.Method(compilation, "PreparseScript", new[] {typeof(CompilablePlugin)});
            if (target == null)
            {
                FileLog.Log("[OxideNoLimit] Can't find PreparseScript method. Not patched.");
                return null;
            }
            return target;
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var lst = instructions.ToList();
            var index = lst.FindIndex(inst => inst.opcode == OpCodes.Ldstr && (string) inst.operand == "Harmony");
            if (index <= 0 || lst[index + 1].opcode != OpCodes.Callvirt)
            {
                FileLog.Log("[OxideNoLimit] Can't find Harmony in PreparseScript. Not patched.");
                return lst;
            }
            lst.RemoveRange(index, 4);
            return lst;
        }
    }
    
    [HarmonyPatch(typeof(CSharpPluginLoader), MethodType.Constructor, typeof(CSharpExtension))]
    internal class OxideReferenceTweakTwo
    {
        static void Prefix() => CSharpPluginLoader.PluginReferences.Add("0Harmony");
    }
    
    // Can't patch static cctors. lol.
    /*[HarmonyPatch(typeof(CSharpPluginLoader), MethodType.StaticConstructor)]
    public class OxideReferenceTweakTwo
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var lst = instructions.ToList();
            if (lst.Count == 0 || !lst[0].opcode.Name.StartsWith("ldc.i4", StringComparison.InvariantCultureIgnoreCase))
            {
                FileLog.Log("[OxideNoLimit] Invalid CSharpPluginLoader.cctor. Not patched.");
                return lst;
            }
            var ctr = 0;
            var index = 0;
            for (var i = 0; i < lst.Count; i++)
            {
                var inst = lst[i];
                if (inst.opcode == OpCodes.Stelem_Ref)
                    ctr++;
                if (inst.opcode == OpCodes.Stsfld)
                {
                    index = i;
                    break;
                }
            }
            if (ctr == 0 || index == 0)
            {
                FileLog.Log("[OxideNoLimit] Invalid CSharpPluginLoader.cctor. Not patched.");
                return lst;
            }
            lst[0] = new CodeInstruction(OpCodes.Ldc_I4, ctr+1);
            lst.InsertRange(index, new []
            {
                new CodeInstruction(OpCodes.Dup), 
                new CodeInstruction(OpCodes.Ldc_I4, ctr), 
                new CodeInstruction(OpCodes.Ldstr,  "0Harmony"), 
                new CodeInstruction(OpCodes.Stelem_Ref), 
            });
            return lst;
        }
    }*/
}
