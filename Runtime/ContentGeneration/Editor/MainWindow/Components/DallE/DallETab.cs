using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.DallE
{
    public class DallETab: VisualElement
    {
        public new class UxmlFactory : UxmlFactory<DallETab, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }
        
        public DallETab()
        {
            var asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/ContentGeneration/Editor/MainWindow/Components/DallE/DallETab.uxml");
            asset.CloneTree(this);
        }
    }
}
