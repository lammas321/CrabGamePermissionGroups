using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace PermissionGroups
{
    [BepInPlugin($"lammas123.{MyPluginInfo.PLUGIN_NAME}", MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("lammas123.PersistentData")]
    [BepInDependency("lammas123.BetterChat", BepInDependency.DependencyFlags.SoftDependency)]
    public class PermissionGroups : BasePlugin
    {
        internal static PermissionGroups Instance;

        public override void Load()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            Instance = this;

            Api.DefaultPermissionGroupId = Config.Bind("Permission Groups", "Api.DefaultPermissionGroupId", "member", "The id of the default permission group.").Value.ToLower();
            if (Api.DefaultPermissionGroupId.Length == 0)
                Api.DefaultPermissionGroupId = "member";

            Api.HostPermissionGroupId = Config.Bind("Permission Groups", "Api.HostPermissionGroupId", "host", "The id of the host permission group.").Value.ToLower();
            if (Api.HostPermissionGroupId.Length == 0)
                Api.HostPermissionGroupId = "host";

            Directory.CreateDirectory(Api.PermissionGroupsPath);
            foreach (string permissionGroupPath in Directory.GetFiles(Api.PermissionGroupsPath))
            {
                string permissionGroupId = Path.GetFileNameWithoutExtension(Path.GetFileName(permissionGroupPath)).Replace(' ', '_').ToLower();
                if (permissionGroupId.Length == 0 || Api.permissionGroups.ContainsKey(permissionGroupId))
                    continue;

                string[] lines = File.ReadAllLines(permissionGroupPath);
                if (lines.Length == 0 || lines[0].Length == 0)
                    continue;

                string permissionGroupName = lines[0];
                List<string> permissions = [];
                foreach (string permission in lines[1..])
                    if (permission.Length != 0 && !permission.StartsWith('#'))
                        permissions.Add(permission);

                Api.permissionGroups.Add(permissionGroupId, new(permissionGroupId, permissionGroupName, [.. permissions]));
            }

            if (!Api.permissionGroups.ContainsKey(Api.DefaultPermissionGroupId))
            {
                string defaultPermissionGroupName = $"{Api.DefaultPermissionGroupId[0].ToString().ToUpper()}{Api.DefaultPermissionGroupId[1..]}";
                Api.permissionGroups.Add(Api.DefaultPermissionGroupId, new(Api.DefaultPermissionGroupId, defaultPermissionGroupName, []));
                File.WriteAllLines(Path.Combine(Api.PermissionGroupsPath, $"{Api.DefaultPermissionGroupId}.txt"), [defaultPermissionGroupName]);
            }

            if (!Api.permissionGroups.ContainsKey(Api.HostPermissionGroupId))
            {
                string hostPermissionGroupName = $"{Api.HostPermissionGroupId[0].ToString().ToUpper()}{Api.HostPermissionGroupId[1..]}";
                Api.permissionGroups.Add(Api.HostPermissionGroupId, new(Api.HostPermissionGroupId, hostPermissionGroupName, ["*"]));
                File.WriteAllLines(Path.Combine(Api.PermissionGroupsPath, $"{Api.HostPermissionGroupId}.txt"), [hostPermissionGroupName, "*"]);
            }

            if (BetterChatCompatibility.Enabled)
            {
                BetterChatCompatibility.RegisterFormatting("PERMISSION_GROUP", FormatPermissionGroup);
                BetterChatCompatibility.RegisterFormatting("PERMISSION_GROUP_ID", FormatPermissionGroupId);
            }

            Harmony.CreateAndPatchAll(typeof(Patches));
            Log.LogInfo($"Loaded [{MyPluginInfo.PLUGIN_NAME} {MyPluginInfo.PLUGIN_VERSION}]");
        }

        internal static string FormatPermissionGroup(ulong clientId)
            => Api.GetPermissionGroupName(Api.GetClientPermissionGroup(clientId));
        internal static string FormatPermissionGroupId(ulong clientId)
            => Api.GetClientPermissionGroup(clientId);
    }
}