using System.Collections.Generic;
using System.Threading.Tasks;
using ContentGeneration.Models;
using ContentGeneration.Models.Meshy;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.Meshy
{
    public class TextToVoxel : ParametersBasedGenerator<TextToVoxelParameters, MeshyTextToVoxelParameters>
    {
        public new class UxmlFactory : UxmlFactory<TextToVoxel, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        protected override string apiMethodName => nameof(ContentGenerationApi.RequestMeshyTextToVoxelGeneration);

        protected override Task RequestToApi(MeshyTextToVoxelParameters parameters, GenerationOptions generationOptions,
            object data)
        {
            return ContentGenerationApi.Instance.RequestMeshyTextToVoxelGeneration(
                parameters,
                generationOptions, data: data);
        }
    }
}