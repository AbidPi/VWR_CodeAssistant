using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityCodeAssistant
{
    public static class ScriptWriter
    {
        public static bool SaveScript(string scriptContent, string folderPath, string fileName, out string message)
        {
            if (!Directory.Exists(folderPath))
            {
                message = "Target folder path does not exist.";
                return false;
            }

            if (!fileName.EndsWith(".cs"))
            {
                fileName += ".cs";
            }

            string fullPath = Path.Combine(folderPath, fileName);

            try
            {
                File.WriteAllText(fullPath, scriptContent);
                AssetDatabase.Refresh();
                message = $"Script saved to: {fullPath}";
                return true;
            }
            catch (System.Exception e)
            {
                message = $"Failed to save script: {e.Message}";
                return false;
            }
        }
    }
}
// This script provides a utility to save generated Unity C# scripts to a specified folder.
// It checks if the folder exists, appends the .cs extension if necessary, and writes the content to a file.