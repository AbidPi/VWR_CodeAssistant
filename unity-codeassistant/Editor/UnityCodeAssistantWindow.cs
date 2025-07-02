using UnityEngine;
using UnityEditor;
using System.IO;

namespace UnityCodeAssistant
{
    public class UnityCodeAssistantWindow : EditorWindow
    {
        private MonoScript selectedScript;
        private string prompt = "";
        private DefaultAsset folder;
        private string statusMessage = "";

        [MenuItem("Tools/Unity Code Assistant")]
        public static void ShowWindow()
        {
            GetWindow<UnityCodeAssistantWindow>("Code Assistant");
        }

        void OnGUI()
        {
            GUILayout.Label("Generate Unity C# Script", EditorStyles.boldLabel);

            EditorGUILayout.LabelField("Enter your prompt:");
            prompt = EditorGUILayout.TextArea(prompt, GUILayout.Height(60));

            EditorGUILayout.Space();
            folder = (DefaultAsset)EditorGUILayout.ObjectField("Save Folder", folder, typeof(DefaultAsset), false);

            if (GUILayout.Button("Generate Script"))
            {
                if (string.IsNullOrEmpty(prompt) || folder == null)
                {
                    statusMessage = "Please enter a prompt and choose a folder.";
                }
                else
                {
                    string folderPath = AssetDatabase.GetAssetPath(folder);
                    PromptHandler.GenerateScript(prompt, folderPath, OnComplete);
                    statusMessage = "Generating script...";
                }
            }

            if (!string.IsNullOrEmpty(statusMessage))
            {
                EditorGUILayout.HelpBox(statusMessage, MessageType.Info);
            }
            //adding file selection for existing script review
            GUILayout.Space(20);
            GUILayout.Label("Review Existing Script", EditorStyles.boldLabel);

            selectedScript = (MonoScript)EditorGUILayout.ObjectField("Script File", selectedScript, typeof(MonoScript), false);

            if (selectedScript != null)
            {
                if (GUILayout.Button("Load Script"))
                {
                    string path = AssetDatabase.GetAssetPath(selectedScript);

                    if (File.Exists(path))
                    {
                        string scriptCode = File.ReadAllText(path);
                        Debug.Log($"Loaded script from: {path}");
                        Debug.Log(scriptCode);
                        statusMessage = "Script loaded. Ready for analysis.";
                    }
                    else
                    {
                        Debug.LogError("Could not find the selected file.");
                        statusMessage = "Failed to load the script.";
                    }
                }
            }
        }

        private void OnComplete(bool success, string message)
        {
            statusMessage = message;
        }
    }
}
// This script creates a Unity Editor window for the Code Assistant tool.
// It allows users to input a prompt and select a folder to save the generated script.