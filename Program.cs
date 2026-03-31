using System.Net;
using System.Text;
using Bean;
using Bean.Debug;

class Program
{
    static void Main()
    {
// #if DEBUG
//         Thread serverThread = new Thread(DebugServer.StartWebServer);
//         serverThread.Start();
//
//         Thread socketThread = new Thread(DebugServer.StartWebSocketServer);
//         socketThread.Start();
//
//         if(OperatingSystem.IsWindows())
//         {
// 			System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
// 			{
// 				FileName = "http://localhost:8080/",
// 				UseShellExecute = true
// 			});
// 		}
//         else if (OperatingSystem.IsLinux())
//         {
//             System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
//             {
//                 FileName = "xdg-open",
//                 Arguments = "http://localhost:8080/",
//                 UseShellExecute = true
//             });
//         }
// #endif

        using var game = new DemoGame.Game();
        game.Run();
    }
}
