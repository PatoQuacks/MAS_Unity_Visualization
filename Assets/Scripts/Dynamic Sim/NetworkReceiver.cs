using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using UnityEngine;
public class ObjectPositionsPayload
{
    public List<Agent> agents;
    public List<Product> products;
}

public class Agent
{
    public string id;
    public List<float[]> positions;
}

public class Product
{
    public string id;
    public List<float[]> positions;
}

public class NetworkReceiver : MonoBehaviour
{

    [Header("Server Settings")]
    [SerializeField] private string listenUrl = "http://localhost:5005/";

    public static event Action<string[]> OnAgentListReceived;
    public static event Action<float[][][]> OnAgentPositionListReceived;

    public static event Action<string[]> OnProductListReceived;
    public static event Action<float[][][]> OnProductPositionListReceived;

    // Requiered attributes for the reading and handling of data sent by MAS simulator
    private HttpListener listener;
    private Thread listenerThread;
    private readonly Queue<Action> mainThreadActions = new Queue<Action>();
    private readonly object queueLock = new object();

    void Start()
    {
        StartServer();
    }

    void StartServer()
    {
        listener = new HttpListener();
        listener.Prefixes.Add(listenUrl);
        listener.Start();

        listenerThread = new Thread(ListenLoop);
        listenerThread.IsBackground = true;
        listenerThread.Start();

        Debug.Log($"NetworkReceiver listening on {listenUrl}");
    }

    void ListenLoop()
    {
        while (listener.IsListening)
        {
            try
            {
                HttpListenerContext context = listener.GetContext(); // blocks until a request arrives
                HandleRequest(context);
            }
            catch (Exception e)
            {
                // Thrown when listener.Stop() is called while GetContext() is blocking - safe to ignore on shutdown
                Debug.LogWarning($"Listener loop ended: {e.Message}");
            }
        }
    }

    void HandleRequest(HttpListenerContext context)
    {
        string json;
        using (var reader = new System.IO.StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
        {
            json = reader.ReadToEnd();
        }

        // Respond immediately so the Python client's request doesn't hang/timeout
        string responseText = "{\"status\":\"received\"}";
        byte[] buffer = Encoding.UTF8.GetBytes(responseText);
        context.Response.ContentType = "application/json";
        context.Response.ContentLength64 = buffer.Length;
        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        context.Response.OutputStream.Close();

        // Queue the actual parsing to run on Unity's main thread
        lock (queueLock)
        {
            mainThreadActions.Enqueue(() => ParseIncomingJson(json));
        }
    }

    void ParseIncomingJson(string json)
    {
        try
        {        
            ObjectPositionsPayload data = JsonConvert.DeserializeObject<ObjectPositionsPayload>(json);

            // Extract agent IDs
            string[] agentList = data.agents.Select(agent => agent.id).ToArray();

            // Extract positions
            float[][][] agentPositionList = data.agents.Select(agent => agent.positions.ToArray()).ToArray();
            
            Debug.Log($"Received data for {agentList.Length} agents.");

            // Extract product IDs
            string[] productList = data.products.Select(product => product.id).ToArray();

            // Extract positions
            float[][][] productPositionList = data.products.Select(product => product.positions.ToArray()).ToArray();
            
            Debug.Log($"Received data for {productList.Length} products.");

            // Handoff data to scripts subscribed to this event
            OnAgentListReceived?.Invoke(agentList);
            OnAgentPositionListReceived?.Invoke(agentPositionList);
            
            OnProductListReceived?.Invoke(productList);
            OnProductPositionListReceived?.Invoke(productPositionList);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to parse incoming JSON: {e.Message}");
        }
    }

    // Check the actions queued up by listener thread and act upon it on main thread
    void Update()
    {
        // Drain and execute any queued actions on the main thread
        lock (queueLock)
        {
            while (mainThreadActions.Count > 0)
            {
                mainThreadActions.Dequeue().Invoke();
            }
        }
    }

    // Close off the server
    void OnApplicationQuit()
    {
        StopServer();
    }

    void OnDestroy()
    {
        StopServer();
    }

    void StopServer()
    {
        if (listener != null && listener.IsListening)
        {
            listener.Stop();
            listener.Close();
        }
        listenerThread?.Join(500); // wait briefly for the thread to exit cleanly
    }
}
