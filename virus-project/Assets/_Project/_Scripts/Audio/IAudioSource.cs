using System;
using UnityEngine;

namespace Virus
{
    public interface IAudioSource
    {
        event Action<float> OnMasterVolumeChange;
        event Action<float> OnSFXVolumeChange;
        event Action<float> OnMusicVolumeChange;

        float CurrentMasterVolume { get; }
        float CurrentSFXVolume { get; }
        float CurrentMusicVolume { get; }
        void SetMasterVolume(float volume);
        void SetSFXVolume(float volume);
        void SetMusicVolume(float volume);

        void PlayLevelMusic(string audioName);
        void PlayOneShot(string audioName);

        void PlayJumpSFX();
        void PlayCookieSFX();
        void PlayShootSFX();
        void PlayLaserDamageSFX();
        void PlayLaserOffSFX();
        void PlayEnemyShootSFX();
        void PlayAmbientAudio(string audioName);
        void StopAmbientAudio();
    }
}
