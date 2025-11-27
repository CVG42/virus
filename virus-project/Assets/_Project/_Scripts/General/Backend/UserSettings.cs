using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Virus
{
    public class UserSettings : MonoBehaviour
    {
        private const int DEBOUNCE_DELAY_MS = 300;
        
        private CancellationTokenSource _debounceCTS;

        private void Start()
        {
            AudioManager.Source.OnMasterVolumeChange += OnAudioSettingChanged;
            AudioManager.Source.OnMusicVolumeChange += OnAudioSettingChanged;
            AudioManager.Source.OnSFXVolumeChange += OnAudioSettingChanged;

            LocalizationManager.Source.OnLanguageChanging += SaveLanguageSetting;
        }

        private void OnDestroy()
        {
            AudioManager.Source.OnMasterVolumeChange -= OnAudioSettingChanged;
            AudioManager.Source.OnMusicVolumeChange -= OnAudioSettingChanged;
            AudioManager.Source.OnSFXVolumeChange -= OnAudioSettingChanged;

            LocalizationManager.Source.OnLanguageChanging -= SaveLanguageSetting;

            _debounceCTS?.Cancel();
        }

        private void OnAudioSettingChanged(float _)
        {
            DebounceSave();
        }

        private async void DebounceSave()
        {
            _debounceCTS?.Cancel();
            _debounceCTS = new CancellationTokenSource();

            try
            {
                await UniTask.Delay(DEBOUNCE_DELAY_MS, cancellationToken: _debounceCTS.Token);
            }
            catch
            {
                return;
            }

            await SaveAudioSettings();
        }

        private async UniTask SaveAudioSettings()
        {
            int userId = PlayerPrefs.GetInt("UserID", 0);
            if (userId == 0) return;

            await UserAPI.UpdateUserSettings(
                userId,
                AudioManager.Source.CurrentSFXVolume,
                AudioManager.Source.CurrentMusicVolume,
                AudioManager.Source.CurrentMasterVolume,
                LocalizationManager.Source.CurrentLanguage
            );
        }

        private async void SaveLanguageSetting(string newLang)
        {
            int userId = PlayerPrefs.GetInt("UserID", 0);
            if (userId == 0) return;

            await UserAPI.UpdateUserSettings(
                userId,
                AudioManager.Source.CurrentSFXVolume,
                AudioManager.Source.CurrentMusicVolume,
                AudioManager.Source.CurrentMasterVolume,
                newLang
            );
        }
    }
}
