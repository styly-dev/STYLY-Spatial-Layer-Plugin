using System.Linq;
using UnityEditor;

/// <summary>
/// Scripting Define Symbols will be added to the Player Settings on load of Unity Editor, Unity package installation, switch platform or project build.
/// </summary>
[InitializeOnLoad]
class AddDefineSymbols
{
    // Scripting Define Symbol list to add
    static readonly string[] symbolsToAdd = {
            "UNITYGLTF_FORCE_DEFAULT_IMPORTER_ON",         // Enable UnityGLTF import
            "GLTFAST_FORCE_DEFAULT_IMPORTER_OFF",          // Disable GLTFast import
            "UNIGLTF_DISABLE_DEFAULT_GLTF_IMPORTER",    // Disable importer in UniVRM
            "UNIGLTF_DISABLE_DEFAULT_GLB_IMPORTER",     // Disable importer in UniVRM
        };

    // This method will be called on load of Unity Editor
    static AddDefineSymbols()
    {
        AddDefineSymbolsToPlayerSettings(symbolsToAdd);
    }

    private static void AddDefineSymbolsToPlayerSettings(string[] symbols)
    {
        // Get the current list of defined symbols for the active build target
        string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        var allDefines = definesString.Split(';').ToList();
        bool symbolsAdded = false;

        foreach (var symbol in symbols)
        {
            if (!allDefines.Contains(symbol))
            {
                allDefines.Add(symbol);
                symbolsAdded = true;
            }
        }

        // Only update the define symbols if new symbols were actually added
        if (symbolsAdded)
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", allDefines.ToArray()));
        }
    }
}