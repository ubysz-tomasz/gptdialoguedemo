using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GPTAPIClient : MonoBehaviour
{
    [Serializable]
    private class GPTResponse
    {
        public Choice[] choices;
    }

    [Serializable]
    private class Choice
    {
        public Message message;
    }

    [Serializable]
    private class GPTPayload
    {
        public string model;
        public Message[] messages;
    }

    [Serializable]
    private class Message
    {
        public string role;
        public string content;
    }

    [SerializeField] private string _model = "gpt-3.5-turbo";
    [SerializeField] private string _endpoint = "https://api.openai.com/v1/chat/completions";
    [SerializeField] private string _key = "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";

    private List<Message> _messagesSoFar = new();

    public void RequestResponse(string prompt, Action<string> onResponseReady)
    {
        StartCoroutine(GenerateResponse(prompt, onResponseReady));
    }

    private IEnumerator GenerateResponse(string prompt, Action<string> callback)
    {
        _messagesSoFar.Add(new Message { role = "user", content = prompt });
        
        GPTPayload payload = new GPTPayload
        {
            model = _model,
            messages = _messagesSoFar.ToArray()
        };

        string jsonPayload = JsonUtility.ToJson(payload);

        using UnityWebRequest request = new UnityWebRequest(_endpoint, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {_key}");
            
        yield return request.SendWebRequest();

        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
            Debug.LogError("Error: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text);
        }
        else
        {
            string jsonResponse = request.downloadHandler.text;
            GPTResponse response = JsonUtility.FromJson<GPTResponse>(jsonResponse);
            
            _messagesSoFar.Add(new Message
            {
                role = response.choices[0].message.role,
                content = response.choices[0].message.content
            });
            
            callback(response.choices[0].message.content);
        }
    }
}
