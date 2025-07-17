using System;
using System.IO;
using UnityEditor;

namespace UnityCodeAssistant
{
    public static class PromptHandler
    {
        public static void GenerateScript(string prompt, string folderPath, string filename, Action<bool, string> callback)
        {
            if (string.IsNullOrEmpty(filename))
            {
                callback?.Invoke(false, "Filename is required.");
                return;
            }

            // ⚠️ FIX: Do NOT hardcode this value. Use provided filename:
            string savePath = Path.Combine(folderPath, filename.EndsWith(".cs") ? filename : filename + ".cs");

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

                bool result = ScriptWriter.SaveScript(response, savePath, out string message);
                callback?.Invoke(result, message);
            });
        }

        // Analyze method unchanged
        public static void AnalyzeScript(string scriptCode, Action<bool, string> callback)
        {
            string analysisPrompt = $@"
You are a Unity C# assistant.

Refactor the following Unity MonoBehaviour script by:
- Fixing any compilation or logic issues
- Applying Unity best practices
- Adding inline comments where needed

⚠️ Return only clean Unity C# code — no explanations, no markdown, no ```csharp formatting.

Script:
{scriptCode}
";

            OpenAIService.SendPrompt(analysisPrompt, callback);
        }
    }
}
