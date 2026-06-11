using ModuleZ.Duel.Runtime;
using UnityEngine;

namespace ModuleZ.Duel.Rules
{
    public class DuelMusicController : MonoBehaviour
    {
        private AudioSource musicSource;

        private void Awake()
        {
            musicSource = gameObject.AddComponent<AudioSource>();

            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.spatialBlend = 0f;
            musicSource.volume = 0.6f;
        }

        public void PlayThemeMusic(DuelThemeData themeData)
        {
            if (themeData == null)
                return;

            AudioClip clip =
                Resources.Load<AudioClip>(themeData.musicResourcePath);

            if (clip == null)
            {
                Debug.Log(
                    "[Module Z Music] No encontrada: " +
                    themeData.musicResourcePath
                );

                return;
            }

            musicSource.clip = clip;
            musicSource.Play();
        }

        public void StopMusic()
        {
            musicSource.Stop();
        }
    }
}