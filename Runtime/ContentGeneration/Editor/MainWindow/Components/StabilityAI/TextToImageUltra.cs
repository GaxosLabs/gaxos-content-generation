using System.Collections.Generic;
using System.Threading.Tasks;
using ContentGeneration.Models;
using ContentGeneration.Models.Stability;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.StabilityAI
{
    public class TextToImageUltra : ParametersBasedGenerator<TextToImageUltraParameters, StabilityUltraTextToImageParameters>
    {
        public new class UxmlFactory : UxmlFactory<TextToImageUltra, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        protected override string apiMethodName => nameof(ContentGenerationApi.RequestStabilityUltraTextToImageGeneration);
        protected override Task RequestToApi(StabilityUltraTextToImageParameters parameters,
            GenerationOptions generationOptions, object data)
        {
            return ContentGenerationApi.Instance.RequestStabilityUltraTextToImageGeneration(
                parameters,
                generationOptions,
                data: data);
        }
    }
}