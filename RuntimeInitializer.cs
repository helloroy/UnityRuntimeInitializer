using UnityEditor;
using UnityEngine;

[HelpURL("https://docs.unity3d.com/ScriptReference/RuntimeInitializerLoadType.html")]
public class RuntimeInitializer : ScriptableObject
{
    private static string fileName = "RuntimeInitializeSettings";
    private static RuntimeInitializer _instance;
    private static RuntimeInitializer Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = Resources.Load<RuntimeInitializer>(fileName);
            }
            return _instance;
        }
    }

    [SerializeField] private bool debug;
    [SerializeField] private GameObject[] beforeSplashScreen;
    [SerializeField] private GameObject[] beforeSceneLoad;
    [SerializeField] private GameObject[] afterSceneLoad;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    static void BeforeSplashScreen()
    {
        if(Instance) InstantiateAll(Instance.beforeSplashScreen);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void BeforeSceneLoad()
    {
        if (Instance) InstantiateAll(Instance.beforeSceneLoad);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AfterSceneLoad()
    {
        if (Instance) InstantiateAll(Instance.afterSceneLoad);
    }

    static void InstantiateAll(GameObject[] objectList)
    {
        foreach (GameObject prefab in objectList)
        {
            GameObject addedPrefab = Instantiate(prefab);
            addedPrefab.name = prefab.name;
            if (Instance.debug) Debug.Log($"Instantiate {addedPrefab.name}", prefab);
        }
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Runtime Initialize Settings")]
    static void Create()
    {
        RuntimeInitializer[] assets = Resources.LoadAll<RuntimeInitializer>("");

        if (assets.Length > 0)
        {
            Debug.LogWarning($"StartUp asset exists already.", assets[0]);
        }
        else
        {
            RuntimeInitializer asset = CreateInstance<RuntimeInitializer>();
            AssetDatabase.CreateAsset(asset, "Assets/Resources/"+ fileName +".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            Debug.Log("Runtime Initialize Settings asset created, more details please check https://docs.unity3d.com/ScriptReference/RuntimeInitializerLoadType.html");
        }
    }
#endif
}
