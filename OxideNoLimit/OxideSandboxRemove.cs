using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Harmony;
using Oxide.Plugins;

namespace OxideNoLimit
{
    [HarmonyPatch]
    public static class OxideSandboxRemove
    {
        private static MethodBase GetSandboxMethod(Type typ) =>
            AccessTools.FirstMethod(typ,
                info => info.IsAssembly && info.ReturnType == typeof(void) && info.GetParameters().FirstOrDefault()?.ParameterType.Name == "TypeDefinition");

        internal static MethodBase TargetMethod()
        {
            MethodBase target = null;
            var targetDelegateType = AccessTools.FirstInner(typeof(CompiledAssembly), typ =>
            {
                if (!typ.IsClass || !typ.IsDefined(typeof(CompilerGeneratedAttribute),false) || AccessTools.Field(typ, "patchModuleType") == null)
                    return false;
                return (target=GetSandboxMethod(typ)) != null;
            });
            if (targetDelegateType != null)
                return target;
            FileLog.Log("[OxideNoLimit] Can't find target in CompiledAssembly. Not patched.");
            return null;
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Ret);
        }
    }
}
