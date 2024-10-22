using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PermissionGroups
{
    public static class Api
    {
        public static string PermissionGroupsPath
            => Path.Combine(Paths.ConfigPath, $"lammas123.{MyPluginInfo.PLUGIN_GUID}");

        internal static Dictionary<string, PermissionGroup> permissionGroups = [];

        public static string DefaultPermissionGroupId;
        public static string HostPermissionGroupId;
        

        public static string[] GetPermissionGroupIds()
            => [.. permissionGroups.Keys];

        public static bool HasPermissionGroup(string permissionGroupId)
            => permissionGroups.ContainsKey(permissionGroupId);
        
        public static string GetPermissionGroupName(string permissionGroupId)
            => HasPermissionGroup(permissionGroupId) ? permissionGroups[permissionGroupId].Name : permissionGroupId;
        public static bool PermissionGroupHasPermission(string permissionGroupId, string permission)
            => HasPermissionGroup(permissionGroupId) && permissionGroups[permissionGroupId].HasPermission(permission);

        public static string GetClientPermissionGroup(ulong clientId)
        {
            PersistentData.ClientDataFile clientDataFile = PersistentData.Api.GetClientDataFile(clientId);
            string permissionGroupId = clientDataFile.Get("PermissionGroup");
            if (permissionGroupId != null && HasPermissionGroup(permissionGroupId))
                return permissionGroupId;
            
            clientDataFile.Set("PermissionGroup", DefaultPermissionGroupId);
            clientDataFile.SaveFile();
            return DefaultPermissionGroupId;
        }
        public static bool SetClientPermissionGroup(ulong clientId, string permissionGroupId)
        {
            if (!HasPermissionGroup(permissionGroupId))
                return false;

            PersistentData.ClientDataFile clientDataFile = PersistentData.Api.GetClientDataFile(clientId);
            clientDataFile.Set("PermissionGroup", permissionGroupId);
            clientDataFile.SaveFile();
            return true;
        }
    }

    public struct PermissionGroup
    {
        public string Id { get; internal set; }
        public string Name { get; internal set; }
        public string[] Permissions { get; internal set; }


        internal PermissionGroup(string id, string name, string[] permissions)
        {
            Id = id;
            Name = name;
            Permissions = permissions;
        }

        public readonly bool HasPermission(string permission)
            => Permissions.Length == 1 && Permissions[0] == "*" || Permissions.Contains(permission);
    }
}