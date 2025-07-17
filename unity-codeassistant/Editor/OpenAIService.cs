using System;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using Unity.EditorCoroutines.Editor;

namespace UnityCodeAssistant
{
    public static class OpenAIService
    {
        private const string apiUrl = "https://api.openai.com/v1/chat/completions";
        private const string model = "gpt-4o";

        public static void SendPrompt(string prompt, Action<bool, string> callback)
        {
            string apiKey = EditorPrefs.GetString("OpenAI_API_Key", "");

            if (string.IsNullOrEmpty(apiKey))
            {
                callback?.Invoke(false, "API key is missing. Please set it in EditorPrefs.");
                return;
            }

            // Create the actual request object
            ChatRequest chatRequest = new ChatRequest
            {
                model = model,
                messages = new Message[]
                {
            new Message
            {
                role = "user",
                content = prompt
            }
                }
            };

            string jsonBody = JsonUtility.ToJson(chatRequest);

            UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

            EditorCoroutineUtility.StartCoroutineOwnerless(SendRequest(request, callback));
        }
        // Define request and message types
        [Serializable]
        private class ChatRequest
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

        private static IEnumerator SendRequest(UnityWebRequest request, Action<bool, string> callback)
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                callback?.Invoke(false, $"Error: {request.error}");
            }
            else
            {
                try
                {
                    var response = JsonUtility.FromJson<ChatResponse>(request.downloadHandler.text);
                    string content = response.choices[0].message.content.Trim();
                    callback?.Invoke(true, content);
                }
                catch
                {
                    callback?.Invoke(false, "Failed to parse OpenAI response.");
                }
            }
            //Debug.Log("Response JSON: " + request.downloadHandler.text);

        }

        // OpenAI response wrapper

        [Serializable]
        private class ChatResponse
        {
            public Choice[] choices;
        }

        [Serializable]
        private class Choice
        {
            public Message message;
        }

        // JSON workaround wrapper
        [Serializable]
        private class Wrapper
        {
            public string model;
            public Message[] messages;

            public Wrapper(object source)
            {
                var json = JsonUtility.ToJson(source);
                JsonUtility.FromJsonOverwrite(json, this);
            }
        }
    }
}
