using System;
using System.Threading.Tasks;
using ContentGeneration.Helpers;
using ContentGeneration.Models;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components
{
    public interface IParameters<T>
    {
        GenerationOptionsElement generationOptions { get; }
        Action codeHasChanged { set; }
        bool Valid();
        void ApplyParameters(T parameters);
        string GetCode();
    }
    
    public abstract class ParametersBasedGenerator<T, TU> : VisualElementComponent 
        where T : VisualElement, IParameters<TU>
        where TU: new()
    {
        T parameters => this.Q<T>("parameters");

        TextField code => this.Q<TextField>("code");
        Button generateButton => this.Q<Button>("generate");
        VisualElement sendingRequest => this.Q<VisualElement>("sendingRequest");
        VisualElement requestSent => this.Q<VisualElement>("requestSent");
        VisualElement requestFailed => this.Q<VisualElement>("requestFailed");

        protected ParametersBasedGenerator()
        {
            parameters.codeHasChanged = RefreshCode;
            if(parameters.generationOptions!=null)
            {
                parameters.generationOptions.OnCodeHasChanged = RefreshCode;
            }

            sendingRequest.style.display = DisplayStyle.None;
            requestSent.style.display = DisplayStyle.None;
            requestFailed.style.display = DisplayStyle.None;

            generateButton.RegisterCallback<ClickEvent>(_ =>
            {
                if (!generateButton.enabledSelf) return;

                requestSent.style.display = DisplayStyle.None;
                requestFailed.style.display = DisplayStyle.None;
                if (!parameters.Valid())
                {
                    return;
                }

                generateButton.SetEnabled(false);
                sendingRequest.style.display = DisplayStyle.Flex;

                var stabilityTextToImageParameters = new TU();
                parameters.ApplyParameters(stabilityTextToImageParameters);
                RequestToApi(
                    stabilityTextToImageParameters,
                    parameters.generationOptions?.GetGenerationOptions(), 
                    data: new
                    {
                        player_id = ContentGenerationStore.editorPlayerId
                    }).ContinueInMainThreadWith(
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
                        ContentGenerationStore.Instance.RefreshRequestsAsync().Finally(() => ContentGenerationStore.Instance.RefreshStatsAsync().CatchAndLog());
                    });
            });

            code.SetVerticalScrollerVisibility(ScrollerVisibility.Auto);
            RefreshCode();
        }

        protected abstract Task RequestToApi(TU parameters, GenerationOptions generationOptions, object data);

        protected abstract string apiMethodName { get; }
        void RefreshCode()
        {
            code.value =
                $"var requestId = await ContentGenerationApi.Instance.{apiMethodName}\n" +
                $"\t(new {typeof(TU).Name}\n" +
                "\t{\n" +
                parameters.GetCode() +
                "\t},\n" +
                parameters.generationOptions?.GetCode() +
                ")";
        }
    }
}