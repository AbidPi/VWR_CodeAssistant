using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace UnityCodeAssistant
{
    public class UnityCodeAssistantWindow : EditorWindow
    {
        private string prompt = "";
        private string statusMessage = "";
        private MonoScript selectedScript;

        private string originalScriptCode = "";
        private string analyzedCode = "";
        private string filename = "NewScript.cs";
        private DefaultAsset folder;

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

            filename = EditorGUILayout.TextField("Filename (e.g. PlayerController.cs):", filename);
            folder = (DefaultAsset)EditorGUILayout.ObjectField("Save Folder", folder, typeof(DefaultAsset), false);

            if (GUILayout.Button("Generate Script"))
            {
                if (string.IsNullOrEmpty(prompt))
                {
                    statusMessage = "Please enter a prompt.";
                }
                else if (folder == null)
                {
                    statusMessage = "Please choose a folder to save the script.";
                }
                else
                {
                    string folderPath = AssetDatabase.GetAssetPath(folder);
                    PromptHandler.GenerateScript(prompt, folderPath, filename, OnComplete);
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

            if (selectedScript != null && GUILayout.Button("Analyze Script"))
            {
                string path = AssetDatabase.GetAssetPath(selectedScript);

                if (File.Exists(path))
                {
                    originalScriptCode = File.ReadAllText(path);
                    statusMessage = "Analyzing script...";
                    PromptHandler.AnalyzeScript(originalScriptCode, OnAnalysisComplete);
                    filename = Path.GetFileName(path);
                }
                else
                {
                    Debug.LogError("Could not find the selected file.");
                    statusMessage = "Failed to load the script.";
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

                    var originalLines = originalScriptCode.Split('\n');
                    var suggestedLines = analyzedCode.Split('\n');
                    var diffs = GetLineDiff(originalLines, suggestedLines);

                    // Left: Original (readonly)
                    GUILayout.BeginVertical(GUILayout.Width(position.width / 2 - 10));
                    GUILayout.Label("Original", EditorStyles.miniBoldLabel);
                    scrollOriginal = EditorGUILayout.BeginScrollView(scrollOriginal, GUILayout.Height(300));
                    foreach (var diff in diffs)
                    {
                        if (diff.type == " " || diff.type == "-")
                        {
                            if (diff.type == "-")
                                GUI.backgroundColor = new Color(1f, 0.85f, 0.85f);
                            EditorGUILayout.SelectableLabel(diff.original, EditorStyles.textField, GUILayout.Height(16));
                            GUI.backgroundColor = Color.white;
                        }
                    }
                    EditorGUILayout.EndScrollView();
                    GUILayout.EndVertical();

                    // Right: Suggested (editable)
                    GUILayout.BeginVertical(GUILayout.Width(position.width / 2 - 10));
                    GUILayout.Label("Suggested", EditorStyles.miniBoldLabel);
                    scrollModified = EditorGUILayout.BeginScrollView(scrollModified, GUILayout.Height(300));

                    GUIStyle greenStyle = new GUIStyle(EditorStyles.textField);
                    greenStyle.normal.background = Texture2D.whiteTexture;
                    greenStyle.normal.textColor = Color.black;

                    List<string> editedLines = new List<string>();
                    foreach (var diff in diffs)
                    {
                        if (diff.type == " " || diff.type == "+")
                        {
                            if (diff.type == "+")
                                GUI.backgroundColor = new Color(0.85f, 1f, 0.85f);

                            string edited = EditorGUILayout.TextField(diff.suggested, GUILayout.Height(16));
                            editedLines.Add(edited);
                            GUI.backgroundColor = Color.white;
                        }
                    }

                    analyzedCode = string.Join("\n", editedLines);
                    EditorGUILayout.EndScrollView();
                    GUILayout.EndVertical();

                    GUILayout.EndHorizontal();

                    if (GUILayout.Button("Apply Fixes"))
                    {
                        if (selectedScript != null)
                        {
                            string path = AssetDatabase.GetAssetPath(selectedScript);
                            File.WriteAllText(path, analyzedCode);
                            AssetDatabase.Refresh();
                            statusMessage = "Fixes applied successfully.";
                        }
                    }
                }
                else
                {
                    GUILayout.Label("Suggested Fixes (Editable)", EditorStyles.boldLabel);
                    analyzedCode = EditorGUILayout.TextArea(analyzedCode, GUILayout.Height(300));

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
                }
            }
        }

        private void OnComplete(bool success, string message)
        {
            statusMessage = success ? message : $"Error: {message}";
            Debug.Log(statusMessage);
        }

        private void OnAnalysisComplete(bool success, string message)
        {
            if (success)
            {
                analyzedCode = message;
                statusMessage = "Analysis complete. Review and apply changes.";
            }
            else
            {
                analyzedCode = "";
                statusMessage = message;
            }
        }

        private struct DiffLine
        {
            public string type;
            public string original;
            public string suggested;
        }

        private List<DiffLine> GetLineDiff(string[] original, string[] modified)
        {
            List<DiffLine> result = new List<DiffLine>();
            int i = 0, j = 0;
            while (i < original.Length && j < modified.Length)
            {
                if (original[i] == modified[j])
                {
                    result.Add(new DiffLine { type = " ", original = original[i], suggested = modified[j] });
                    i++; j++;
                }
                else
                {
                    result.Add(new DiffLine { type = "-", original = original[i], suggested = "" });
                    result.Add(new DiffLine { type = "+", original = "", suggested = modified[j] });
                    i++; j++;
                }
            }
            while (i < original.Length)
                result.Add(new DiffLine { type = "-", original = original[i++], suggested = "" });
            while (j < modified.Length)
                result.Add(new DiffLine { type = "+", original = "", suggested = modified[j++] });

            return result;
        }
    }
}
