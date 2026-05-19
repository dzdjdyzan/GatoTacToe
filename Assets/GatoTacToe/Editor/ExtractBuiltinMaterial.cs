using UnityEngine;
using UnityEditor;

public class ExtractBuiltinMaterial : MonoBehaviour
{
    [MenuItem("Tools/Extract Default-ParticleSystem Material")]
    static void ExtractMaterial()
    {
        // 1. Load the built-in material using its path and name
        string materialPath = "Resources/unity_builtin_extra";
        string materialName = "Default-ParticleSystem";
        Material builtinMaterial = AssetDatabase.LoadAssetAtPath<Material>(materialPath);

        // If not found by direct path, search through all assets in the built-in resource file
        if (builtinMaterial == null)
        {
            UnityEngine.Object[] allBuiltinAssets = AssetDatabase.LoadAllAssetsAtPath(materialPath);
            foreach (var asset in allBuiltinAssets)
            {
                if (asset is Material && asset.name == materialName)
                {
                    builtinMaterial = asset as Material;
                    break;
                }
            }
        }

        if (builtinMaterial == null)
        {
            Debug.LogError($"Could not find material '{materialName}' in '{materialPath}'");
            return;
        }

        // 2. Create a new instance (a copy) of the material
        Material newMaterial = new Material(builtinMaterial);

        // 3. Save the copy as an asset in your project
        string savePath = "Assets/GlowLine_Material.mat";
        AssetDatabase.CreateAsset(newMaterial, savePath);
        AssetDatabase.SaveAssets();

        // 4. Select the new material in the Project window
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newMaterial;

        Debug.Log($"Successfully created a copy at: {savePath}");
    }
}