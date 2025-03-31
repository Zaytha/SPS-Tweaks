#if UNITY_EDITOR
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
        // called when editor goes into playmode
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
        // check all needed files exist, show popups if missing
        if (!File.Exists(expected_sps_path))
        {
            EditorUtility.DisplayDialog("Patch Error", "Expected SPS script not found in:\n" + expected_sps_path + "\n\nPlease re-install Zaytha's SPS-Tweaks", "OK");
            Debug.LogError("Expected SPS script not found in: " + expected_sps_path);
            return;
        }

        if (!File.Exists(patched_sps_path))
        {
            EditorUtility.DisplayDialog("Patch Error", "Patched SPS script not found in:\n" + patched_sps_path + "\n\nPlease re-install Zaytha's SPS-Tweaks", "OK");
            Debug.LogError("Patched SPS script not found in: " + patched_sps_path);
            return;
        }
        
        if (!File.Exists(target_script_path))
        {
            EditorUtility.DisplayDialog("Patch Error", "Target SPS script not found in:\n" + target_script_path + "\n\nMake sure VRCFury is installed", "OK");
            Debug.LogError("Target SPS script not found in: " + target_script_path);
            return;
        }

        string expected_content = File.ReadAllText(expected_sps_path);
        string patch_content = File.ReadAllText(patched_sps_path);
        string target_content = File.ReadAllText(target_script_path);

        // already patched, do nothing
        if (target_content == patch_content)
        {
            Debug.Log("sps_main.cginc is already patched.");
            return;
        }

        // All expetec content matches, do patching process
        else if (target_content == expected_content)
        {
            Debug.Log("Patching sps_main.cginc...");
            File.WriteAllText(target_script_path, patch_content);
            AssetDatabase.Refresh(); // Refresh Unity to apply changes
            Debug.Log("Shader patch applied successfully!");
            return;
        }

        // sps_main is different from what we expected, this mostly likely means SPS has been updated.
        else if (target_content != expected_content && target_content != patch_content)
        {
            EditorUtility.DisplayDialog(
                "Patch Error", 
                "DO NOT REPORT THIS ERROR TO VRCFURY!!!\nsps_main.cginc not in the expected form and has not been patched.\n-----------------------------------------------\n\nZaytha's SPS Tweaks needs to be updated to the latest version.\nIf it is on the latest version, sps has been updated and the patcher needs to be fixed to match that and will be updated soon.\n\nUntil the update is out, you can uninstall Zaytha's SPS Tweaks and the model will still work w/o the added features.", 
                "OK"
            );
            Debug.LogError("sps_main.cginc not in the expected form and has not been patched.Zaytha SPS Tweaks needs to be updated. If it's up to date, the patcher needs to be fixed and will be updated soon.");
            return;
        }

        else 
        {
            EditorUtility.DisplayDialog("Patch Error", "DO NOT REPORT THIS ERROR TO VRCFURY!!!\nUnexpected error with Zaytha's SPS Tweaks.", "OK");
            Debug.LogError("Unexpected error with Zaytha's SPS Tweaks.");
            return;
        }
    }

}
#endif