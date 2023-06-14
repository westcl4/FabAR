using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class OctoPrintSessionHandler
{
    private readonly HttpClient _httpClient;

    public OctoPrintSessionHandler()
    {
        _httpClient = new HttpClient();
    }

    public IEnumerator LoginAsync(string username, string password, Action<string> onSuccess)
    {
        var requestUrl = "https://homer.local/api/login/login";
        var content = new FormUrlEncodedContent(new[]
        {
        new KeyValuePair<string, string>("username", username),
        new KeyValuePair<string, string>("password", password)
    });

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes("username:password")));

        var response = _httpClient.PostAsync(requestUrl, content);
        while (!response.IsCompleted) yield return null;
        response.Result.EnsureSuccessStatusCode();

        var responseContent = response.Result.Content.ReadAsStringAsync();
        while (!responseContent.IsCompleted) yield return null;
        var sessionData = JsonUtility.FromJson<SessionData>(responseContent.Result);

        onSuccess(sessionData.session_id);
    }

    [Serializable]
    public class SessionData
    {
        public string SessionId { get; set; }
    }
}
