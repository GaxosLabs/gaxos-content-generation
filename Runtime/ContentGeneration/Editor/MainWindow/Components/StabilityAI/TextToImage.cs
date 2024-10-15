using System.Collections.Generic;
using System.Threading.Tasks;
using ContentGeneration.Models;
using ContentGeneration.Models.Stability;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.StabilityAI
{
    public class TextToImage : ParametersBasedGenerator<TextToImageParameters, StabilityTextToImageParameters>
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

        protected override string apiMethodName => nameof(ContentGenerationApi.RequestStabilityTextToImageGeneration);
        protected override Task RequestToApi(StabilityTextToImageParameters parameters,
            GenerationOptions generationOptions, object data)
        {
            return ContentGenerationApi.Instance.RequestStabilityTextToImageGeneration(
                parameters,
                generationOptions,
                data: data);
        }

        public override Generator generator => Generator.StabilityTextToImage;
    }
}