using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class APIController : MonoBehaviour
{
    public Camera drawCamera;
    [SerializeField]
    private Text outputText;
    private const string URL = "https://ronejfourn.pythonanywhere.com/";

    public void GenerateRequest()
    {
        StartCoroutine(ProcessRequest(URL));
    }

    private IEnumerator ProcessRequest(string uri)
    {
        var form = new WWWForm();
        RenderTexture rt = drawCamera.targetTexture;
        Texture2D t2D = new Texture2D(rt.width, rt.height);
        RenderTexture.active = rt;
        t2D.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        t2D.Apply();
        RenderTexture.active = null;
        byte[] bytes = t2D.EncodeToPNG();
        form.AddBinaryData("image", bytes, "guessme.png", "image/png");

        using (UnityWebRequest request = UnityWebRequest.Post(uri, form))
        {
            request.timeout = 10;
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                outputText.text = request.downloadHandler.text;
            }
            else
            {
                outputText.text = "Error";
                Debug.Log(request.error);
            }
        }
    }
}
