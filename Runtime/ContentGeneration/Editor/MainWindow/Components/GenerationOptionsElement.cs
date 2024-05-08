using System;
using System.Collections.Generic;
using ContentGeneration.Models;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components
{
    public class GenerationOptionsElement: VisualElement
    {
        public new class UxmlFactory : UxmlFactory<GenerationOptionsElement, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }
        
        Toggle makeTransparentColor => this.Q<Toggle>("makeTransparentColor");
        ColorField transparentColor => this.Q<ColorField>("transparentColor");
        Slider transparentColorReplaceDelta => this.Q<Slider>("transparentColorReplaceDelta");
        Toggle improvePrompt => this.Q<Toggle>("improvePrompt");
        
        public event Action OnCodeChanged;

        public GenerationOptionsElement()
        {
            var asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/ContentGeneration/Editor/MainWindow/Components/GenerationOptionsElement.uxml");
            asset.CloneTree(this);

            transparentColor.style.display =
                transparentColorReplaceDelta.style.display =
                    makeTransparentColor.value ? DisplayStyle.Flex : DisplayStyle.None;
            transparentColor.value = Color.magenta;
            makeTransparentColor.RegisterValueChangedCallback(evt =>
            {
                transparentColor.style.display =
                    transparentColorReplaceDelta.style.display =
                        evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
                OnCodeChanged?.Invoke();
            });

            makeTransparentColor.RegisterValueChangedCallback(_ => OnCodeChanged?.Invoke());
            transparentColorReplaceDelta.RegisterValueChangedCallback(_ => OnCodeChanged?.Invoke());
            transparentColor.RegisterValueChangedCallback(_ => OnCodeChanged?.Invoke());
            improvePrompt.RegisterValueChangedCallback(_ => OnCodeChanged?.Invoke());
        }

        public string GetCode()
        {
            return
                "\tnew GenerationOptions{\n" +
                (makeTransparentColor.value
                    ? $"\t\tTransparentColor = new Color({transparentColor.value.r}f, {transparentColor.value.g}f, {transparentColor.value.b}f),\n" +
                      $"\t\tReplaceDelta = {transparentColor.value}f,\n"
                    : "") +
                (improvePrompt.value ? $"\t\tImprovePrompt = {improvePrompt.value},\n" : "") +
                "\t}";
        }

        public GenerationOptions GetGenerationOptions()
        {
            var ret = new GenerationOptions();
            if (makeTransparentColor.value)
            {
                ret.TransparentColor = transparentColor.value;
                ret.TransparentColorReplaceDelta = transparentColorReplaceDelta.value;
            }

            ret.ImprovePrompt = improvePrompt.value;

            return ret;
        }
    }
}