using HarmonyLib;
using SteamworksNative;

namespace PermissionGroups
{
    internal static class Patches
    {
        //   Set permission group for the host
        [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.StartLobby))]
        [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.StartPracticeLobby))]
        [HarmonyPostfix]
        internal static void PostLobbyManagerStartLobby()
        {
            PersistentData.ClientDataFile hostFile = PersistentData.Api.GetClientDataFile(SteamUser.GetSteamID().m_SteamID);
            hostFile.Set("PermissionGroup", Api.HostPermissionGroupId);
            hostFile.SaveFile();
        }

        //   Set permission group for those that join
        [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.OnPlayerJoinLeaveUpdate))]
        [HarmonyPostfix]
        internal static void PostLobbyManagerOnPlayerJoinLeaveUpdate(CSteamID param_1, bool param_2)
        {
            if (!SteamManager.Instance.IsLobbyOwner() || !param_2)
                return;

            PersistentData.ClientDataFile clientFile = PersistentData.Api.GetClientDataFile(param_1.m_SteamID);
            string permissionGroupId = clientFile.Get("PermissionGroup");
            if (permissionGroupId != null && Api.HasPermissionGroup(permissionGroupId))
                return;

            clientFile.Set("PermissionGroup", Api.DefaultPermissionGroupId);
            clientFile.SaveFile();
        }
    }
}