using System;
using System.Collections.Generic;
using ContentGeneration.Models.Stability;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.StabilityAI
{
    public class TextToImage : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<TextToImage, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }
        
        TextField code => this.Q<TextField>("code");
        DropdownField engine => this.Q<DropdownField>("engine");
        DropdownField resolution => this.Q<DropdownField>("resolution");
        Button generateButton => this.Q<Button>("generate");
        VisualElement sendingRequest => this.Q<VisualElement>("sendingRequest");
        VisualElement requestSent => this.Q<VisualElement>("requestSent");
        VisualElement requestFailed => this.Q<VisualElement>("requestFailed");
        StabilityParametersElement stabilityParameters => this.Q<StabilityParametersElement>("stabilityParameters");
        GenerationOptionsElement generationOptions => this.Q<GenerationOptionsElement>("generationOptions");
        
        public TextToImage()
        {
            var asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/ContentGeneration/Editor/MainWindow/Components/StabilityAI/TextToImage.uxml");
            asset.CloneTree(this);

            stabilityParameters.OnCodeChanged += RefreshCode;
            generationOptions.OnCodeChanged += RefreshCode;

            var engines = new[]
            {
                "esrgan-v1-x2plus",
                "stable-diffusion-xl-1024-v0-9",
                "stable-diffusion-xl-1024-v1-0",
                "stable-diffusion-v1-6",
                "stable-diffusion-512-v2-1",
                "stable-diffusion-xl-beta-v2-2-2",
            };
            engine.choices = new List<string>(engines);
            engine.index = Array.IndexOf(engines, "stable-diffusion-v1-6");
            
            void RefreshResolutionDropdown(string value)
            {
                resolution.choices.Clear();
                uint xMin;
                uint xMax;
                uint yMin;
                uint yMax;
                switch (value)
                {
                    case "stable-diffusion-xl-1024-v0-9":
                    case "stable-diffusion-xl-1024-v1-0":
                        resolution.choices.AddRange(new[]
                        {
                            "1024x1024",
                            "1152x896",
                            "1216x832",
                            "1344x768",
                            "1536x640",
                            "640x1536",
                            "768x1344",
                            "832x1216",
                            "896x1152"
                        });
                        resolution.value = "1024x1024";
                        return;
                    case "stable-diffusion-v1-6":
                        xMin = 320;
                        xMax = 1536;
                        yMin = 320;
                        yMax = 1536;
                        break;
                    case "stable-diffusion-xl-beta-v2-2-2":
                        xMin = 128;
                        xMax = 512;
                        yMin = 128;
                        yMax = 512;
                        break;
                    case "esrgan-v1-x2plus":
                    case "stable-diffusion-512-v2-1":
                        xMin = 128;
                        xMax = 1536;
                        yMin = 128;
                        yMax = 1536;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(StabilityTextToImageParameters.EngineId), value);
                }
        
                for (var x = xMin; x <= xMax; x += 64)
                {
                    for (var y = yMin; y <= yMax; y += 64)
                    {
                        resolution.choices.Add($"{x}x{y}");
                    }
                }
        
                if (value == "stable-diffusion-xl-beta-v2-2-2")
                {
                    for (var i = 512 + 64; i <= 896; i += 64)
                    {
                        resolution.choices.Add($"512x{i}");
                        resolution.choices.Add($"{i}x512");
                    }
                }
        
                resolution.value = "512x512";
            }
        
            engine.RegisterValueChangedCallback(evt => { RefreshResolutionDropdown(evt.newValue); });
            RefreshResolutionDropdown(engine.value);
        
            sendingRequest.style.display = DisplayStyle.None;
            requestSent.style.display = DisplayStyle.None;
            requestFailed.style.display = DisplayStyle.None;
        
            generateButton.RegisterCallback<ClickEvent>(_ =>
            {
                if (!generateButton.enabledSelf) return;
        
                requestSent.style.display = DisplayStyle.None;
                requestFailed.style.display = DisplayStyle.None;
                if (!stabilityParameters.Valid())
                {
                    return;
                }
        
                generateButton.SetEnabled(false);
                sendingRequest.style.display = DisplayStyle.Flex;
        
                var resolutionSplit = resolution.value.Split('x');
                var parameters = new StabilityTextToImageParameters
                {
                    EngineId = engine.value,
                    Width = uint.Parse(resolutionSplit[0]),
                    Height = uint.Parse(resolutionSplit[1]),
                };
                stabilityParameters.ApplyParameters(parameters);
                ContentGenerationApi.Instance.RequestStabilityTextToImageGeneration(
                    parameters,
                    generationOptions.GetGenerationOptions()).ContinueInMainThreadWith(
                    t =>
                    {
                        generateButton.SetEnabled(true);
                        sendingRequest.style.display = DisplayStyle.None;
                        if (t.IsFaulted)
                        {
                            requestFailed.style.display = DisplayStyle.Flex;
                            Debug.LogException(t.Exception);
                        }
                        else
                        {
                            requestSent.style.display = DisplayStyle.Flex;
                        }
                    });
            });
        
            engine.RegisterValueChangedCallback(_ => RefreshCode());
            resolution.RegisterValueChangedCallback(_ => RefreshCode());
        
            code.SetVerticalScrollerVisibility(ScrollerVisibility.Auto);
            RefreshCode();
        }
        
        void RefreshCode()
        {
            var resolutionSplit = this.resolution.value.Split('x');
            code.value =
                "var requestId = await ContentGenerationApi.Instance.RequestTextToImageGeneration\n" +
                "\t(new StabilityTextToImageParameters\n" +
                "\t{\n" +
                $"\t\tEngineId = \"{engine.value}\",\n" +
                $"\t\tWidth = {uint.Parse(resolutionSplit[0])},\n" +
                $"\t\tHeight = {uint.Parse(resolutionSplit[1])},\n" +
                stabilityParameters.GetCode() +
                "\t},\n" +
                $"{generationOptions.GetCode()}" +
                ")";
        }
    }
}