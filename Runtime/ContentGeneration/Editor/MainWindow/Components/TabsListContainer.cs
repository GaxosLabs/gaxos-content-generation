using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components
{
    public class TabsListContainer : VisualElementComponent
    {
        public new class UxmlFactory : UxmlFactory<TabsListContainer, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield return new UxmlChildElementDescription(typeof(Tab)); }
            }
        }

        DropdownField list => this.Q<DropdownField>("list");
        public override VisualElement contentContainer => this.Q<VisualElement>("contentContainer");

        readonly HashSet<string> createdTabs = new();

        public TabsListContainer()
        {
            list.RegisterValueChangedCallback(v =>
            {
                foreach (var visualElement in contentContainer!.Children())
                {
                    if (visualElement is Tab t)
                    {
                        if (t.tabName == v.newValue)
                        {
                            visualElement.style.display = DisplayStyle.Flex;
                            continue;
                        }
                    }
                    visualElement.style.display = DisplayStyle.None;
                }
            });
            RegisterCallback<AttachToPanelEvent>(_ =>
            {
                foreach (var visualElement in contentContainer!.Children())
                {
                    if (visualElement is Tab t)
                    {
                        if (createdTabs.Add(t.tabName))
                        {
                            list.choices.Add(t.tabName);
                        }
                    }
                }

                list.index = 0;
            });
        }
    }
}