using System.Collections.Generic;
using ContentGeneration.Helpers;
using ContentGeneration.Models.Comfy;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.Comfy
{
    public class TextToImage : VisualElementComponent
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

        SliderInt width => this.Q<SliderInt>("width");
        SliderInt height => this.Q<SliderInt>("height");

        TextField code => this.Q<TextField>("code");
        ComfyParametersElement comfyParametersElement => this.Q<ComfyParametersElement>("comfyParametersElement");
        GenerationOptionsElement generationOptionsElement => this.Q<GenerationOptionsElement>("generationOptions");
        VisualElement requestSent => this.Q<VisualElement>("requestSent");
        VisualElement requestFailed => this.Q<VisualElement>("requestFailed");
        VisualElement sendingRequest => this.Q<VisualElement>("sendingRequest");
        Button generateButton => this.Q<Button>("generateButton");

        public TextToImage()
        {
            comfyParametersElement.OnCodeChanged += RefreshCode;
            width.RegisterValueChangedCallback(_ => RefreshCode());
            height.RegisterValueChangedCallback(_ => RefreshCode());
            generationOptionsElement.OnCodeChanged += RefreshCode;

            requestSent.style.display = DisplayStyle.None;
            requestFailed.style.display = DisplayStyle.None;
            sendingRequest.style.display = DisplayStyle.None;

            generateButton.RegisterCallback<ClickEvent>(_ =>
            {
                if (!generateButton.enabledSelf) return;

                requestSent.style.display = DisplayStyle.None;
                requestFailed.style.display = DisplayStyle.None;

                if (!comfyParametersElement.Valid())
                {
                    return;
                }

                generateButton.SetEnabled(false);
                sendingRequest.style.display = DisplayStyle.Flex;


                var parameters = new ComfyTextToImageParameters
                {
                    Width = (uint)width.value,
                    Height = (uint)height.value 
                };
                comfyParametersElement.ApplyParameters(parameters);
                ContentGenerationApi.Instance.RequestComfyTextToImageGeneration(
                    parameters,
                    generationOptionsElement.GetGenerationOptions()).ContinueInMainThreadWith(
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

            RefreshCode();
        }

        void RefreshCode()
        {
            code.value =
                "var requestId = await ContentGenerationApi.Instance.RequestComfyTextToImageGeneration\n" +
                "\t(new ComfyTextToImageParameters\n" +
                "\t{\n" +
                comfyParametersElement?.GetCode() +
                "\t},\n" +
                $"{generationOptionsElement?.GetCode()}" +
                ")";
        }
    }
}