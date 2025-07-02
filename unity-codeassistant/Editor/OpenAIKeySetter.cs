using UnityEditor;
using UnityEngine;

public class OpenAIKeySetter : EditorWindow
{
    private string apiKey = "";

    [MenuItem("Tools/OpenAI/Set API Key")]
    public static void ShowWindow()
    {
        GetWindow<OpenAIKeySetter>("OpenAI API Key");
    }

    private void OnGUI()
    {
        GUILayout.Label("Set your OpenAI API Key", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        apiKey = EditorGUILayout.TextField("API Key", apiKey);

        if (GUILayout.Button("Save"))
        {
            if (!string.IsNullOrEmpty(apiKey))
            {
                EditorPrefs.SetString("OpenAI_API_Key", apiKey);
                Debug.Log("✅ OpenAI API Key saved.");
                Close();
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "API key cannot be empty.", "OK");
            }
        }
    }
}
// This script creates a Unity Editor window to set the OpenAI API key.
// It saves the key in EditorPrefs for later use by the OpenAI service.