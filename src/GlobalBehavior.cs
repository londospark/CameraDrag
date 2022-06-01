using System.Linq;
using ICities;
using UnityEngine;

namespace CameraMouseDragMod
{
    public class GlobalBehavior : MonoBehaviour
    {
        private Vector3 oldMousePosition = new Vector3(-1, -1, -1);
        private bool oldIsButtonDown = false;
        private bool windowHasFocus = true;
        private ModConfig config;

        private bool IsButtonDown()
        {
            if (!windowHasFocus)
                return false;
            if (!Input.GetMouseButton(config.MouseButton))
                return false;

            var modifierDown = Modifiers.PressedKeysFor(config.Modifier).All(func => func());
            var otherModifiersClear = Modifiers.ReleasedKeysFor(config.Modifier).All(func => !func());

            return modifierDown && otherModifiersClear;
        }

        public void Awake()
        {
            config = Configuration<ModConfig>.Load();
        }

        public void LateUpdate()
        {
            GameObject mainCameraObject = GameObject.FindGameObjectWithTag("MainCamera");
            if (mainCameraObject != null)
            {
                //Camera cam = mainCameraObject.GetComponent<Camera>();

                if (oldMousePosition != (new Vector3(-1, -1, -1)))
                {
                    CameraController cameraController = mainCameraObject.GetComponent<CameraController>();
                    Vector3 currentPos = cameraController.m_currentPosition;

                    Vector3 mouseD = Input.mousePosition - oldMousePosition;

                    if (IsButtonDown())
                    {
                        /*if(!oldIsButtonDown && UserMod.configuredAllowCursorLock){
                            Cursor.lockState = CursorLockMode.Locked;
                            //Cursor.visible = false;
                        }*/
                        if (Cursor.lockState == CursorLockMode.Locked)
                        {
                            mouseD.y *= -1.0f; // Apparently this is needed to maintain the same direction
                        }
                        Vector3 v = mainCameraObject.transform.forward;
                        v.y = 0.0f;
                        v.Normalize();
                        Vector3 u = Vector3.Cross(v, new Vector3(0, 1, 0));
                        float ground_height = 100f; // This is kind of close
                        float base_speed = currentPos.y - ground_height;
                        if (base_speed < 5f)
                            base_speed = 5f;
                        float user_speed = config.Speed;
                        if (config.Invert)
                            user_speed *= -1.0f;
                        if (Cursor.lockState == CursorLockMode.Locked)
                            base_speed *= 1.5f; // For some reason this feels better
                        Vector3 d = (-v * mouseD.y * 1.2f) + (u * mouseD.x * 1.0f);
                        d *= base_speed * -0.015f * user_speed;
                        cameraController.m_targetPosition = currentPos + d;

                        // NOTE: On Linux when the window is unfocused mouse is at (0,0)
                        if (Cursor.lockState != CursorLockMode.Locked && config.AllowCursorLock &&
                                Input.mousePosition != (new Vector3(0f, 0f, 0f)))
                        {
                            Vector3 predicted = Input.mousePosition + mouseD;
                            if (predicted.x < 0f || predicted.x >= Screen.width ||
                                    predicted.y < 0f || predicted.y >= Screen.height)
                            {
                                Cursor.lockState = CursorLockMode.Locked;
                                //Cursor.visible = false;
                            }
                        }
                    }
                }
                if (!IsButtonDown() && oldIsButtonDown && config.AllowCursorLock)
                {
                    Cursor.lockState = CursorLockMode.None;
                    //Cursor.visible = true;
                }
                if (windowHasFocus)
                {
                    if (Cursor.lockState == CursorLockMode.Locked)
                    {
                        oldMousePosition = new Vector3(Screen.width / 2f, Screen.height / 2f);
                    }
                    else
                    {
                        oldMousePosition = Input.mousePosition;
                    }
                    oldIsButtonDown = IsButtonDown();
                }
            }
        }

        public void OnApplicationFocus(bool focusStatus)
        {
            windowHasFocus = focusStatus;
            if (!windowHasFocus)
            {
                Cursor.lockState = CursorLockMode.None;
                //Cursor.visible = true;
                oldMousePosition = new Vector3(-1, -1, -1);
            }
        }
    }

}
