using ModuleZ.Duel3D.Core;
using UnityEngine;

namespace ModuleZ.Duel3D.Runtime
{
    public class Duel3DCameraBuilder : MonoBehaviour
    {
        public Camera BuildCamera()
        {
            Camera existingCamera = Camera.main;

            GameObject cameraObject;

            if (existingCamera != null)
            {
                cameraObject = existingCamera.gameObject;
            }
            else
            {
                cameraObject = new GameObject("Duel3D_MainCamera");
                existingCamera = cameraObject.AddComponent<Camera>();
                cameraObject.tag = "MainCamera";
            }

            cameraObject.name = "Duel3D_MainCamera";

            cameraObject.transform.position = new Vector3(
                Duel3DGameRules.CameraPosX,
                Duel3DGameRules.CameraPosY,
                Duel3DGameRules.CameraPosZ
            );

            cameraObject.transform.rotation = Quaternion.Euler(
                Duel3DGameRules.CameraRotX,
                Duel3DGameRules.CameraRotY,
                Duel3DGameRules.CameraRotZ
            );

            existingCamera.fieldOfView = Duel3DGameRules.CameraFieldOfView;
            existingCamera.nearClipPlane = 0.05f;
            existingCamera.farClipPlane = 200f;
            existingCamera.clearFlags = CameraClearFlags.Skybox;

            EnsureAudioListener(cameraObject);
            BuildLight();

            return existingCamera;
        }

        private void EnsureAudioListener(GameObject cameraObject)
        {
            AudioListener[] listeners = FindObjectsOfType<AudioListener>();

            for (int i = 0; i < listeners.Length; i++)
            {
                if (listeners[i].gameObject != cameraObject)
                    Destroy(listeners[i]);
            }

            if (cameraObject.GetComponent<AudioListener>() == null)
                cameraObject.AddComponent<AudioListener>();
        }

        private void BuildLight()
        {
            Light existingLight = FindObjectOfType<Light>();

            GameObject lightObject;

            if (existingLight != null)
            {
                lightObject = existingLight.gameObject;
            }
            else
            {
                lightObject = new GameObject("Duel3D_DirectionalLight");
                existingLight = lightObject.AddComponent<Light>();
            }

            lightObject.name = "Duel3D_DirectionalLight";
            lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            existingLight.type = LightType.Directional;
            existingLight.intensity = 1.4f;
            existingLight.shadows = LightShadows.Soft;
        }
    }
}