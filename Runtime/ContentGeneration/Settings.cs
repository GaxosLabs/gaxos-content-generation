using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif

namespace ContentGeneration
{
    [DefaultExecutionOrder(-10000)]
    public class Settings : ScriptableObject
    {
#if UNITY_EDITOR
        class EnsurePreloadedProcessor : IPreprocessBuildWithReport
        {
            public int callbackOrder => 0;
            public void OnPreprocessBuild(BuildReport report)
            {
                CheckCreated();
            }
        }
        
        const string Path = "Assets/" + nameof(ContentGeneration) + "." + nameof(Settings) + ".asset";
        [InitializeOnLoadMethod]
        static void CheckCreated()
        {
            var settings = AssetDatabase.LoadAssetAtPath<Settings>(Path);
            if (settings == null)
            {
                settings = CreateInstance<Settings>();
                AssetDatabase.CreateAsset(settings, Path);
                AssetDatabase.SaveAssets();
            }

            var preloadedAssets = PlayerSettings.GetPreloadedAssets().ToList();
            if(!preloadedAssets.Contains(settings))
            {
                preloadedAssets.Add(settings);
                PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
            }
        }
#endif
        
        public static Settings instance { get; private set; }
        void OnEnable()
        {
            instance = this;
        }

        [SerializeField]
        public string apiKey;
    }
}