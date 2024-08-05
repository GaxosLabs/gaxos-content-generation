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
    public class ContentGenerationStore
    {
        public static ContentGenerationStore Instance = new();
        ContentGenerationStore()
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

        public Stats stats { get; private set; }
        public event Action<Stats> OnStatsChanged;
        CancellationTokenSource _lastRefreshStatsRequest;
        public async Task RefreshStatsAsync()
        {
            _lastRefreshStatsRequest?.Cancel();
            var cts = _lastRefreshRequestsListRequest = new CancellationTokenSource();
            var currentStats = await ContentGenerationApi.Instance.GetStats();
            if (cts.IsCancellationRequested)
            {
                return;
            }

            stats = currentStats;
            OnStatsChanged?.Invoke(currentStats);
        }
    }
}