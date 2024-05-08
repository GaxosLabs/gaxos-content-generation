using System;
using System.Collections.Generic;
using System.Threading;
using ContentGeneration.Editor.MainWindow.Components.RequestsList;
using ContentGeneration.Models;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.Meshy
{
    public class MeshyTextToMeshRequestedItem : VisualElement, IRequestedItem
    {
        public new class UxmlFactory : UxmlFactory<MeshyTextToMeshRequestedItem, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        Label refineStatus => this.Q<Label>("refineStatus");
        VisualElement refineErrorDetails => this.Q<VisualElement>("refineErrorDetails");
        Label refineError => this.Q<Label>("refineError");
        Button videoButton => this.Q<Button>("videoButton");
        Button refineButton => this.Q<Button>("refineButton");
        Button saveButton => this.Q<Button>("saveButton");

        RequestedItemCommon requestedItemCommon => this.Q<RequestedItemCommon>();

        public MeshyTextToMeshRequestedItem()
        {
            var asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/ContentGeneration/Editor/MainWindow/Components/Meshy/MeshyTextToMeshRequestedItem.uxml");
            asset.CloneTree(this);
            requestedItemCommon.OnDeleted += () =>
            {
                OnDeleted?.Invoke();
            };
            requestedItemCommon.OnRefreshed += v => value = v;

            refineButton.SetEnabled(false);
            refineButton.clicked += () =>
            {
                if (!refineButton.enabledSelf)
                    return;

                refineButton.SetEnabled(false);
                ContentGenerationApi.Instance.RefineMeshyTextToMesh(value.ID).ContinueInMainThreadWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        Debug.LogException(t.Exception!.InnerException);
                    }
                    else
                    {
                        requestedItemCommon.Refresh();
                    }

                    refineButton.SetEnabled(true);
                });
            };
            videoButton.SetEnabled(false);
            videoButton.clicked += () =>
            {
                Application.OpenURL(
                    value.GenerationResult["refineResult"]?["video_url"]!.ToObject<string>() ??
                    value.GenerationResult["video_url"]!.ToObject<string>());
            };
            saveButton.clicked += () =>
            {
                if (!saveButton.enabledSelf)
                    return;

                saveButton.SetEnabled(false);
                MeshyModelHelper.Save(
                    value.GenerationResult["refineResult"] ?? value.GenerationResult
                    ).ContinueInMainThreadWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        Debug.LogException(t.Exception!.InnerException);
                    }

                    saveButton.SetEnabled(true);
                });
            };
        }

        CancellationTokenSource _cancellationTokenSource;
        public event Action OnDeleted;

        public Request value
        {
            get => requestedItemCommon.value;
            set
            {
                requestedItemCommon.value = value;

                _cancellationTokenSource?.Cancel();

                if (value == null)
                    return;

                videoButton.SetEnabled(value.GenerationResult != null);
                refineButton.SetEnabled(
                    value.GenerationResult != null && !value.GenerationResult.ContainsKey("refineStatus"));

                refineStatus.text = "Not requested";
                refineErrorDetails.style.display = DisplayStyle.None;

                if (value.GenerationResult != null && value.GenerationResult.ContainsKey("refineStatus"))
                {
                    var refineStatusText = value.GenerationResult["refineStatus"]!.ToObject<string>();
                    var refineStatusValue = Enum.Parse<RequestStatus>(refineStatusText, true);

                    refineStatus.text = refineStatusValue.ToString();
                    refineStatus.ClearClassList();
                    refineStatus.AddToClassList(refineStatus.text.ToLower());

                    refineErrorDetails.style.display =
                        refineStatusValue == RequestStatus.Failed ? DisplayStyle.Flex : DisplayStyle.None;
                    refineError.text = value.FailedDetails?.Message +
                                       (string.IsNullOrEmpty(value.FailedDetails?.Error)
                                           ? ""
                                           : $" [{value.FailedDetails?.Error}]");
                }

                _cancellationTokenSource = new CancellationTokenSource();
            }
        }
    }
}