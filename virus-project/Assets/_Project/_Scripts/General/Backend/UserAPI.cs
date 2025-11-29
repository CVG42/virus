using System;
using System.Globalization;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class UserAPI
{
    private static string baseUrl = "https://fral117.com/api/";

    public static async UniTask<Response> RegisterUser(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        using var request = UnityWebRequest.Post(baseUrl + "register_user.php", form);
        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
            return new Response { success = false, message = "Connection error" };
        }

        Debug.Log("Response: " + request.downloadHandler.text);
        var response = JsonUtility.FromJson<Response>(request.downloadHandler.text);
        return response;
    }

    public static async UniTask<Response> LoginUser(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        using var request = UnityWebRequest.Post(baseUrl + "login_user.php", form);
        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
            return new Response { success = false, message = "Connection error" };
        }

        var response = JsonUtility.FromJson<Response>(request.downloadHandler.text);
        return response;
    }

    public static async UniTask<UserSettingsResponse> GetUserSettings(int userId)
    {
        WWWForm form = new WWWForm();
        form.AddField("user_id", userId);
        form.AddField("action", "get");

        using var request = UnityWebRequest.Post(baseUrl + "user_settings.php", form);
        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
            return new UserSettingsResponse { success = false, message = "Connection error" };
        }

        var response = JsonUtility.FromJson<UserSettingsResponse>(request.downloadHandler.text);
        return response;
    }

    public static async UniTask<Response> UpdateUserSettings(int userId, float sound, float music, float master, string language)
    {
        WWWForm form = new WWWForm();
        form.AddField("user_id", userId);
        form.AddField("action", "update");
        form.AddField("sound_volume", sound.ToString(CultureInfo.InvariantCulture));
        form.AddField("music_volume", music.ToString(CultureInfo.InvariantCulture));
        form.AddField("master_volume", master.ToString(CultureInfo.InvariantCulture));
        form.AddField("game_language", language);

        using var request = UnityWebRequest.Post(baseUrl + "user_settings.php", form);
        await request.SendWebRequest();

        Debug.Log("RAW UPDATE SETTINGS RESPONSE:\n" + request.downloadHandler.text);

        if (request.result != UnityWebRequest.Result.Success)
            return new Response { success = false, message = "Connection error" };

        return JsonUtility.FromJson<Response>(request.downloadHandler.text);
    }

    public static async UniTask<SessionStartResponse> StartSession(int userId)
    {
        WWWForm form = new WWWForm();
        form.AddField("user_id", userId);
        form.AddField("action", "start");

        var request = UnityWebRequest.Post(baseUrl + "session_api.php", form);
        await request.SendWebRequest();

        Debug.Log("START SESSION RAW: " + request.downloadHandler.text);

        return JsonUtility.FromJson<SessionStartResponse>(request.downloadHandler.text);
    }

    public static async UniTask<Response> EndSession(int userId, int sessionId)
    {
        WWWForm form = new WWWForm();
        form.AddField("user_id", userId);
        form.AddField("sid", sessionId);
        form.AddField("action", "end");

        var request = UnityWebRequest.Post(baseUrl + "session_api.php", form);
        await request.SendWebRequest();

        Debug.Log("END SESSION RAW: " + request.downloadHandler.text);

        return JsonUtility.FromJson<Response>(request.downloadHandler.text);
    }

    public static async UniTask<UserProgressResponse> GetProgress(int userId, int levelId)
{
    WWWForm form = new WWWForm();
    form.AddField("action", "get");
    form.AddField("user_id", userId);
    form.AddField("level_id", levelId);

    var req = UnityWebRequest.Post(baseUrl + "user_progress.php", form);
    await req.SendWebRequest();

    return JsonUtility.FromJson<UserProgressResponse>(req.downloadHandler.text);
}

    public static async UniTask<Response> UpdateCookies(int userId, int levelId, int cookies)
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "update_cookies");
        form.AddField("user_id", userId);
        form.AddField("level_id", levelId);
        form.AddField("cookies", cookies);

        var req = UnityWebRequest.Post(baseUrl + "user_progress.php", form);
        await req.SendWebRequest();

        return JsonUtility.FromJson<Response>(req.downloadHandler.text);
    }

    public static async UniTask<Response> CompleteLevel(int userId, int levelId, int cookies, int time)
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "complete_level");
        form.AddField("user_id", userId);
        form.AddField("level_id", levelId);
        form.AddField("cookies", cookies);
        form.AddField("time_seconds", time);

        var req = UnityWebRequest.Post(baseUrl + "user_progress.php", form);
        await req.SendWebRequest();

        return JsonUtility.FromJson<Response>(req.downloadHandler.text);
    }

    public static async UniTask<GetAchievementsResponse> GetAchievements(int userId)
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "get");
        form.AddField("user_id", userId);

        var request = UnityWebRequest.Post(baseUrl + "achievements_api.php", form);
        await request.SendWebRequest();

        Debug.Log("ACHIEVEMENTS RAW: " + request.downloadHandler.text);

        return JsonUtility.FromJson<GetAchievementsResponse>(request.downloadHandler.text);
    }

    public static async UniTask<Response> UnlockAchievement(int userId, string achievementId)
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "unlock");
        form.AddField("user_id", userId);
        form.AddField("achievement_id", achievementId);

        var request = UnityWebRequest.Post(baseUrl + "achievements_api.php", form);
        await request.SendWebRequest();

        Debug.Log("UNLOCK RAW: " + request.downloadHandler.text);

        return JsonUtility.FromJson<Response>(request.downloadHandler.text);
    }

    [System.Serializable]
    public class AchievementData
    {
        public string achievement_id;
        public string name;
        public string description;
        public int unlocked;
        public string unlocked_at;
    }

    [System.Serializable]
    public class GetAchievementsResponse
    {
        public bool success;
        public AchievementData[] achievements;
    }

    [System.Serializable]
    public class UserProgressResponse
    {
        public bool success;
        public ProgressData progress;
    }

    [System.Serializable]
    public class SessionStartResponse
    {
        public bool success;
        public string message;
        public int sid;
    }

    [System.Serializable]
    public class Response
    {
        public bool success;
        public string message;
        public int user_id;
        public string username;
    }

    [System.Serializable]
    public class UserSettingsResponse
    {
        public bool success;
        public string message;
        public float sound_volume;
        public float music_volume;
        public float master_volume;
        public string game_language;
    }
}

[System.Serializable]
public class ProgressData
{
    public int progress_id;
    public int user_id;
    public int level_id;
    public int cookies_collected;
    public int best_cookies_collected;
    public int completion_time_seconds;
    public string completed_at;
}
