using UnityEditor;
using UnityEngine;
using System.IO;

[HelpURL("https://docs.unity3d.com/ScriptReference/RuntimeInitializeLoadType.html")]
public class RuntimeInitializer : ScriptableObject
{
    private static string fileName = "RuntimeInitializerSetting"; // Change this if needed, but remember to rename the exists asset as well.
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
    [SerializeField] private GameObject[] subsystemRegistration;
    [SerializeField] private GameObject[] beforeSplashScreen;
    [SerializeField] private GameObject[] afterAssembliesLoaded;
    [SerializeField] private GameObject[] beforeSceneLoad;
    [SerializeField] private GameObject[] afterSceneLoad;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void SubsystemRegistration()
    {
        if (Instance && Instance.debug == true) { Debug.Log($"{nameof(RuntimeInitializer)}: Start instantiation by <b>{fileName}</b>", Instance); }
        if (Instance) InstantiateAll(Instance.subsystemRegistration, RuntimeInitializeLoadType.SubsystemRegistration);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    static void BeforeSplashScreen()
    {
        if (Instance) InstantiateAll(Instance.beforeSplashScreen, RuntimeInitializeLoadType.BeforeSplashScreen);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static void AfterAssembliesLoaded()
    {
        if (Instance) InstantiateAll(Instance.afterAssembliesLoaded, RuntimeInitializeLoadType.AfterAssembliesLoaded);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void BeforeSceneLoad()
    {
        if (Instance) InstantiateAll(Instance.beforeSceneLoad, RuntimeInitializeLoadType.BeforeSceneLoad);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AfterSceneLoad()
    {
        if (Instance) InstantiateAll(Instance.afterSceneLoad, RuntimeInitializeLoadType.AfterSceneLoad);
    }

    static void InstantiateAll(GameObject[] objectList, RuntimeInitializeLoadType type)
    {
        foreach (GameObject prefab in objectList)
        {
            GameObject addedPrefab = Instantiate(prefab);
            addedPrefab.name = prefab.name;
            if (Instance.debug) Debug.Log($"{nameof(RuntimeInitializer)}: Instantiate <b>{addedPrefab.name}</b> on {type}", prefab);
        }
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/RuntimeInitializerSetting")]
    public static void Create()
    {
        if (!Directory.Exists("Assets/Resources"))
        {
            Directory.CreateDirectory("Assets/Resources");
        }

        RuntimeInitializer asset = Resources.Load<RuntimeInitializer>(fileName);

        if (asset)
        {
            Debug.LogWarning($"{nameof(RuntimeInitializer)}: <b>{fileName}</b> asset exists already, you may rename the exists one.", asset);
        }
        else
        {
            RuntimeInitializer newAsset = CreateInstance<RuntimeInitializer>();
            AssetDatabase.CreateAsset(newAsset, "Assets/Resources/" + fileName + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = newAsset;
        }
    }
#endif
}
