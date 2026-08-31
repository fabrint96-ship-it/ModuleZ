using ModuleZ.Core.Managers;
using ModuleZ.OpenWorld.Encounters;
using UnityEngine;

namespace ModuleZ.Duel3D.Audio
{
    public class Duel3DMusicController : MonoBehaviour
    {
        private AudioSource musicSource;

        private void Awake()
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.spatialBlend = 0f;
            musicSource.volume = 0.55f;
        }

        public void PlayCurrentDuelMusic()
        {
            string path = GetMusicPath();

            AudioClip clip = Resources.Load<AudioClip>(path);

            if (clip == null)
            {
                Debug.LogWarning("[ModuleZ Duel Music] No encontrada: " + path);
                return;
            }

            musicSource.clip = clip;
            musicSource.Play();

            Debug.Log("[ModuleZ Duel Music] Reproduciendo: " + path);
        }

        public void StopMusic()
        {
            if (musicSource != null)
                musicSource.Stop();
        }

        private string GetMusicPath()
        {
            switch (ModuleZGameState.CurrentDuelRival)
            {
                case ModuleZRivalId.Madrid:
                    return "Audio/Music/Duel/MUS_Duel_Madrid70s";

                case ModuleZRivalId.Barcelona:
                    return "Audio/Music/Duel/MUS_Duel_Barcelona70s";

                case ModuleZRivalId.Valencia:
                    return "Audio/Music/Duel/MUS_Duel_Valencia70s";

                case ModuleZRivalId.Andalucia:
                    return "Audio/Music/Duel/MUS_Duel_Andalucia70s";

                default:
                    return "Audio/Music/Duel/MUS_Duel_Madrid70s";
            }
        }
    }
}