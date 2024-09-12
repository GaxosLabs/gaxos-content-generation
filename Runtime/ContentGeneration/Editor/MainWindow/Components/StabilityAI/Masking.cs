using System.Collections.Generic;
using System.Threading.Tasks;
using ContentGeneration.Models;
using ContentGeneration.Models.Stability;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.StabilityAI
{
    public class Masking : ParametersBasedGenerator<MaskingParameters, StabilityMaskedImageParameters>
    {
        public new class UxmlFactory : UxmlFactory<Masking, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        protected override string apiMethodName => nameof(ContentGenerationApi.RequestStabilityMaskedImageGeneration);
        protected override Task RequestToApi(StabilityMaskedImageParameters parameters, GenerationOptions generationOptions, object data)
        {
            return ContentGenerationApi.Instance.RequestStabilityMaskedImageGeneration(
                    parameters,
                    generationOptions, data: data);
        }
    }
}