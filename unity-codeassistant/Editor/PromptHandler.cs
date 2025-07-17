using System;
using UnityEditor;

namespace UnityCodeAssistant
{
    public static class PromptHandler
    {
        public static void GenerateScript(string prompt, string folderPath, Action<bool, string> callback)
        {
            string wrappedPrompt = $@"
You are a Unity C# developer assistant.

ONLY return Unity C# MonoBehaviour code.

Do NOT include:
- Explanations
- Steps
- Markdown (``` formatting)

Only return valid C# code with inline comments.

Prompt: {prompt}
";

            OpenAIService.SendPrompt(wrappedPrompt, (success, response) =>
            {
                if (!success || string.IsNullOrEmpty(response))
                {
                    callback?.Invoke(false, "Failed to get a valid response from OpenAI.");
                    return;
                }

                // Extract code block manually if markdown exists
                int codeStart = response.IndexOf("```csharp");
                if (codeStart == -1) codeStart = response.IndexOf("```");

                if (codeStart != -1)
                {
                    int codeEnd = response.IndexOf("```", codeStart + 3);
                    if (codeEnd != -1)
                    {
                        response = response.Substring(codeStart + 9, codeEnd - (codeStart + 9)).Trim();
                    }
                }

                string fileName = "GeneratedScript.cs"; // You can add filename extraction later
                bool result = ScriptWriter.SaveScript(response, folderPath, fileName, out string message);
                callback?.Invoke(result, message);
            });
        }
        // Analyze an existing script for issues
        public static void AnalyzeScript(string scriptCode, Action<bool, string> callback)
        {
            string analysisPrompt = $@"
                You are a Unity C# expert.

                Analyze the following MonoBehaviour script for:
                - Compilation errors
                - Logical bugs
                - Common Unity mistakes
                - Poor coding practices

                If needed, return a corrected version of the script.

                Script:
                ```csharp
                {scriptCode}
                ";
            OpenAIService.SendPrompt(analysisPrompt, callback);
        }

    }
}
