using System.Collections.Generic;
using System.Globalization;
using ContentGeneration.Helpers;
using UnityEditor;
using UnityEngine;
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
            void RefreshCredits()
            {
                refreshCredits.SetEnabled(false);
                credits.value = "";
                ContentGenerationApi.Instance.GetCredits().ContinueInMainThreadWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        Debug.LogException(t.Exception);
                    }
                    else
                    {
                        credits.value = t.Result.ToString("r", CultureInfo.InvariantCulture);
                    }
                    
                    refreshCredits.SetEnabled(true);
                });
            }
            refreshCredits.clicked += () =>
            {
                if (!refreshCredits.enabledSelf)
                    return;
                RefreshCredits();
            };
            RefreshCredits();
        }
    }
}