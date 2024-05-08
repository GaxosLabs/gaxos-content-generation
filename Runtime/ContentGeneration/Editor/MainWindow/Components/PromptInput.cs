using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components
{
    public class PromptInput : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<PromptInput, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        TextField text => this.Q<TextField>("text");
        public string value
        {
            get => text.value;
            set => text.value = value;
        }

        public event Action<string> OnChanged;
        public PromptInput()
        {
            var asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/ContentGeneration/Editor/MainWindow/Components/PromptInput.uxml");
            asset.CloneTree(this);

            text.RegisterValueChangedCallback(v => OnChanged?.Invoke(v.newValue));
        }
    }
}
