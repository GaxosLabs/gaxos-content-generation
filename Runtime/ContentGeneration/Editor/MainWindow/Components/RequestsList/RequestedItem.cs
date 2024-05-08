using System;
using System.Collections.Generic;
using ContentGeneration.Models;
using UnityEditor;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.RequestsList
{
    public class RequestedItem : VisualElement, IRequestedItem
    {
        public new class UxmlFactory : UxmlFactory<RequestedItem, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        public event Action OnDeleted;

        public Request value
        {
            get => requestedItemCommon.value;
            set => requestedItemCommon.value = value;
        }

        RequestedItemCommon requestedItemCommon => this.Q<RequestedItemCommon>();

        public RequestedItem()
        {
            var asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/ContentGeneration/Editor/MainWindow/Components/RequestsList/RequestedItem.uxml");
            asset.CloneTree(this);
            requestedItemCommon.OnDeleted += () =>
            {
                OnDeleted?.Invoke();
            };
        }
    }
}