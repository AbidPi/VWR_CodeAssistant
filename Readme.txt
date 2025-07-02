# Unity Code Assistant 

Generate Unity C# scripts from plain English using OpenAI — directly inside the Unity Editor.

## Features

-  Turn natural language prompts into Unity-ready MonoBehaviour scripts.
-  Built as a Unity Editor plugin (Editor-only, safe for builds).
-  Save generated scripts directly into your selected folder.
- Secure API key setup using Unity's EditorPrefs.

---

##  Installation (Using `.tgz` File)

1. Download the file "unity-code-assistant-V-1.0.tgz"
2. Copy the .tgz` file to your local system (e.g., `C:/UnityPackages/` or `/Users/you/Downloads/`).
3. Open your Unity project folder.
4. Edit Packages/manifest.json`.
5. Inside the "dependencies"` block, add:

```json
"com.vwr.unitycodeassistant": "file:/absolute/path/to/unity-code-assistant-V-1.0.tgz"


Our API Key :: 

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
