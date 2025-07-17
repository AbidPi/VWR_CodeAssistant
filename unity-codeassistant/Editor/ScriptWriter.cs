using System;
using System.IO;

namespace UnityCodeAssistant
{
    public static class ScriptWriter
    {
        public static bool SaveScript(string scriptContent, string fullPath, out string message)
        {
            try
            {
                // Auto-strip markdown if somehow still included
                if (scriptContent.StartsWith("```"))
                {
                    int start = scriptContent.IndexOf('\n');
                    int end = scriptContent.LastIndexOf("```");
                    if (start != -1 && end != -1 && end > start)
                    {
                        scriptContent = scriptContent.Substring(start + 1, end - start - 1).Trim();
                    }
                }

                File.WriteAllText(fullPath, scriptContent);
                message = $"Script saved to: {fullPath}";
                UnityEditor.AssetDatabase.Refresh(); // Refresh Unity to see the new file
                return true;
            }
            catch (Exception ex)
            {
                message = $"Failed to save script: {ex.Message}";
                return false;
            }
        }
    }
}
