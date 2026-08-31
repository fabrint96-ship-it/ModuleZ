using UnityEngine;

namespace ModuleZ.Duel3D.Audio
{
    public class Duel3DAudioFeedbackController : MonoBehaviour
    {
        [Header("Audio Clips")]
        [SerializeField] private AudioClip placeClip;
        [SerializeField] private AudioClip removeClip;
        [SerializeField] private AudioClip invalidClip;
        [SerializeField] private AudioClip turnClip;
        [SerializeField] private AudioClip victoryClip;
        [SerializeField] private AudioClip defeatClip;

        [Header("Settings")]
        [SerializeField] private float volume = 0.8f;

        private AudioSource audioSource;
        private const string AudioPath = "Audio/SFX/Duel3D/";

        private void Awake()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
            audioSource.volume = volume;

            LoadAudio();
        }

        private void LoadAudio()
        {
            placeClip =
                Resources.Load<AudioClip>(
                    AudioPath + "SFX_Duel3D_Place");

            removeClip =
                Resources.Load<AudioClip>(
                    AudioPath + "SFX_Duel3D_Remove");

            invalidClip =
                Resources.Load<AudioClip>(
                    AudioPath + "SFX_Duel3D_Invalid");

            turnClip =
                Resources.Load<AudioClip>(
                    AudioPath + "SFX_Duel3D_Turn");

            victoryClip =
                Resources.Load<AudioClip>(
                    AudioPath + "SFX_Duel3D_Victory");

            defeatClip =
                Resources.Load<AudioClip>(
                    AudioPath + "SFX_Duel3D_Defeat");
        }

        public void PlayPlace()
        {
            Play(placeClip);
        }

        public void PlayRemove()
        {
            Play(removeClip);
        }

        public void PlayInvalid()
        {
            Play(invalidClip);
        }

        public void PlayTurn()
        {
            Play(turnClip);
        }

        public void PlayVictory()
        {
            Play(victoryClip);
        }

        public void PlayDefeat()
        {
            Play(defeatClip);
        }

        private void Play(AudioClip clip)
        {
            if (clip == null || audioSource == null)
                return;

            audioSource.PlayOneShot(clip, volume);
        }
    }
}