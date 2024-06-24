using System.Collections.Generic;
using System.Globalization;
using ContentGeneration.Helpers;
using UnityEditor;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components
{
    public class Configuration : VisualElementComponent
    {
        public new class UxmlFactory : UxmlFactory<Configuration, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        public Configuration()
        {
            var apiKey = this.Q<TextField>("apiKey");
            apiKey.value = Settings.instance.apiKey;
            apiKey.RegisterValueChangedCallback(v =>
            {
                Settings.instance.apiKey = v.newValue;
                EditorUtility.SetDirty(Settings.instance);
                AssetDatabase.SaveAssets();
            });
            
            var credits = this.Q<TextField>("credits");
            var refreshCredits = this.Q<Button>("refreshCredits");
            void RefreshCredits(float v)
            {
                credits.value = v.ToString("r", CultureInfo.InvariantCulture);
            }
            MainWindowStore.Instance.OnCreditsChanged += RefreshCredits;
            RefreshCredits(MainWindowStore.Instance.credits);
            refreshCredits.clicked += () =>
            {
                if (!refreshCredits.enabledSelf)
                    return;
                refreshCredits.SetEnabled(false);
                MainWindowStore.Instance.RefreshCreditsAsync().Finally(() =>
                {
                    refreshCredits.SetEnabled(true);
                });
            };
            if(!string.IsNullOrEmpty(Settings.instance.apiKey))
            {
                MainWindowStore.Instance.RefreshCreditsAsync().CatchAndLog();
            }
        }
    }
}