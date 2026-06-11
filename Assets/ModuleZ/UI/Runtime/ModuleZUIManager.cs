using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ModuleZ.UI.Runtime
{
    public class ModuleZUIManager : MonoBehaviour
    {
        public static ModuleZUIManager Instance { get; private set; }

        private Canvas currentCanvas;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            EnsureEventSystem();

            Debug.Log("[Module Z] UIManager iniciado correctamente.");
        }

        public Canvas CreateCanvas(string canvasName)
        {
            DestroyCurrentCanvas();
            EnsureEventSystem();

            GameObject canvasObject = new GameObject(canvasName);
            canvasObject.transform.SetParent(transform, false);

            currentCanvas = canvasObject.AddComponent<Canvas>();
            currentCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            currentCanvas.sortingOrder = 100;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();

            return currentCanvas;
        }

        public void DestroyCurrentCanvas()
        {
            if (currentCanvas != null)
            {
                Destroy(currentCanvas.gameObject);
                currentCanvas = null;
            }
        }

        private void EnsureEventSystem()
        {
            if (FindObjectOfType<EventSystem>() != null)
                return;

            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
            DontDestroyOnLoad(eventSystem);
        }
    }
}