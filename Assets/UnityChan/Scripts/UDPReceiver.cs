// using UnityEngine;
// using System.Text;
// using System.Net;
// using System.Net.Sockets;
// using System.Threading;

// public class UDPReceiver : MonoBehaviour
// {
//     private UdpClient udpClient;
//     private IPEndPoint remoteEndPoint;
//     private Thread receiveThread;
//     private bool isRunning;

//     void Start()
//     {
//         int listenPort = 12345;
//         udpClient = new UdpClient(listenPort);
//         remoteEndPoint = new IPEndPoint(IPAddress.Any, listenPort);
//         isRunning = true;

//         Debug.Log($"UDPReceiver: Listening on port {listenPort}");

//         // スレッドで受信開始
//         receiveThread = new Thread(ReceiveData);
//         receiveThread.IsBackground = true;
//         receiveThread.Start();
//     }

//     void ReceiveData()
//     {
//         while (isRunning)
//         {
//             try
//             {
//                 byte[] data = udpClient.Receive(ref remoteEndPoint);
//                 string text = Encoding.UTF8.GetString(data);

//                 // 受信データをコンソールに表示
//                 Debug.Log($"[UDP] 受信データ: {text}");
//             }
//             catch (System.Exception ex)
//             {
//                 Debug.LogError($"受信エラー: {ex.Message}");
//             }
//         }
//     }

//     void OnApplicationQuit()
//     {
//         isRunning = false;
//         udpClient.Close();
//         receiveThread.Abort();
//     }
// }
