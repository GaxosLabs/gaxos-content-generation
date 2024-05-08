using System;
using System.Threading.Tasks;
using ContentGeneration;
using UnityEngine;
using Object = UnityEngine.Object;

public static class TaskExtension
{
    public static void CatchAndLog(this Task t, Object context = null)
    {
        t.ContinueWith(taskResult =>
        {
            if (taskResult.IsFaulted)
            {
                Debug.LogException(taskResult.Exception!.InnerException, context);
            }
        });
    }

    public static void ContinueInMainThreadWith(this Task t,
        Action<Task> continuationAction)
    {
        t.ContinueWith(tResult =>
        {
            Dispatcher.instance.ToMainThread(() =>
            {
                continuationAction(tResult);
            });
        });
    }
    public static void ContinueInMainThreadWith<T>(this Task<T> t,
        Action<Task<T>> continuationAction)
    {
        t.ContinueWith(tResult =>
        {
            Dispatcher.instance.ToMainThread(() =>
            {
                continuationAction(tResult);
            });
        });
    }
}