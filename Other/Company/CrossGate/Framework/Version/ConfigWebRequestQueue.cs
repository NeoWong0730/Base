using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ConfigWebRequestQueueOperation
{
    private bool m_Completed = false;
    public UnityWebRequestAsyncOperation Result;
    public Action<UnityWebRequestAsyncOperation> OnComplete;

    public bool IsDone
    {
        get { return m_Completed || Result != null; }
    }

    public void Exit()
    {
        if (!IsDone)
        {
            if (OnComplete != null)
            {
                OnComplete = null;
            }
            if (Result != null)
            {
                Result = null;
            }
        }
    }

    internal UnityWebRequest m_WebRequest;

    public ConfigWebRequestQueueOperation(UnityWebRequest request)
    {
        m_WebRequest = request;
    }

    internal void Complete(UnityWebRequestAsyncOperation asyncOp)
    {
        m_Completed = true;
        Result = asyncOp;
        OnComplete?.Invoke(Result);
    }
}



public static class ConfigWebRequestQueue 
{
    internal static int s_MaxRequest = 5; //500;
    internal static Queue<ConfigWebRequestQueueOperation> s_QueuedOperations = new Queue<ConfigWebRequestQueueOperation>();
    internal static List<UnityWebRequestAsyncOperation> s_ActiveRequests = new List<UnityWebRequestAsyncOperation>();
    public static void SetMaxConcurrentRequests(int maxRequests)
    {
        if (maxRequests < 1)
            throw new ArgumentException("MaxRequests must be 1 or greater.", "maxRequests");
        s_MaxRequest = maxRequests;
    }

    public static ConfigWebRequestQueueOperation QueueRequest(UnityWebRequest request)
    {
        ConfigWebRequestQueueOperation queueOperation = new ConfigWebRequestQueueOperation(request);
        if (s_ActiveRequests.Count < s_MaxRequest)
        {
            UnityWebRequestAsyncOperation webRequestAsyncOp = null;
            try
            {
                webRequestAsyncOp = request.SendWebRequest();
                s_ActiveRequests.Add(webRequestAsyncOp);

                if (webRequestAsyncOp.isDone)
                    OnWebAsyncOpComplete(webRequestAsyncOp);
                else
                    webRequestAsyncOp.completed += OnWebAsyncOpComplete;

            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            queueOperation.Complete(webRequestAsyncOp);
        }
        else
            s_QueuedOperations.Enqueue(queueOperation);

        return queueOperation;
    }

    private static void OnWebAsyncOpComplete(AsyncOperation operation)
    {
        s_ActiveRequests.Remove((operation as UnityWebRequestAsyncOperation));

        if (s_QueuedOperations.Count > 0)
        {
            var nextQueuedOperation = s_QueuedOperations.Dequeue();
            var webRequestAsyncOp = nextQueuedOperation.m_WebRequest.SendWebRequest();
            webRequestAsyncOp.completed += OnWebAsyncOpComplete;
            s_ActiveRequests.Add(webRequestAsyncOp);
            nextQueuedOperation.Complete(webRequestAsyncOp);
        }
    }

}
