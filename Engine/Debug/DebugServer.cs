using System.Net;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using Bean.Scenes;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

using DemoGame;
using System.Runtime.CompilerServices;

namespace Bean.Debug;

public static class DebugServer
{

    public static Prop FocucedProp;

    #if DEBUG
    public static void StartWebServer()
    {
        // HttpListener listener = new HttpListener();
        // listener.Prefixes.Add("http://localhost:8080/");
        // listener.Start();
        // Console.WriteLine("Server running on http://localhost:8080/");
        //
        // while (true)
        // {
        //     var context = listener.GetContext();
        //     var request = context.Request;
        //     var response = context.Response;
        //
        //     string responseString;
        //
        //     if (request.Url.AbsolutePath == "/")
        //     {
        //         responseString = File.ReadAllText(FileManager.ServerPath + "EngineDebug.html");
        //     }
        //     else
        //     {
        //         response.StatusCode = 404;
        //         responseString = "<html><body><h1>404 Not Found</h1></body></html>";
        //     }
        //
        //     byte[] buffer = Encoding.UTF8.GetBytes(responseString);
        //     response.ContentLength64 = buffer.Length;
        //     using var output = response.OutputStream;
        //     output.Write(buffer, 0, buffer.Length);
        // }
    }

    private static WebSocket? _connectedSocket = null;

    public static async void StartWebSocketServer()
    {
        // HttpListener listener = new HttpListener();
        // listener.Prefixes.Add("http://localhost:8081/ws/");
        // listener.Start();
        // Console.WriteLine("WebSocket server started on ws://localhost:8081/ws/");
        //
        // while (true)
        // {
        //     var context = await listener.GetContextAsync();
        //
        //     if (context.Request.IsWebSocketRequest)
        //     {
        //         var wsContext = await context.AcceptWebSocketAsync(null);
        //         _connectedSocket = wsContext.WebSocket;
        //
        //         Console.WriteLine("WebSocket connected.");
        //
        //         _ = Task.Run(async () =>
        //         {
        //             var buffer = new byte[1024];
        //             while (_connectedSocket.State == WebSocketState.Open)
        //             {
        //                 var result = await _connectedSocket.ReceiveAsync(buffer, CancellationToken.None);
        //                 var received = Encoding.UTF8.GetString(buffer, 0, result.Count);
        //
        //                 if (!received.Contains(": "))
        //                     FocucedProp = SceneManager.Instance.ActiveScene.GetSceneProps().First(i => i.PropID == received);
        //                 else
        //                 {
        //                     string[] split = received.Split(": ");
        //
        //                     string variableName = split[0];
        //
        //                     string value = split[1];
        //
        //                     FocucedProp.ChangeVariableValue(variableName, value);
        //                 }
        //             }
        //         });
        //     }
        // }
    }

    
    private static async Task SendToWebSocket(string message)
    {
        if (_connectedSocket != null && _connectedSocket.State == WebSocketState.Open)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            await _connectedSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }


#endif

    public static void Log(object message, object sender, Color? colour = null)
    {
        Console.WriteLine(message);
        
// #if DEBUG
//         string colourString = "rgb(255,255,255)";
//
//         if (colour != null)
//         {
//             Color notNullColour = (Color)colour;
//
//             colourString = $"rgb({notNullColour.R},{notNullColour.G},{notNullColour.B})";
//         }
//
//         string messageString = $"Log:{{colour}}{colourString}{{message}}{message}";
//
//         if (sender is Prop prop)
//         {
//             messageString = $"Log:{{colour}}{colourString}{{message}}{Time.Instance.GameTime?.TotalGameTime.TotalMilliseconds} : {prop.PropID}: {message}";
//         }
//
//
//         _ = SendToWebSocket(messageString);
//         #endif
    }

    public static void LogWarning(object message, object sender)
    {
        Log($"⚠️Warning {message} ⚠️", sender, Color.Yellow);
    }

    public static void SendPropList()
    {
// #if DEBUG
//         if (SceneManager.Instance.ActiveScene == null)
//             return;
//
//         Prop[] props = SceneManager.Instance.ActiveScene.GetSceneProps();
//
//         string messageString = "List:";
//
//         for (int i = 0; i < props.Length; i++)
//         {
//             Prop prop = props[i];
//
//             messageString += "{prop}" + prop.PropID;
//         }
//
//         _ = SendToWebSocket(messageString);
//         #endif
    }

    public static void UpdateSelectedProp()
    {
// #if DEBUG
//         if (FocucedProp == null)
//             return;
//
//         if (FocucedProp.ToRemove)
//         {
//             FocucedProp = null;
//             return;
//         }
//
//
//         string json = GeneratePropJson(FocucedProp);
//
//         _ = SendToWebSocket("Prop:" + json);
//         #endif
    }
} 