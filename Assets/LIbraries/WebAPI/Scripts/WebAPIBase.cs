using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WebAPIBase
{
    public static IEnumerator Get<T>(string url, System.Action<T> success, System.Action<string> error)
    {
        using (var request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                error(request.error);
                yield break;
            }

            success(JsonUtility.FromJson<T>(request.downloadHandler.text));
        }
    }
}
