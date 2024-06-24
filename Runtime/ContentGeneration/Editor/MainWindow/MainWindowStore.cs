using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContentGeneration.Helpers;
using ContentGeneration.Models;
using Unity.EditorCoroutines.Editor;

namespace ContentGeneration.Editor.MainWindow
{
    public class MainWindowStore
    {
        public static MainWindowStore Instance = new();
        MainWindowStore()
        {
            
        }
        
        public readonly List<Request> Requests = new();
        public event Action<List<Request>> OnRequestsChanged;
        CancellationTokenSource _lastRefreshRequestsListRequest;
        EditorCoroutine _refreshRequestsCoroutine;
        public async Task RefreshRequestsAsync()
        {
            _lastRefreshRequestsListRequest?.Cancel();
            var cts = _lastRefreshRequestsListRequest = new CancellationTokenSource();
            var requests = await ContentGenerationApi.Instance.GetRequests();
            if (cts.IsCancellationRequested)
            {
                return;
            }

            Requests.Clear();
            Requests.AddRange(requests);

            if (requests.Any(i => i.Status == RequestStatus.Pending))
            {
                if (_refreshRequestsCoroutine == null)
                {
                    IEnumerator RefreshRequestListCo()
                    {
                        yield return new EditorWaitForSeconds(3);
                        RefreshRequestsAsync().CatchAndLog();
                        _refreshRequestsCoroutine = null;
                    }
                    _refreshRequestsCoroutine = EditorCoroutineUtility.StartCoroutineOwnerless(RefreshRequestListCo());
                }
            }
            else
            {
                if(_refreshRequestsCoroutine!= null)
                {
                    EditorCoroutineUtility.StopCoroutine(_refreshRequestsCoroutine);
                    _refreshRequestsCoroutine = null;
                }
            }
            
            OnRequestsChanged?.Invoke(Requests);
        }

        public float credits { get; private set; }
        public event Action<float> OnCreditsChanged;
        CancellationTokenSource _lastRefreshCreditsRequest;
        public async Task RefreshCreditsAsync()
        {
            _lastRefreshCreditsRequest?.Cancel();
            var cts = _lastRefreshRequestsListRequest = new CancellationTokenSource();
            var currentCredits = await ContentGenerationApi.Instance.GetCredits();
            if (cts.IsCancellationRequested)
            {
                return;
            }

            credits = currentCredits;
            OnCreditsChanged?.Invoke(currentCredits);
        }
    }
}