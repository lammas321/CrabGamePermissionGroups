using SteamworksNative;

namespace PermissionGroups
{
    internal static class Utility
    {
        internal static ulong ClientId => SteamManager.Instance.field_Private_CSteamID_0.m_SteamID;
    }
}