using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using UnityEngine;

[InitializeOnLoad]
public class VATSPSPatcher
{
    private static string target_script_path = "Packages/com.vrcfury.vrcfury/SPS/sps_main.cginc"; 
    private static string expected_sps_path = "Packages/com.zaytha.sps-tweaks/Editor/expected_sps_main.cginc";
    private static string patched_sps_path = "Packages/com.zaytha.sps-tweaks/Editor/patched_sps_main.cginc"; 

    [MenuItem("Tools/VAT SPS Patch")]
    public static void RunPatcher()
    {
        PatchSPS();
    }

    static VATSPSPatcher()
    {
        // Gets called when editor goes into playmode
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            PatchSPS();
        }
    }

    private static void PatchSPS()
    {
        // Ensure all necessary files exist, show popups if missing
        if (!File.Exists(expected_sps_path))
        {
            EditorUtility.DisplayDialog("Patch Error", "Expected SPS script not found in:\n" + expected_sps_path, "OK");
            Debug.LogError("Expected SPS script not found in: " + expected_sps_path);
            return;
        }

        if (!File.Exists(patched_sps_path))
        {
            EditorUtility.DisplayDialog("Patch Error", "Patched SPS script not found in:\n" + patched_sps_path, "OK");
            Debug.LogError("Patched SPS script not found in: " + patched_sps_path);
            return;
        }
        
        if (!File.Exists(target_script_path))
        {
            EditorUtility.DisplayDialog("Patch Error", "Target SPS script not found in:\n" + target_script_path, "OK");
            Debug.LogError("Target SPS script not found in: " + target_script_path);
            return;
        }

        string expected_content = File.ReadAllText(expected_sps_path);
        string patch_content = File.ReadAllText(patched_sps_path);
        string target_content = File.ReadAllText(target_script_path);

        // Already patched, exit
        if (target_content == patch_content)
        {
            Debug.Log("sps_main.cginc is already patched.");
            return;
        }

        // Ready to patch
        // ah
        else if (target_content == expected_content)
        {
            Debug.Log("Patching sps_main.cginc...");
            File.WriteAllText(target_script_path, patch_content);
            AssetDatabase.Refresh(); // Refresh Unity to apply changes
            Debug.Log("Shader patch applied successfully!");
            return;
        }

        // Unexpected discrepancy
        else if (target_content != expected_content && target_content != patch_content)
        {
            EditorUtility.DisplayDialog(
                "Patch Error", 
                "sps_main.cginc is not patched and not in the expected form.\nZaytha SPS Tweaks needs to be updated.", 
                "OK"
            );
            Debug.LogError("sps_main.cginc is not patched, and not in the expected form. Zaytha SPS Tweaks needs to be updated. \n \nIf it's up to date, the patcher needs to be fixed and will be updated soon.");
            return;
        }

        else 
        {
            EditorUtility.DisplayDialog("Patch Error", "Something has gone terribly wrong.", "OK");
            Debug.LogError("Something has gone terribly wrong.");
            return;
        }
    }

}
