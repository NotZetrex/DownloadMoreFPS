using UnityEngine;
using System;
using MelonLoader;

[assembly: MelonInfo(typeof(DownloadMoreFPS.Main), "DownloadMoreFPS", "6.9.4.20", "Zetrex", null)]
[assembly: MelonColor(ConsoleColor.Green)]
[assembly: MelonGame("VRChat", "VRChat")]
[assembly: MelonPriority(2147483647)]

namespace DownloadMoreFPS
{
    public class Main : MelonMod
    {
        private bool enabled;

        public static MelonPreferences_Category category;
        public static MelonPreferences_Entry<bool> ml_Enabled;
        public override void OnApplicationStart()
        {
            category = MelonPreferences.CreateCategory("DownloadMoreFPS");

            ml_Enabled = category.CreateEntry("enabled", false);

            enabled = ml_Enabled.Value;

            MelonLogger.Msg("Use the \"-\" and \"+\" keys to enable and disable.");

            if (enabled)
            {
                MelonLogger.Msg("Currently Enabled");
                VRChatUtilityKit.Utilities.NetworkEvents.OnAvatarInstantiated += FixAvatar;
            }
            else
            {
                MelonLogger.Msg("Currently Disabled");
                VRChatUtilityKit.Utilities.NetworkEvents.OnAvatarInstantiated -= FixAvatar;
            }
        }

        private void FixAvatar(VRCAvatarManager vrca, VRC.Core.ApiAvatar apiAvatar, GameObject gameObject)
        {
            GameObject shadowClone = gameObject.transform.parent.Find("_AvatarShadowClone").gameObject;
            GameObject mirrorClone = gameObject.transform.parent.Find("_AvatarMirrorClone").gameObject;
            GameObject.DestroyImmediate(shadowClone);
            GameObject.DestroyImmediate(mirrorClone);

            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                GameObject obj = gameObject.transform.GetChild(i).gameObject;

                if (obj.GetComponent<SkinnedMeshRenderer>())
                {
                    SkinnedMeshRenderer skinnedMeshRenderer = obj.GetComponent<SkinnedMeshRenderer>();

                    skinnedMeshRenderer.castShadows = true;
                    obj.layer = 9;
                }
            }
        }

        public override void OnFixedUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Equals))
            {
                enabled = true;
                ml_Enabled.Value = enabled;
                ml_Enabled.Save();
                VRChatUtilityKit.Utilities.NetworkEvents.OnAvatarInstantiated += FixAvatar;
                LoggerInstance.Msg("Enabled");
                category.SaveToFile(false);
            }

            if (Input.GetKeyDown(KeyCode.Minus))
            {
                enabled = false;
                ml_Enabled.Value = enabled;
                ml_Enabled.Save();
                MelonPreferences.SetEntryValue<bool>("DownloadMoreFPS", "enabled", enabled);
                VRChatUtilityKit.Utilities.NetworkEvents.OnAvatarInstantiated -= FixAvatar;
                LoggerInstance.Msg("Disabled");
                category.SaveToFile(false);
            }
        }
    }
}
