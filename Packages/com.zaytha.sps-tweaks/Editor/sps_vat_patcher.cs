#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using UnityEngine;
using System.Linq;

[InitializeOnLoad]
public class VATSPSPatcher
{
    // Target Content
    private static string target_sps_globals = "Packages/com.vrcfury.vrcfury/SPS/sps_globals.cginc";
    private static string target_sps_props = "Packages/com.vrcfury.vrcfury/SPS/sps_props.cginc";
    private static string target_SPSConfigurer = "Packages/com.vrcfury.vrcfury/Editor/VF/Builder/Haptics/SpsConfigurer.cs";
    private static string target_SPSPatcher = "Packages/com.vrcfury.vrcfury/Editor/VF/Builder/Haptics/SpsPatcher.cs";
    private static string target_VRCFuryHapticPlug = "Packages/com.vrcfury.vrcfury/Runtime/VF/Component/VRCFuryHapticPlug.cs";
    private static string target_VRCFuryHapticPlugEditor = "Packages/com.vrcfury.vrcfury/Editor/VF/Inspector/VRCFuryHapticPlugEditor.cs";
    private static string target_sps_main = "Packages/com.vrcfury.vrcfury/SPS/sps_main.cginc";

    // Expected Content
    private static string expected_sps_globals = "Packages/com.zaytha.sps-tweaks/PatcherContent/expected_content/sps_globals.cginc";
    private static string expected_sps_props = "Packages/com.zaytha.sps-tweaks/PatcherContent/expected_content/sps_props.cginc";
    private static string expected_SPSConfigurer = "Packages/com.zaytha.sps-tweaks/PatcherContent/expected_content/SpsConfigurer.cs";
    private static string expected_SPSPatcher = "Packages/com.zaytha.sps-tweaks/PatcherContent/expected_content/SpsPatcher.cs";
    private static string expected_VRCFuryHapticPlug = "Packages/com.vrcfury.vrcfury/Runtime/VF/Component/VRCFuryHapticPlug.cs";
    private static string expected_VRCFuryHapticPlugEditor = "Packages/com.zaytha.sps-tweaks/PatcherContent/expected_content/VRCFuryHapticPlugEditor.cs";
    private static string expected_sps_main = "Packages/com.zaytha.sps-tweaks/PatcherContent/expected_content/sps_main.cginc";

    // Patched Content
    private static string patched_sps_globals = "Packages/com.zaytha.sps-tweaks/PatcherContent/patched_content/sps_globals.cginc";
    private static string patched_sps_props = "Packages/com.zaytha.sps-tweaks/PatcherContent/patched_content/sps_props.cginc";
    private static string patched_SPSConfigurer = "Packages/com.zaytha.sps-tweaks/PatcherContent/patched_content/SpsConfigurer.cs";
    private static string patched_SPSPatcher = "Packages/com.zaytha.sps-tweaks/PatcherContent/patched_content/SpsPatcher.cs";
    private static string patched_VRCFuryHapticPlug = "Packages/com.zaytha.sps-tweaks/PatcherContent/patched_content/VRCFuryHapticPlug.cs";
    private static string patched_VRCFuryHapticPlugEditor = "Packages/com.zaytha.sps-tweaks/PatcherContent/patched_content/VRCFuryHapticPlugEditor.cs";
    // no patched_sps_main as we're passing in the vat_patch as an option
    
    // New SPS Vat
    private static string sps_main_with_vat_path = "Packages/com.zaytha.sps-tweaks/PatcherContent/patched_content/sps_main_with_vats.cginc";
    private static string target_sps_main_with_vat_path = "Packages/com.vrcfury.vrcfury/SPS/sps_main_with_vats.cginc";
    private static string sps_vat_path = "Packages/com.zaytha.sps-tweaks/PatcherContent/patched_content/sps_vat.cginc";
    private static string target_sps_vat_path = "Packages/com.vrcfury.vrcfury/SPS/sps_vat.cginc";

    [MenuItem("Tools/Zaytha's SPS Tweaks/Patch")]
    public static void RunPatcher()
    {
        PatchSPS();
    }

    [MenuItem("Tools/Zaytha's SPS Tweaks/Remove Patch")]
    public static void RemovePatcher()
    {
        EditorUtility.DisplayDialog(
            "Zaytha SPS Tweaks", 
            "!!THE PATCH HAS NOT BEEN REMOVED, YOU MUST DO IT MANUALLY!!\n\nTo remove this patch:\n-Uninstall Zaytha's SPS Tweaks and VRCFury through VCC\n-Reinstall VRCFury through VCC", 
            "Got it."
        );
    }


    private static void PatchSPS()
    {

        // Build array of all files to make sure everything exists as it should before patching
        string[] target_content_path = {
            target_sps_globals,
            target_sps_props,
            target_SPSConfigurer,
            target_SPSPatcher,
            target_VRCFuryHapticPlug,
            target_VRCFuryHapticPlugEditor,
            target_sps_main
        };
        string[] expected_content_path = {
            expected_sps_globals,
            expected_sps_props,
            expected_SPSConfigurer,
            expected_SPSPatcher,
            expected_VRCFuryHapticPlug,
            expected_VRCFuryHapticPlugEditor,
            expected_sps_main
        };
        string[] patched_content_path = {
            patched_sps_globals,
            patched_sps_props,
            patched_SPSConfigurer,
            patched_SPSPatcher,
            patched_VRCFuryHapticPlug,
            patched_VRCFuryHapticPlugEditor,
            sps_main_with_vat_path,
            sps_vat_path
        };

        // Error check each array to make sure the files exist
        foreach (string path in target_content_path)
        {
            if (!File.Exists(path))
            {
                EditorUtility.DisplayDialog("Patch Error", "Target content file not found in:\n" + path + "\n\nMake sure VRCFury is installed", "OK");
                Debug.LogError("Target content file not found in: " + path);
                return;
            }
        }
        foreach (string path in expected_content_path)
        {
            if (!File.Exists(path))
            {
                EditorUtility.DisplayDialog("Patch Error", "Expected content file not found in:\n" + path + "\n\nPlease re-install Zaytha's SPS-Tweaks", "OK");
                Debug.LogError("Expected content file not found in: " + path);
                return;
            }
        }
        foreach (string path in patched_content_path)
        {
            if (!File.Exists(path))
            {
                EditorUtility.DisplayDialog("Patch Error", "Patched content file not found in:\n" + path + "\n\nPlease re-install Zaytha's SPS-Tweaks", "OK");
                Debug.LogError("Patched content file not found in: " + path);
                return;
            }
        }

        // Check to see if the target content is already patched
        // Assume its already pathces, and check if its not
        bool is_already_patched = true;
        for (int i = 0; i < target_content_path.Length - 1; i++) // subtract -1 so we don't check target_sps_main
        {
            string target_content = File.ReadAllText(target_content_path[i]);
            string patched_content = File.ReadAllText(patched_content_path[i]);

            if (NormalizeContent(target_content) != NormalizeContent(patched_content))
            {
                is_already_patched = false;
                break;
            }
        }

        // check if sps vat and sps main with vat exist in the target location
        if (!File.Exists(target_sps_vat_path) || !File.Exists(target_sps_main_with_vat_path))
        {
            is_already_patched = false;
        }

        // Skip patching if its already patched
        if (is_already_patched)
        {
            EditorUtility.DisplayDialog(
                "Zaytha SPS Tweaks", 
                "SPS is already patched.", 
                "Got it."
            );
            Debug.Log("SPS is already patched.");
            return;
        }
        
        // check target content versus expected content and throw error if theres a mismatch
        for (int i = 0; i < target_content_path.Length; i++)
        {
            string target_content = File.ReadAllText(target_content_path[i]);
            string expected_content = File.ReadAllText(expected_content_path[i]);

            if (NormalizeContent(target_content) != NormalizeContent(expected_content))
            {
                EditorUtility.DisplayDialog(
                "Patch Error", 
                "DO NOT REPORT THIS ERROR TO VRCFURY!!!\n\n" + target_content_path[i] + " is not in the expected form and has not been patched.\n-----------------------------------------------\n\n" + 
                "Zaytha's SPS Tweaks needs to be updated to the latest version.\n" + 
                "If it is on the latest version, sps has been updated and the patcher needs to be fixed to match.\n " + 
                "If I'm still alive, I'm working on doing this, so check for updates.\n\nUntil the update is out, you can uninstall Zaytha's SPS Tweaks and the model will still work w/o the added features.",
                "OK"
                );
                Debug.LogError("SpsPatcher.cs not in the expected form and has not been patched. Zaytha SPS Tweaks needs to be updated. If it's up to date, the patcher needs to be fixed and will be updated soon.");
                return;
            }
        }
        

        // If everythings fine, patch content
        bool patch_confirm_choice = EditorUtility.DisplayDialog(
            "Zaytha SPS Tweaks", 
            "SPS is ready to be patched. Are you sure you'd like to patch?",
            "Yes, begin patching.", 
            "No, cancel patch."
        );


        if (patch_confirm_choice)
        {
            // pathc all items in the array
            for (int i = 0; i < target_content_path.Length; i++)
            {
                string patched_content = File.ReadAllText(patched_content_path[i]);
                File.WriteAllText(target_content_path[i], patched_content);
                Debug.Log("Patched: " + target_content_path[i]);
            } 

            // Patch in sps vat
            string sps_vat_content = File.ReadAllText(sps_vat_path);
            File.WriteAllText(target_sps_vat_path, sps_vat_content);
            Debug.Log("Patched: " + target_sps_vat_path);

            // Patch in sps main with vats
            string sps_main_with_vat_content = File.ReadAllText(sps_main_with_vat_path);
            File.WriteAllText(target_sps_main_with_vat_path, sps_main_with_vat_content);
            Debug.Log("Patched: " + target_sps_main_with_vat_path);

            // Refresh Unity to apply changes
            AssetDatabase.Refresh(); 
            Debug.Log("SPS patched.");
        }
        else
        {
            Debug.Log("User cancled sps vat patch");
        }
    }

    // Was having issues where content was matching even though when checked by hand it was fine
    // So sanity check make this nice for computers to read :)
    private static string NormalizeContent(string content)
    {
        // Normalize line endings to unix
        content = content.Replace("\r\n", "\n").Replace("\r", "\n");
        // Trim whitespace from each line
        content = string.Join("\n", content.Split('\n').Select(line => line.Trim()));
        // Remove BOM if present
        return content.TrimStart('\uFEFF');
    }
}
#endif