using UnityEngine;
using UnityEditor;

namespace UnityCodeAssistant
{
    public class UnityCodeAssistantWindow : EditorWindow
    {
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
        }

        private void OnComplete(bool success, string message)
        {
            statusMessage = message;
        }
    }
}
// This script creates a Unity Editor window for the Code Assistant tool.
// It allows users to input a prompt and select a folder to save the generated script.