using UnityEditor;
using UnityEngine;
using System.IO;

namespace UnityCodeAssistant
{
    public class UnityCodeAssistantWindow : EditorWindow
    {
        private string prompt = "";
        private DefaultAsset folder;
        private string statusMessage = "";
        private MonoScript selectedScript;

        private string originalScriptCode = "";
        private string analyzedCode = "";
        private string filename = "ImprovedScript.cs";

        private Vector2 scrollOriginal;
        private Vector2 scrollModified;
        private bool showDiff = false;

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

            GUILayout.Space(20);
            GUILayout.Label("Review Existing Script", EditorStyles.boldLabel);

            selectedScript = (MonoScript)EditorGUILayout.ObjectField("Script File", selectedScript, typeof(MonoScript), false);

            if (selectedScript != null)
            {
                if (GUILayout.Button("Analyze Script"))
                {
                    string path = AssetDatabase.GetAssetPath(selectedScript);

                    if (File.Exists(path))
                    {
                        originalScriptCode = File.ReadAllText(path);
                        statusMessage = "Analyzing script...";
                        PromptHandler.AnalyzeScript(originalScriptCode, OnComplete);
                        filename = Path.GetFileName(path); // default filename
                    }
                    else
                    {
                        Debug.LogError("Could not find the selected file.");
                        statusMessage = "Failed to load the script.";
                    }
                }
            }

            if (!string.IsNullOrEmpty(analyzedCode))
            {
                GUILayout.Space(20);
                showDiff = EditorGUILayout.Toggle("Show Side-by-Side Diff", showDiff);

                if (showDiff)
                {
                    GUILayout.Label("Original vs Suggested", EditorStyles.boldLabel);
                    GUILayout.BeginHorizontal();

                    // Left: Original Script
                    GUILayout.BeginVertical(GUILayout.Width(position.width / 2 - 10));
                    GUILayout.Label("Original", EditorStyles.miniBoldLabel);
                    scrollOriginal = EditorGUILayout.BeginScrollView(scrollOriginal, GUILayout.Height(300));
                    EditorGUILayout.TextArea(originalScriptCode, GUILayout.ExpandHeight(true));
                    EditorGUILayout.EndScrollView();
                    GUILayout.EndVertical();

                    // Right: Suggested Script (editable)
                    GUILayout.BeginVertical(GUILayout.Width(position.width / 2 - 10));
                    GUILayout.Label("Suggested", EditorStyles.miniBoldLabel);
                    scrollModified = EditorGUILayout.BeginScrollView(scrollModified, GUILayout.Height(300));
                    analyzedCode = EditorGUILayout.TextArea(analyzedCode, GUILayout.ExpandHeight(true));
                    EditorGUILayout.EndScrollView();
                    GUILayout.EndVertical();

                    GUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.Label("Suggested Fixes (Editable)", EditorStyles.boldLabel);
                    analyzedCode = EditorGUILayout.TextArea(analyzedCode, GUILayout.Height(300));
                }

                filename = EditorGUILayout.TextField("Filename", filename);

                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Save Changes"))
                {
                    if (selectedScript != null)
                    {
                        string path = AssetDatabase.GetAssetPath(selectedScript);
                        File.WriteAllText(path, analyzedCode);
                        AssetDatabase.Refresh();
                        statusMessage = "Original file overwritten.";
                    }
                }

                if (GUILayout.Button("Save as Version"))
                {
                    string folderPath = folder != null ? AssetDatabase.GetAssetPath(folder) : "Assets";
                    string savePath = Path.Combine(folderPath, filename);
                    File.WriteAllText(savePath, analyzedCode);
                    AssetDatabase.Refresh();
                    statusMessage = "File saved as a new version.";
                }

                GUILayout.EndHorizontal();
            }
        }

        private void OnComplete(bool success, string message)
        {
            if (success)
            {
                analyzedCode = message;
                statusMessage = "Analysis complete. You can now review and save.";
                Debug.Log("Analysis Result:\n" + message);
            }
            else
            {
                Debug.LogError(message);
                statusMessage = message;
            }
        }
    }
}
