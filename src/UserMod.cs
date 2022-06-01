using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ICities;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using UnityEngine;

[assembly: AssemblyVersion("1.0.*")]

namespace CameraMouseDragMod
{

    [ConfigurationPath("CameraDrag.xml")]
    public class ModConfig
    {
        public int MouseButton { get; set; } = 1;
        public float Speed { get; set; } = 2.0f;
        public bool Invert { get; set; } = false;
        public bool AllowCursorLock { get; set; } = true;
        public int Modifier { get; set; } = 0;
    }

    public static class Modifiers
    {
        private static Dictionary<string, Func<bool>> KeysToCheck = new Dictionary<string, Func<bool>>
        {
            {"Left Alt", () => Input.GetKey(KeyCode.LeftAlt)},
            {"Left Ctrl", () => Input.GetKey(KeyCode.LeftControl)},
            {"Left Shift", () => Input.GetKey(KeyCode.LeftShift)},
            {"Right Alt", () => Input.GetKey(KeyCode.RightAlt)},
            {"Right Ctrl", () => Input.GetKey(KeyCode.RightControl)},
            {"Right Shift", () => Input.GetKey(KeyCode.RightShift)},
        };

        public static IEnumerable<string> Labels() => new[] { "<None>" }.Concat(KeysToCheck.Keys);
        public static IEnumerable<Func<bool>> PressedKeysFor(int index) => index > 0 ? new List<Func<bool>> { KeysToCheck.Values.ElementAt(index - 1) } : new List<Func<bool>>();
        public static IEnumerable<Func<bool>> ReleasedKeysFor(int index) => index > 0 ? KeysToCheck.Values.Where((f, i) => i != index - 1).ToList() : KeysToCheck.Values.ToList();
    }

    public class UserMod : IUserMod
    {
        public string Name => "Camera Mouse Drag";
        public string Description => "Camera scroll by dragging the mouse";

        public void OnSettingsUI(UIHelperBase helper)
        {
            var configuration = Configuration<ModConfig>.Load();
            UIHelperBase group = helper.AddGroup("Camera Mouse Drag");
            group.AddSlider("Speed", 0.1f, 5.0f, 0.1f,
                    configuration.Speed, value => SaveAfterApply(value, x => configuration.Speed = x));
            group.AddCheckbox("Invert direction",
                    configuration.Invert, value => SaveAfterApply(value, x => configuration.Invert = x));
            group.AddCheckbox("Lock cursor at middle when dragging to edge",
                    configuration.AllowCursorLock, value => SaveAfterApply(value, x => configuration.AllowCursorLock = x));
            group.AddDropdown("Modifier Key:", Modifiers.Labels().ToArray(), configuration.Modifier, value => SaveAfterApply(value, x => configuration.Modifier = x));
        }

        private void SaveAfterApply<T>(T value, Action<T> apply)
        {
            apply(value);
            Configuration<ModConfig>.Save();
        }
    }
}
