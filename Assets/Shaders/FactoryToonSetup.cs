using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

// Explicit menu action only; importing this file does not alter project settings.
public static class FactoryToonSetup
{
    const string Root = "Assets/Factory_URP";

    [MenuItem("Tools/Factory/Create URP Toon Prefab")]
    public static void Create()
    {
        var pipeline = GraphicsSettings.currentRenderPipeline;
        if (pipeline == null || !pipeline.GetType().Name.Contains("UniversalRenderPipelineAsset"))
            throw new InvalidOperationException("Select a URP pipeline asset in Project Settings before running this command.");
        var shader = Shader.Find("Toon/Toon");
        if (shader == null)
            throw new InvalidOperationException("Install a Unity Toon Shader (com.unity.toonshader) version compatible with your Unity/URP version first.");
        var model = AssetDatabase.LoadAssetAtPath<GameObject>(Root + "/Models/Factory.fbx");
        if (model == null) throw new FileNotFoundException("Keep the supplied Assets/Factory_URP folder structure.");

        var baseMap = LoadTexture("ToonBase", true);
        var first = LoadTexture("Shade1", true);
        var second = LoadTexture("Shade2", true);
        var outline = LoadTexture("OutlineWidth", false);
        var material = new Material(shader) { name = "Factory_URP_Toon" };
        string[] required = { "_MainTex", "_1st_ShadeMap", "_2nd_ShadeMap", "_Outline_Sampler" };
        foreach (string key in required)
            if (!material.HasProperty(key))
            {
                UnityEngine.Object.DestroyImmediate(material);
                throw new InvalidOperationException("Installed Toon/Toon shader does not support " + key);
            }
        Texture(material, "_MainTex", baseMap);
        Texture(material, "_BaseMap", baseMap);
        Texture(material, "_1st_ShadeMap", first);
        Texture(material, "_2nd_ShadeMap", second);
        Texture(material, "_Outline_Sampler", outline);
        ColorValue(material, "_BaseColor", Color.white);
        ColorValue(material, "_Color", Color.white);
        ColorValue(material, "_1st_ShadeColor", Color.white);
        ColorValue(material, "_2nd_ShadeColor", Color.white);
        ColorValue(material, "_Outline_Color", new Color(.12f, .08f, .075f, 1));
        ColorValue(material, "_HighColor", Color.black);
        ColorValue(material, "_Emissive_Color", Color.black);
        Float(material, "_Use_BaseAs1st", 0);
        Float(material, "_Use_1stAs2nd", 0);
        Float(material, "_utsTechnique", 0);
        Float(material, "_BaseColor_Step", .55f);
        Float(material, "_ShadeColor_Step", .25f);
        Float(material, "_BaseShade_Feather", .035f);
        Float(material, "_1st2nd_Shades_Feather", .04f);
        Float(material, "_TransparentEnabled", 0);
        Float(material, "_ClippingMode", 0);
        Float(material, "_CullMode", 2);
        Float(material, "_ZWriteMode", 1);
        Float(material, "_Set_SystemShadowsToBase", 1);
        Float(material, "_Is_NormalMapToBase", 0);
        Float(material, "_RimLight", 0);
        Float(material, "_MatCap", 0);
        Float(material, "_AngelRing", 0);
        Float(material, "_Outline_Width", .25f);
        Float(material, "_OUTLINE", 0);
        Float(material, "_Is_BlendBaseColor", 0);
        Float(material, "_GI_Intensity", .1f);
        Float(material, "_Unlit_Intensity", 1);
        material.DisableKeyword("_SHADINGGRADEMAP");
        material.DisableKeyword("_IS_CLIPPING_MODE");
        material.DisableKeyword("_IS_CLIPPING_TRANSMODE");
        material.DisableKeyword("_OUTLINE_POS");
        material.EnableKeyword("_IS_CLIPPING_OFF");
        material.EnableKeyword("_IS_OUTLINE_CLIPPING_NO");
        material.EnableKeyword("_OUTLINE_NML");
        material.EnableKeyword("_EMISSIVE_SIMPLE");
        material.renderQueue = 2000;
        material.SetOverrideTag("RenderType", "Opaque");

        if (!AssetDatabase.IsValidFolder(Root + "/Generated"))
            AssetDatabase.CreateFolder(Root, "Generated");
        var materialPath = AssetDatabase.GenerateUniqueAssetPath(Root + "/Generated/Factory_URP_Toon.mat");
        AssetDatabase.CreateAsset(material, materialPath);
        GameObject instance = null;
        try
        {
            instance = (GameObject)PrefabUtility.InstantiatePrefab(model);
            instance.name = "Factory_URP_Toon";
            foreach (var renderer in instance.GetComponentsInChildren<Renderer>(true))
            {
                var slots = renderer.sharedMaterials;
                for (int i = 0; i < slots.Length; i++) slots[i] = material;
                renderer.sharedMaterials = slots;
            }
            var prefabPath = AssetDatabase.GenerateUniqueAssetPath(Root + "/Generated/Factory_URP_Toon.prefab");
            var prefab = PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            AssetDatabase.SaveAssets();
            Selection.activeObject = prefab;
            EditorGUIUtility.PingObject(prefab);
            Debug.Log("Factory toon prefab created: " + prefabPath + ". Adjust outline width and shade thresholds under your scene lighting.");
        }
        finally
        {
            if (instance != null) UnityEngine.Object.DestroyImmediate(instance);
        }
    }

    static Texture2D LoadTexture(string name, bool srgb)
    {
        string path = Root + "/Textures/Factory_" + name + ".png";
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) throw new FileNotFoundException(path);
        importer.textureType = TextureImporterType.Default;
        importer.sRGBTexture = srgb;
        importer.alphaSource = TextureImporterAlphaSource.None;
        importer.mipmapEnabled = true;
        importer.maxTextureSize = 2048;
        importer.wrapMode = TextureWrapMode.Repeat;
        importer.filterMode = FilterMode.Bilinear;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }
    static void Float(Material m, string key, float value) { if (m.HasProperty(key)) m.SetFloat(key, value); }
    static void ColorValue(Material m, string key, Color value) { if (m.HasProperty(key)) m.SetColor(key, value); }
    static void Texture(Material m, string key, Texture value) { if (m.HasProperty(key)) m.SetTexture(key, value); }
}

