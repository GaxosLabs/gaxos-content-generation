using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContentGeneration.Editor.MainWindow.Components.Meshy;
using ContentGeneration.Helpers;
using ContentGeneration.Models;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.RequestsList
{
    public class RequestsListTab : VisualElementComponent
    {
        public new class UxmlFactory : UxmlFactory<RequestsListTab, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        Button refreshButton => this.Q<Button>("refreshButton");
        MultiColumnListView listView => this.Q<MultiColumnListView>();
        IRequestedItem defaultRequestedItem => this.Q<RequestedItem>("defaultRequestedItem");
        IRequestedItem meshyTextToMeshRequestedItem => this.Q<MeshyTextToMeshRequestedItem>("meshyTextToMeshRequestedItem");
        IRequestedItem meshyTextToTextureRequestedItem => this.Q<MeshyTextToTextureRequestedItem>("meshyTextToTextureRequestedItem");

        IRequestedItem[] allRequestedItems => new[]
        {
            defaultRequestedItem, meshyTextToMeshRequestedItem, meshyTextToTextureRequestedItem
        };

        readonly List<Request> _requests = new();

        public RequestsListTab()
        {
            refreshButton.RegisterCallback<ClickEvent>(_ => { Refresh(); });

            listView.itemsSource = _requests;
            if(!string.IsNullOrEmpty(Settings.instance.apiKey))
            {
                Refresh();
            }

            Func<VisualElement> CreateCell(int i1)
            {
                return () =>
                {
                    var ret = new Label
                    {
                        name = $"p{i1}"
                    };
                    ret.AddToClassList(ret.name);

                    return ret;
                };
            }

            for (var i = 0; i < listView.columns.Count; i++)
            {
                var listViewColumn = listView.columns[i];
                listViewColumn.makeCell = CreateCell(i);
            }

            listView.columns["id"].bindCell = (element, index) =>
                (element as Label)!.text = _requests[index].ID.ToString();
            listView.columns["generator"].bindCell = (element, index) =>
                (element as Label)!.text = _requests[index].Generator.ToString();
            listView.columns["created"].bindCell = (element, index) =>
                (element as Label)!.text = _requests[index].CreatedAt.ToString(CultureInfo.InvariantCulture);
            listView.columns["completed"].bindCell = (element, index) =>
                (element as Label)!.text = _requests[index].CompletedAt.ToString(CultureInfo.InvariantCulture);
            listView.columns["status"].bindCell = (element, index) =>
            {
                var label = (element as Label)!;
                label.text = _requests[index].Status.ToString();
                label.RemoveFromClassList("generated");
                label.RemoveFromClassList("failed");
                label.AddToClassList(_requests[index].Status.ToString().ToLower());
            };

            foreach (var requestedItem in allRequestedItems)
            {
                requestedItem.OnDeleted += () =>
                {
                    listView.selectedIndex = -1;
                    Refresh();
                };
                requestedItem.style.display = DisplayStyle.None;
            }
            listView.selectionChanged += objects =>
            {
                var objectsArray = objects.ToArray();
                foreach (var requestedItem in allRequestedItems)
                {
                    requestedItem.value = null;
                }
                if (objectsArray.Length > 0)
                {
                    var request = (objectsArray[0] as Request)!;
                    if (request?.Generator == Generator.MeshyTextToMesh)
                    {
                        meshyTextToMeshRequestedItem.value = request;
                    }
                    else if (request?.Generator == Generator.MeshyTextToTexture)
                    {
                        meshyTextToTextureRequestedItem.value = request;
                    }
                    else
                    {
                        defaultRequestedItem.value = request;
                    }
                }
                foreach (var requestedItem in allRequestedItems)
                {
                    requestedItem.style.display =
                        requestedItem.value == null ? DisplayStyle.None : DisplayStyle.Flex;
                }
            };
        }

        CancellationTokenSource _lastRequest;

        void Refresh()
        {
            _lastRequest?.Cancel();

            var thisRequest = _lastRequest = new CancellationTokenSource();
            RefreshAsync(thisRequest).CatchAndLog();
        }

        async Task RefreshAsync(CancellationTokenSource thisRequest)
        {
            _requests.Clear();
            var requests = await ContentGenerationApi.Instance.GetRequests();
            if (thisRequest.IsCancellationRequested)
            {
                return;
            }

            _requests.AddRange(requests);
            listView.selectedIndex = -1;
            listView.RefreshItems();
        }
    }
}