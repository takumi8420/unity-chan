// using System.IO.Ports;
// using UnityEngine;

// public class SerialReader : MonoBehaviour
// {
//     public string portName = "COM3"; // Arduino が接続されているポート
//     public int baudRate = 9600; // ボーレート（Arduino と一致させる）
//     private SerialPort serialPort;
//     public float receivedAngle = 0.0f; // 受信した角度

//     void Start()
//     {
//         // シリアルポートを初期化
//         serialPort = new SerialPort(portName, baudRate);
//         try
//         {
//             serialPort.Open();
//         }
//         catch (System.Exception e)
//         {
//             Debug.LogError($"Failed to open serial port: {e.Message}");
//         }
//     }

//     void Update()
//     {
//         if (serialPort != null && serialPort.IsOpen)
//         {
//             try
//             {
//                 string data = serialPort.ReadLine(); // データを1行受信
//                 receivedAngle = float.Parse(data); // 受信したデータをfloatに変換
//             }
//             catch (System.Exception e)
//             {
//                 Debug.LogWarning($"Failed to read serial data: {e.Message}");
//             }
//         }
//     }

//     void OnDestroy()
//     {
//         // アプリ終了時にシリアルポートを閉じる
//         if (serialPort != null && serialPort.IsOpen)
//         {
//             serialPort.Close();
//         }
//     }
// }
