using BepInEx.IL2CPP;
using System;
using System.Runtime.CompilerServices;

namespace PermissionGroups
{
    internal static class BetterChatCompatibility
    {
        internal static bool? enabled;
        internal static bool Enabled
            => enabled == null ? (bool)(enabled = IL2CPPChainloader.Instance.Plugins.ContainsKey("lammas123.BetterChat")) : enabled.Value;

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        internal static bool RegisterFormatting(string formatting, Func<ulong, string> func)
            => BetterChat.Api.RegisterFormatting(formatting, func);
    }
}