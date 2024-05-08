using System.Linq;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace ContentGeneration
{
    [DefaultExecutionOrder(-10000)]
    public class Settings : ScriptableObject
    {
#if UNITY_EDITOR
        const string Path = "Assets/ContentGeneration/" + nameof(Settings) + ".asset";
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


        [SerializeField] string _apiKey;
        public string apiKey => _apiKey;
    }
}