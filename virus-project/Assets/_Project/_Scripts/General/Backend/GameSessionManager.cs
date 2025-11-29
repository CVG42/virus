using UnityEngine;
using Cysharp.Threading.Tasks;
using static UserAPI;

using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Virus
{
    public class GameSessionManager : MonoBehaviour
    {
        private static GameSessionManager _instance;
        private bool _sessionEnded = false;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
#endif
        }

        private async void Start()
        {
            int userId = PlayerPrefs.GetInt("UserID", 0);
            if (userId == 0) return;

            var session = await UserAPI.StartSession(userId);

            if (session.success)
            {
                PlayerPrefs.SetInt("SessionID", session.sid);
                Debug.Log("Session started: " + session.sid);
            }
        }

        private void OnApplicationQuit()
        {
            Debug.Log("OnApplicationQuit -> Real EndSession()");
            _ = EndSession();
        }

        private async UniTask EndSession()
        {
            if (_sessionEnded) return;
            _sessionEnded = true;

            int userId = PlayerPrefs.GetInt("UserID", 0);
            int sessionId = PlayerPrefs.GetInt("SessionID", 0);

            if (userId == 0 || sessionId == 0) return;

            Debug.Log("Ending session...");

            Response resp = null;

            try
            {
                resp = await UserAPI.EndSession(userId, sessionId);
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Error EndSession: " + ex.Message);
            }

            if (resp != null && resp.success)
            {
                PlayerPrefs.DeleteKey("SessionID");
                Debug.Log("Session closed successfully.");
            }
            else
            {
                Debug.LogWarning("EndSession not confirmed by server. SessionID won't be deleted.");
            }

            await UniTask.Delay(250);
        }

#if UNITY_EDITOR
        private void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                Debug.Log("Editor exiting play mode: EndSession()");
                _ = EndSession();
            }
        }
#endif
    }
}
