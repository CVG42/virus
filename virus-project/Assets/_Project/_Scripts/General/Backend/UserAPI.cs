using System.Globalization;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class UserAPI
{
    private static string baseUrl = "http://localhost/unity/";

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
