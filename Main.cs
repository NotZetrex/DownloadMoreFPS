using UnityEngine;
using System;
using MelonLoader;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

[assembly: MelonInfo(typeof(DownloadMoreFPS.Main), "DownloadMoreFPS", "0.0.0.2", "Zetrex", null)]
[assembly: MelonColor(ConsoleColor.Green)]
[assembly: MelonGame("VRChat", "VRChat")]

namespace DownloadMoreFPS
{
    public class Main : MelonMod
    {
        private bool enabled;
        private bool can_run = true;

        public static MelonPreferences_Category category;
        public static MelonPreferences_Entry<bool> ml_Enabled;

        public override void OnPreSupportModule()
        {
            if (!File.Exists(MelonUtils.GameDirectory + "/Mods/VRChatUtilityKit.dll"))
            {
                MelonLogger.Warning("Could not find VRChatUtilityKit.dll, this mod with not function without it!");
                can_run = false;
                Thread.Sleep(2000)
            }

            if (!File.Exists(MelonUtils.GameDirectory + "/Mods/UIExpansionKit.dll"))
            {
                MelonLogger.Warning("Could not find UIExpansionKit.dll, this mod with not function without it!");
                can_run = false;
                hread.Sleep(2000)
            }
        }
        public override void OnApplicationStart()
        {
            if (!can_run)
                return;

            category = MelonPreferences.CreateCategory("DownloadMoreFPS");

            ml_Enabled = category.CreateEntry("DMFPS_Enabled", false);

            enabled = ml_Enabled.Value;

            if (enabled == true)
            {
                VRChatUtilityKit.Utilities.NetworkEvents.OnAvatarInstantiated += FixAvatar;
            }

            ml_Enabled.OnValueChanged += Update;
        }

        private void FixAvatar(VRCAvatarManager vrca, VRC.Core.ApiAvatar apiAvatar, GameObject gameObject)
        {
            MelonLogger.Msg("Running fix...");

            GameObject shadowClone = gameObject.transform.parent.Find("_AvatarShadowClone").gameObject;
            GameObject mirrorClone = gameObject.transform.parent.Find("_AvatarMirrorClone").gameObject;
            GameObject.DestroyImmediate(shadowClone);
            GameObject.DestroyImmediate(mirrorClone);

            SkinnedMeshRenderer[] skinnedMeshRenderers = Resources.FindObjectsOfTypeAll<SkinnedMeshRenderer>();
            MeshRenderer[] MeshRenderers = Resources.FindObjectsOfTypeAll<MeshRenderer>();

            Task.Run(() =>
            {
                foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
                {
                    if (skinnedMeshRenderer.gameObject.layer == 10)
                    {
                        skinnedMeshRenderer.castShadows = true;
                        skinnedMeshRenderer.gameObject.layer = 9;
                    }
                }

                foreach (MeshRenderer MeshRenderer in MeshRenderers)
                {
                    if (MeshRenderer.gameObject.layer == 10)
                    {
                        MeshRenderer.castShadows = true;
                        MeshRenderer.gameObject.layer = 9;
                    }
                }
            });

            if (gameObject.transform.parent.gameObject.name != "ForwardDirection")
            {
                MelonLogger.Error("Avatar failed to reload properly, please reload again!");
                return;
            }

            MelonLogger.Msg("Fix Complete!");
        }
        private void Update(bool _bool, bool _bool2)
        {
            if (_bool2)
            {
                enabled = ml_Enabled.Value;
                MelonPreferences.SetEntryValue<bool>("DownloadMoreFPS", "DMFPS_Enabled", enabled);
                VRChatUtilityKit.Utilities.NetworkEvents.OnAvatarInstantiated += FixAvatar;
                LoggerInstance.Msg("Enabled");
                category.SaveToFile(false);
            }

            if (!_bool2)
            {
                enabled = ml_Enabled.Value;
                MelonPreferences.SetEntryValue<bool>("DownloadMoreFPS", "DMFPS_Enabled", enabled);
                VRChatUtilityKit.Utilities.NetworkEvents.OnAvatarInstantiated -= FixAvatar;
                LoggerInstance.Msg("Disabled");
                category.SaveToFile(false);
            }
        }
    }
}
