using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components
{
    public class Tab : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<Tab, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            readonly UxmlStringAttributeDescription _tabName = new() { name = "Tab-Name" };
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get
                {
                    yield return new UxmlChildElementDescription(typeof (VisualElement));
                }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var element = (Tab)ve;
                element.tabName = _tabName.GetValueFromBag(bag, cc);
            }
        }

        public string tabName { get; private set; }

        public override VisualElement contentContainer => this.Q<VisualElement>("tabContents");

        public Tab()
        {
            var asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/ContentGeneration/Editor/MainWindow/Components/Tab.uxml");
            asset.CloneTree(this);
        }
    }
}

