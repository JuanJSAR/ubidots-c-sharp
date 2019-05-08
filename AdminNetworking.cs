using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class AdminNetworking
{
    public struct Headers { public string name; public string value; }
    public enum MethodsHTTP { GET, HEAD, POST, PUT, DELETE, CONNECT, OPTIONS, TRACE, PATCH };
    public static IEnumerator SendRequest(string url, MethodsHTTP type, System.Action<bool, string> outputMessage, string arguments = "", string json = "", string token = "", UploadHandler handler = null, List<Headers> headers = null, int timeout = -1, DownloadHandler downloadHandler = null)
    {
        Debug.Log(url + arguments);

        var www = new UnityWebRequest(url + arguments, type.ToString());

        if (timeout < 1)
        {
            www.timeout = 2;
        }
        else
        {
            www.timeout = timeout;
        }

        if (handler != null)
        {
            www.uploadHandler = handler;
        }
        else if (!string.IsNullOrEmpty(json))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            www.SetRequestHeader("Content-Type", "application/json");
        }

        if (!string.IsNullOrEmpty(token))
        {
            www.SetRequestHeader("authorization", token);
        }

        if (headers != null && headers.Count > 0)
        {
            headers.ForEach((x) => www.SetRequestHeader(x.name, x.value));
        }

        if (downloadHandler == null)
        {
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        }
        else
        {
            www.downloadHandler = downloadHandler;
        }

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            outputMessage(false, Encoding.UTF8.GetString(www.downloadHandler.data));
            Debug.Log(Encoding.UTF8.GetString(www.downloadHandler.data));
        }
        else
        {
            Debug.Log(Encoding.UTF8.GetString(www.downloadHandler.data));
            outputMessage(true, (Encoding.UTF8.GetString(www.downloadHandler.data)));
        }
    }

    public static string PrettyJSON(string text)
    {
        int star = -1;
        int end = -1;

        for (int i = 0; i < text.Length; i++)
        {
            if (star == -1)
            {
                if (text[i] == "{"[0])
                {
                    star = i;
                    break;
                }
            }
        }

        for (int i = text.Length - 1; i > 0; i--)
        {
            if (end == -1)
            {
                if (text[i] == "}"[0])
                {
                    end = i;
                    break;
                }
            }
        }

        return text.Substring(star, (end - star) + 1);
        //tempText = System.Text.RegularExpressions.Regex.Replace(tempText, @"\s(?=([^""]*""[^""]*"")*[^""]*$)", string.Empty);
    }

    public static IEnumerator CheckServerConnection(string url, System.Action<bool> outputMessage)
    {
        WWW www = new WWW(url);
        yield return www;

        if (www.error == null)
        {
            Debug.Log("State: " + true + " - " + System.Text.Encoding.UTF8.GetString(www.bytes));
            outputMessage(true);
        }
        else
        {
            outputMessage(false);
            Debug.Log("State: " + false + " - " + System.Text.Encoding.UTF8.GetString(www.bytes));
        }

        //if (www.isDone && www.bytesDownloaded > 0) { }
        //if (www.isDone && www.bytesDownloaded == 0) { }
    }

    public static bool isInternetConnection() { return (Application.internetReachability != NetworkReachability.NotReachable); }

    /// <summary>
    /// Codifica a "MD5".
    /// </summary>
    public static string EncodeMd5(string strToEncrypt)
    {
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);

        // encrypt bytes
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);

        // Convert the encrypted bytes back to a string (base 16)
        string hashString = "";

        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }

        return hashString.PadLeft(32, '0');
    }

    /// <summary>
    /// Codifica a "Base64".
    /// </summary>
    public static string EncodeBase64(string value)
    {
        byte[] bytesToEncode = Encoding.UTF8.GetBytes(value);
        string encodedText = System.Convert.ToBase64String(bytesToEncode);
        return encodedText;
    }

    /// <summary>
    /// Decodifica de "Base64".
    /// </summary>
    public static string DecodeBase64(string value)
    {
        byte[] decodedBytes = System.Convert.FromBase64String(value);
        string decodedText = Encoding.UTF8.GetString(decodedBytes);
        return decodedText;
    }
}