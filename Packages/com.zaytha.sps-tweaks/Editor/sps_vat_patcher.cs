#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using UnityEngine;
using System.Linq;

[InitializeOnLoad]
public class VATSPSPatcher
{
    // private static string target_script_path = "Packages/com.vrcfury.vrcfury/SPS/sps_main.cginc"; 
    // private static string expected_sps_path = "Packages/com.zaytha.sps-tweaks/Editor/expected_sps_main.cginc";
    

    private static string edited_sps_main_file_path = "Packages/com.zaytha.sps-tweaks/PatcherContent/sps_main_vat_patch.cginc"; // copy this file
    private static string patched_sps_main_file_path = "Packages/com.vrcfury.vrcfury/SPS/sps_main_vat_patch.cginc"; // to this location
    private static string sps_main_folder = "Packages/com.vrcfury.vrcfury/SPS"; // sanity check the folder exists

    
    private static string expected_patcher_file_path = "Packages/com.zaytha.sps-tweaks/PatcherContent/expected_sps_patcher.cs"; // if this file matches the content in target_pathcer_file_path
    private static string edited_patcher_file_path = "Packages/com.zaytha.sps-tweaks/PatcherContent/edited_sps_patcher.cs"; // copy the contents of this file
    private static string target_pathcer_file_path = "Packages/com.vrcfury.vrcfury/Editor/VF/Builder/Haptics/SpsPatcher.cs"; // to this existing file

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
        // check all needed files exist, show popups if missing
        if (!File.Exists(edited_sps_main_file_path))
        {
            EditorUtility.DisplayDialog("Patch Error", "Expected patched sps_main script not found in:\n" + edited_sps_main_file_path + "\n\nPlease re-install Zaytha's SPS-Tweaks", "OK");
            Debug.LogError("Expected patched sps_main script not found in: " + edited_sps_main_file_path);
            return;
        }

        if (!File.Exists(expected_patcher_file_path))
        {
            EditorUtility.DisplayDialog("Patch Error", "Expected SPS Patcher Reference script not found in:\n" + expected_patcher_file_path + "\n\nPlease re-install Zaytha's SPS-Tweaks", "OK");
            Debug.LogError("Expected SPS Patcher Reference script not found in: " + expected_patcher_file_path);
            return;
        }

        if (!File.Exists(edited_patcher_file_path))
        {
            EditorUtility.DisplayDialog("Patch Error", "Expected SPS Patcher Patch script not found in:\n" + edited_patcher_file_path + "\n\nPlease re-install Zaytha's SPS-Tweaks", "OK");
            Debug.LogError("Expected SPS Patcher Patch script not found in: " + edited_patcher_file_path);
            return;
        }
        

        if (!Directory.Exists(sps_main_folder))
        {
            EditorUtility.DisplayDialog("Patch Error", "Sps folder path not found in:\n" + sps_main_folder + "\n\nMake sure VRCFury is installed", "OK");
            Debug.LogError("Sps folder path not found in: " + sps_main_folder);
            return;
        }

        if (!File.Exists(target_pathcer_file_path))
        {
            EditorUtility.DisplayDialog("Patch Error", "Target SpsPatcher.cs script not found in:\n" + target_pathcer_file_path + "\n\nMake sure VRCFury is installed", "OK");
            Debug.LogError("Target SpsPatcher.cs script not found in: " + target_pathcer_file_path);
            return;
        }

        
        // Overrite the sps_main_vat_patch.cginc file, it may be old if I need to update the script in the future, so take no chances and always update to it
        FileInfo edited_file = new FileInfo(edited_sps_main_file_path);
        edited_file.CopyTo(patched_sps_main_file_path, true);
        Debug.Log("'sps_main_vat_patch.cginc' ready in location");



        // Patch Patcher (cursed as hell)

        // Get refrences
        string expected_patcher_content = File.ReadAllText(expected_patcher_file_path);
        string edited_patcher_content = File.ReadAllText(edited_patcher_file_path);
        string target_patcher_content = File.ReadAllText(target_pathcer_file_path);

        var target_lines = File.ReadAllLines(target_pathcer_file_path).ToList();
        var expected_lines = File.ReadAllLines(expected_patcher_file_path).ToList();




        // Check if Patcher is already patched and return response
        if (target_patcher_content == edited_patcher_content)
        {
            EditorUtility.DisplayDialog(
                "Zaytha SPS Tweaks", 
                "SPS is already patched.", 
                "Got it."
            );
            Debug.Log("SpsPatcher.cs is already patched.");
            return;
        }
        // Check if patcher file matches what we expect from VRCFury
        else if (target_patcher_content == expected_patcher_content)
        {
            Debug.Log("Automatic file check sucess");
            confirm_patch(edited_patcher_content);
            return;
        }
        
        // I don't know why but sometimes the check doesn't work, even if the files are identical
        // Assuming its something to do with the real file being in a Runtime folder?
        // Manually stepping through each line and comparing the file seems to work
        else if (target_patcher_content != expected_patcher_content && target_lines.Count == expected_lines.Count)
        {
            bool do_files_match = true;

            Debug.Log("Manual Check");
            for (int i = 0; i < target_lines.Count; i++)
            {
                string target_line = target_lines[i];
                string expected_line = expected_lines[i];

                if (target_line != expected_line){
                    Debug.Log("Print this if there's an error!");
                    do_files_match = false;
                }
            }

            if (do_files_match)
            {
                Debug.Log("Files are idenentical after manual check, beginning patch...");
                confirm_patch(edited_patcher_content);
                return;
            }
        }
        // SpsPatcher.cs is different from what we expected, this mostly likely means SPS has been updated.
        else if (target_patcher_content != expected_patcher_content && target_patcher_content != edited_patcher_content)
        {
            EditorUtility.DisplayDialog(
                "Patch Error", 
                "DO NOT REPORT THIS ERROR TO VRCFURY!!!\n\nSpsPatcher.cs is not in the expected form and has not been patched.\n-----------------------------------------------\n\nZaytha's SPS Tweaks needs to be updated to the latest version.\nIf it is on the latest version, sps has been updated and the patcher needs to be fixed to match. If I'm still alive, I'm working on doing this, so check for updates.\n\nUntil the update is out, you can uninstall Zaytha's SPS Tweaks and the model will still work w/o the added features.", 
                "OK"
            );
            Debug.LogError("SpsPatcher.cs not in the expected form and has not been patched. Zaytha SPS Tweaks needs to be updated. If it's up to date, the patcher needs to be fixed and will be updated soon.");
            return;
        }
        else // I don't know how this error would come to be
        {
            EditorUtility.DisplayDialog("Patch Error", "DO NOT REPORT THIS ERROR TO VRCFURY!!!\nUnexpected error with Zaytha's SPS Tweaks.", "OK");
            Debug.LogError("Unexpected error with Zaytha's SPS Tweaks.");
            return;
        }
    }



    private static void confirm_patch(string patch_content)
    {
        bool patch_confirm_choice = EditorUtility.DisplayDialog(
            "Zaytha SPS Tweaks", 
            "SPS is ready to be patched. Are you sure you'd like to patch?",
            "Yes, begin patching.", 
            "No, cancel patch."
        );


        if (patch_confirm_choice)
        {
            File.WriteAllText(target_pathcer_file_path, patch_content);
            AssetDatabase.Refresh(); // Refresh Unity to apply changes
            Debug.Log("SpsPatcher.cs patched.");
        }
        else
        {
            Debug.Log("User cancled sps vat patch");
        }
    }
}
#endif