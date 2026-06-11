using UnityEngine;

namespace ModuleZ.Duel.Rules
{
    public class DuelAudioFeedbackController : MonoBehaviour
    {
        public static DuelAudioFeedbackController Instance { get; private set; }

        private AudioSource audioSource;

        private AudioClip victoryClip;
        private AudioClip defeatClip;
        private AudioClip abandonClip;
        private AudioClip movePieceClip;
        private AudioClip rotatePieceClip;

        private void Awake()
        {
            Instance = this;

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
            audioSource.volume = 0.8f;

            LoadClips();
        }

        private void LoadClips()
        {
            victoryClip = Resources.Load<AudioClip>("Audio/SFX/Duel/SFX_Duel_Victory");
            defeatClip = Resources.Load<AudioClip>("Audio/SFX/Duel/SFX_Duel_Defeat");
            abandonClip = Resources.Load<AudioClip>("Audio/SFX/Duel/SFX_Duel_Abandon");
            movePieceClip = Resources.Load<AudioClip>("Audio/SFX/Duel/SFX_Duel_MovePiece");
            rotatePieceClip = Resources.Load<AudioClip>("Audio/SFX/Duel/SFX_Duel_RotatePiece");
        }

        public void PlayVictorySound()
        {
            PlayClip(victoryClip, "[Module Z Audio] Sonido victoria Duel.");
        }

        public void PlayDefeatSound()
        {
            PlayClip(defeatClip, "[Module Z Audio] Sonido derrota Duel.");
        }

        public void PlayAbandonSound()
        {
            PlayClip(abandonClip, "[Module Z Audio] Sonido abandono Duel.");
        }

        public void PlayMovePieceSound()
        {
            PlayClip(movePieceClip, "[Module Z Audio] Sonido mover pieza Z.");
        }

        public void PlayRotatePieceSound()
        {
            PlayClip(rotatePieceClip, "[Module Z Audio] Sonido rotar pieza Z.");
        }

        private void PlayClip(AudioClip clip, string fallbackLog)
        {
            if (clip != null)
            {
                audioSource.PlayOneShot(clip);
                return;
            }

            Debug.Log(fallbackLog);
        }
    }
}