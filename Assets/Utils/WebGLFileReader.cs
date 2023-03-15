using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.Networking;

public class WebGLFileReader : MonoBehaviour
{
    public static IEnumerator Read(string path, System.Action<string> callback)
    {
        var uri = new System.Uri(Path.Combine(Application.streamingAssetsPath, path));
        UnityWebRequest www = UnityWebRequest.Get(uri);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            string jsonStr = www.downloadHandler.text;
            callback(jsonStr);
        }

    }
}
