using UnityEditor;
using UnityEngine;
using System.IO;

[HelpURL("https://docs.unity3d.com/ScriptReference/RuntimeInitializeOnLoadMethodAttribute.html")]
public class RuntimeInitializer : ScriptableObject
{
    private static string fileName = "RuntimeInitializerSetting"; // Change this if needed, but remember to rename the existing asset as well.
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
    [Tooltip("When starting up the runtime. Called before the first scene is loaded.")]
    [SerializeField] private GameObject[] subsystemRegistrationPrefabs;
    [Tooltip("Before the splash screen is shown. At this time the objects of the first scene have not been loaded yet.")]
    [SerializeField] private GameObject[] beforeSplashScreenPrefabs;
    [Tooltip("When all assemblies are loaded and preloaded assets are initialized. At this time the objects of the first scene have not been loaded yet.")]
    [SerializeField] private GameObject[] afterAssembliesLoadedPrefabs;
    [Tooltip("When the first scene's objects are loaded into memory but before Awake has been called.")]
    [SerializeField] private GameObject[] beforeSceneLoadPrefabs;
    [Tooltip("When the first scene's objects are loaded into memory and after Awake has been called.")]
    [SerializeField] private GameObject[] afterSceneLoadPrefabs;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void SubsystemRegistration()
    {
        if (Instance && Instance.debug) { Debug.Log($"{nameof(RuntimeInitializer)}: Start instantiation by <b>{fileName}</b>", Instance); }
        if (Instance) InstantiateAll(Instance.subsystemRegistrationPrefabs, RuntimeInitializeLoadType.SubsystemRegistration);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    static void BeforeSplashScreen()
    {
        if (Instance) InstantiateAll(Instance.beforeSplashScreenPrefabs, RuntimeInitializeLoadType.BeforeSplashScreen);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static void AfterAssembliesLoaded()
    {
        if (Instance) InstantiateAll(Instance.afterAssembliesLoadedPrefabs, RuntimeInitializeLoadType.AfterAssembliesLoaded);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void BeforeSceneLoad()
    {
        if (Instance) InstantiateAll(Instance.beforeSceneLoadPrefabs, RuntimeInitializeLoadType.BeforeSceneLoad);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AfterSceneLoad()
    {
        if (Instance) InstantiateAll(Instance.afterSceneLoadPrefabs, RuntimeInitializeLoadType.AfterSceneLoad);
    }

    static void InstantiateAll(GameObject[] prefabList, RuntimeInitializeLoadType type)
    {
        foreach (GameObject prefab in prefabList)
        {
            GameObject instantiatedPrefab = Instantiate(prefab);
            instantiatedPrefab.name = prefab.name;
            if (Instance.debug) Debug.Log($"{nameof(RuntimeInitializer)}: Instantiate <b>{instantiatedPrefab.name}</b> on {type}", prefab);
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
            Debug.LogWarning($"{nameof(RuntimeInitializer)}: <b>{fileName}</b> asset already exists, you may rename the existing one.", asset);
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
