using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Virus.Flow;

namespace Virus
{
    public class CRUD : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _usernameInput;
        [SerializeField] private TMP_InputField _passwordInput;
        [SerializeField] private TextMeshProUGUI _feedbackText;
        [SerializeField] private Button _registerButton;
        [SerializeField] private Button _loginButton;

        [Header("Redirection settings")]
        [SerializeField] private string _sceneName;

        private void Start()
        {
            _registerButton.onClick.AddListener(OnRegisterButton);
            _loginButton.onClick.AddListener(OnLoginButton);
        }

        public async void OnRegisterButton()
        {
            _feedbackText.text = "Registering...";
            var response = await UserAPI.RegisterUser(_usernameInput.text, _passwordInput.text);

            if (response.success)
            {
                _feedbackText.text = "Account created! Logging in...";

                var loginResponse = await UserAPI.LoginUser(_usernameInput.text, _passwordInput.text);

                if (loginResponse.success)
                {
                    PlayerPrefs.SetInt("UserID", loginResponse.user_id);
                    PlayerPrefs.SetString("Username", loginResponse.username);
                    PlayerPrefs.Save();

                    var settingsResponse = await UserAPI.GetUserSettings(loginResponse.user_id);
                    if (settingsResponse.success)
                    {
                        PlayerPrefs.SetFloat("SFXVolume", settingsResponse.sound_volume);
                        PlayerPrefs.SetFloat("MusicVolume", settingsResponse.music_volume);
                        PlayerPrefs.SetFloat("MasterVolume", settingsResponse.master_volume);

                        LocalizationManager.Source.SetLanguage(LanguageMap.FromDB(settingsResponse.game_language));

                        PlayerPrefs.Save();

                        Debug.Log("Default settings created for new user.");
                    }

                    await UniTask.Delay(800);
                    FlowManager.Source.LoadScene(_sceneName);
                }
            }
            else
            {
                _feedbackText.text = response.message;
            }
        }

        public async void OnLoginButton()
        {
            _feedbackText.text = "Logging in...";
            var response = await UserAPI.LoginUser(_usernameInput.text, _passwordInput.text);

            if (response.success)
            {
                _feedbackText.text = "Welcome " + response.username + "!";
                PlayerPrefs.SetInt("UserID", response.user_id);
                PlayerPrefs.SetString("Username", response.username);
                PlayerPrefs.Save();

                var settingsResponse = await UserAPI.GetUserSettings(response.user_id);
                if (settingsResponse.success)
                {
                    PlayerPrefs.SetFloat("SFXVolume", settingsResponse.sound_volume);
                    PlayerPrefs.SetFloat("MusicVolume", settingsResponse.music_volume);
                    PlayerPrefs.SetFloat("MasterVolume", settingsResponse.master_volume);

                    LocalizationManager.Source.SetLanguage(LanguageMap.FromDB(settingsResponse.game_language));

                    PlayerPrefs.Save();

                    Debug.Log("User settings loaded from database.");
                }
                else
                {
                    Debug.LogWarning("No settings found for user, using defaults.");
                }

                await UniTask.Delay(800);
                FlowManager.Source.LoadScene(_sceneName);
            }
            else
            {
                _feedbackText.text = response.message;
            }
        }

        [System.Serializable]
        private class LoginResponse
        {
            public bool success;
            public string message;
            public int user_id;
            public string username;
        }
    }
}
