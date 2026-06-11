using ModuleZ.UI.MainMenu;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ModuleZ.Core.SceneLoading
{
    public class ModuleZSceneController : MonoBehaviour
    {
        public static ModuleZSceneController Instance { get; private set; }

        public const string BootScene = "Boot";
        public const string OpenWorldScene = "OpenWorld";
        public const string DuelScene = "Duel";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;

            Debug.Log("[Module Z] SceneController iniciado correctamente.");
        }

        private void OnDestroy()
        {
            if (Instance == this)
                SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == BootScene)
            {
                ModuleZMainMenuBuilder menuBuilder = FindObjectOfType<ModuleZMainMenuBuilder>();

                if (menuBuilder != null)
                    menuBuilder.ShowMainMenu();
            }
        }

        public void LoadOpenWorld()
        {
            LoadScene(OpenWorldScene);
        }

        public void LoadDuel()
        {
            LoadScene(DuelScene);
        }

        public void LoadBoot()
        {
            LoadScene(BootScene);
        }

        public void ReturnToMainMenu()
        {
            Time.timeScale = 1f;
            LoadScene(BootScene);
        }

        private void LoadScene(string sceneName)
        {
            Debug.Log("[Module Z] Cargando escena: " + sceneName);
            SceneManager.LoadScene(sceneName);
        }
    }
}