//using System.Collections;

using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace CameraMouseDragMod
{
    public class MainLoadingExtension : LoadingExtensionBase
    {
        public override void OnLevelLoaded(LoadMode mode)
        {
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "CameraMouseDrag: Level loaded");

            UIView uiv = GameObject.FindObjectOfType<UIView>();
            //GlobalBehavior b = uiv.gameObject.AddComponent<GlobalBehavior>();
            uiv.gameObject.AddComponent<GlobalBehavior>();
        }
    }

}
