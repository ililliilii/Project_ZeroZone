using System.IO;
using UnityEditor;
using UnityEngine;

public static class RobotRestoredUTS2Setup
{
    [MenuItem("Tools/Robot Restored UTS2/Create Materials and Prefab")]
    public static void Build()
    {
        var shader = Shader.Find("UnityChanToonShader/Toon_DoubleShadeWithFeather");
        if (!shader) { EditorUtility.DisplayDialog("Robot Restored UTS2", "Unity-Chan Toon Shader 2.0 / Toon_DoubleShadeWithFeather is required.", "OK"); return; }
        string root = null;
        foreach (var guid in AssetDatabase.FindAssets("RobotRestoredUTS2Setup t:MonoScript"))
        {
            var p = AssetDatabase.GUIDToAssetPath(guid);
            if (p.EndsWith("/Editor/RobotRestoredUTS2Setup.cs")) { root = p.Substring(0, p.Length - "/Editor/RobotRestoredUTS2Setup.cs".Length); break; }
        }
        if (root == null) throw new System.Exception("RobotRestoredUTS2Setup.cs 위치를 찾을 수 없습니다.");
        var model = AssetDatabase.LoadAssetAtPath<GameObject>(root + "/Robot_Rigtest_Restored.fbx");
        if (!model) throw new System.Exception("Robot_Rigtest_Restored.fbx를 같은 폴더에 두세요.");
        Directory.CreateDirectory(root + "/Materials");
        AssetDatabase.Refresh();
        var top = Make(root, shader, "Robot_01_Top", "01_Top", Color.white);
        var bot = Make(root, shader, "Robot_01_Bot", "01_Bot", Color.white);
        var track = Make(root, shader, "Track", "Track", Color.white);
        var wheel = Make(root, shader, "Wheel", "Wheel", Color.white);
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(model);
        try
        {
            foreach (var r in instance.GetComponentsInChildren<Renderer>(true))
            {
                var mat = r.name.StartsWith("Track") ? track : r.name.Contains("Wheel") ? wheel : r.name == "01_Top" ? top : bot;
                var slots = r.sharedMaterials;
                if (slots.Length == 0) slots = new Material[1];
                for (int i=0;i<slots.Length;i++) slots[i]=mat;
                r.sharedMaterials=slots;
            }
            PrefabUtility.SaveAsPrefabAsset(instance, root + "/Robot_Rigtest_Restored.prefab");
        }
        finally { Object.DestroyImmediate(instance); }
        AssetDatabase.SaveAssets();
        Selection.activeObject=AssetDatabase.LoadAssetAtPath<GameObject>(root + "/Robot_Rigtest_Restored.prefab");
        EditorGUIUtility.PingObject(Selection.activeObject);
        Debug.Log("Robot Toon: 4 materials and rigged prefab created. Check lighting and outline scale in your scene.");
    }
    static void F(Material m,string p,float v) { if(m.HasProperty(p))m.SetFloat(p,v); }
    static void C(Material m,string p,Color v) { if(m.HasProperty(p))m.SetColor(p,v); }
    static Material Make(string root,Shader shader,string name,string textureName,Color color)
    {
        var path=root+"/Materials/"+name+".mat";
        var m=AssetDatabase.LoadAssetAtPath<Material>(path);
        if(!m) {m=new Material(shader);AssetDatabase.CreateAsset(m,path);} else m.shader=shader;
        string[] suffixes={"BaseMap","1st_ShadeMap","2nd_ShadeMap","OutlineMap"};
        string[] properties={"_MainTex","_1st_ShadeMap","_2nd_ShadeMap","_Outline_Sampler"};
        for(int i=0;i<suffixes.Length;i++)
        {
            var tp=root+"/Textures/"+textureName+"_"+suffixes[i]+".png";
            var imp=AssetImporter.GetAtPath(tp) as TextureImporter;
            if(imp==null) throw new System.Exception("Missing texture: "+tp);
            imp.textureType=TextureImporterType.Default;imp.sRGBTexture=i!=3;imp.maxTextureSize=2048;
            imp.wrapMode=TextureWrapMode.Clamp;imp.mipmapEnabled=true;
            imp.textureCompression=TextureImporterCompression.Uncompressed;imp.SaveAndReimport();
            var tex=AssetDatabase.LoadAssetAtPath<Texture2D>(tp);
            if(!m.HasProperty(properties[i]))throw new System.Exception("UTS2 property missing: "+properties[i]);
            m.SetTexture(properties[i],tex);
        }
        C(m,"_BaseColor",Color.white);C(m,"_Color",Color.white);
        C(m,"_1st_ShadeColor",Color.white);C(m,"_2nd_ShadeColor",Color.white);
        F(m,"_Use_BaseAs1st",0);F(m,"_Use_1stAs2nd",0);
        F(m,"_BaseColor_Step",.52f);F(m,"_BaseShade_Feather",.08f);
        F(m,"_ShadeColor_Step",.27f);F(m,"_1st2nd_Shades_Feather",.06f);
        F(m,"_Is_LightColor_Base",1);F(m,"_Is_LightColor_1st_Shade",1);F(m,"_Is_LightColor_2nd_Shade",1);
        F(m,"_Set_SystemShadowsToBase",1);F(m,"_Is_NormalMapToBase",0);
        C(m,"_HighColor",new Color(.12f,.14f,.16f,1));F(m,"_HighColor_Power",.65f);F(m,"_Is_SpecularToHighColor",1);
        F(m,"_RimLight",1);C(m,"_RimLightColor",new Color(.12f,.16f,.19f,1));F(m,"_RimLight_Power",.8f);
        C(m,"_Outline_Color",new Color(.035f,.043f,.051f,1));F(m,"_Outline_Width",.35f);F(m,"_Is_BlendBaseColor",0);F(m,"_Is_OutlineTex",0);
        F(m,"_ClippingMode",0);F(m,"_TransparentEnabled",0);F(m,"_CullMode",2);F(m,"_ZWriteMode",1);
        m.EnableKeyword("_IS_CLIPPING_OFF");m.DisableKeyword("_IS_CLIPPING_MODE");m.DisableKeyword("_IS_CLIPPING_TRANSMODE");
        m.EnableKeyword("_EMISSIVE_SIMPLE");m.EnableKeyword("_OUTLINE_NML");m.renderQueue=2000;
        EditorUtility.SetDirty(m);return m;
    }
}
