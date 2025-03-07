using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ApiManager : MonoBehaviour
{
    // GASのデプロイURLを設定（実際のURLに置き換えてください）
    private string _apiUrl = "https://script.google.com/macros/s/AKfycbyNpohmRkLkZmauK5KlfIopnzZ6QIv6YAosCfkwmMlfSv8bv3jJeVADicEvuYzEU-KGbA/exec";

    /// <summary>
    /// GAS APIのURLを設定します。
    /// </summary>
    /// <param name="url">新しいAPIのURL</param>
    public void SetApiUrl(string url)
    {
        _apiUrl = url;
        Debug.Log("API URL updated to: " + _apiUrl);
    }

    /// <summary>
    /// 引数を使ってGAS APIを呼び出します。
    /// </summary>
    /// <param name="participantId">ユーザーが入力する4桁のParticipant_Id</param>
    /// <param name="dangerCase">DangerCase ("0", "1", "2" のいずれか)</param>
    /// <param name="elapsedTime">ゲームの経過時間</param>
    /// <param name="timeType">"start", "danger", "detected", "end" のいずれか</param>
    public void CallApi(string participantId, string dangerCase, string elapsedTime, string timeType)
    {
        // クエリパラメータを付与したURLを作成
        string urlWithParams = $"{_apiUrl}?participant_id={participantId}&danger_case={dangerCase}&elapsed_time={elapsedTime}&time_type={timeType}";
        Debug.Log("Calling API: " + urlWithParams);
        StartCoroutine(SendRequest(urlWithParams));
    }

    /// <summary>
    /// UnityWebRequestを使ってGAS APIにGETリクエストを送信します。
    /// </summary>
    /// <param name="url">リクエストURL</param>
    /// <returns></returns>
    private IEnumerator SendRequest(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
#else
            if (request.isNetworkError || request.isHttpError)
#endif
            {
                Debug.LogError("API Request Error: " + request.error);
            }
            else
            {
                Debug.Log("API Request Success: " + request.downloadHandler.text);
            }
        }
    }
}
