using UnityEditor;
using UnityEngine;

public class ResetAPI
{
    [MenuItem("Tools/OpenAI/Clear API Key")]
    public static void ClearAPIKey()
    {
        EditorPrefs.DeleteKey("OpenAI_API_Key");
        Debug.Log("🗑️ OpenAI API Key cleared from EditorPrefs.");
    }
}
// This script provides a menu item to clear the OpenAI API key from EditorPrefs.
// It logs a confirmation message when the key is successfully cleared. 