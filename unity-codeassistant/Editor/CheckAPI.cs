using UnityEditor;
using UnityEngine;

public class OpenAIKeyDebugger
{
    [MenuItem("Tools/OpenAI/Check API Key")]
    public static void CheckAPIKey()
    {
        string apiKey = EditorPrefs.GetString("OpenAI_API_Key", "");

        if (string.IsNullOrEmpty(apiKey))
        {
            Debug.LogWarning("❌ No OpenAI API Key found in EditorPrefs.");
        }
        else
        {
            Debug.Log($"✅ OpenAI API Key is set: {apiKey.Substring(0, 5)}... (truncated)");
        }
    }
}
// This script provides a menu item to check if the OpenAI API key is set in EditorPrefs.
// It logs a warning if the key is missing or confirms its presence with a truncated version for security.