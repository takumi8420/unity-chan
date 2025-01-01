using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class UnityChanController : MonoBehaviour
{
    private Animator animator;

    // UDP関連
    private UdpClient udpClient;
    private IPEndPoint remoteEndPoint;
    private Thread receiveThread;
    private bool isRunning = false;

    // 受信データ
    private string latestReceivedData = null;
    private object dataLock = new object();

    // 左足だけ回転させる
    private Quaternion initialLeftUpperLegRotation;
    private Quaternion targetLeftUpperLegRotation;

    public float rotationSpeed = 5.0f;
    public int listenPort = 12345;

    // センサから回転への変換クラス
    private SensorToRotationConverter converter;

    void Start()
    {
        animator = GetComponent<Animator>();

        // 左足の初期回転を取得
        Transform leftLeg = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
        if (leftLeg != null)
        {
            initialLeftUpperLegRotation = leftLeg.localRotation;
            targetLeftUpperLegRotation = initialLeftUpperLegRotation; // 初期値を設定
        }
        else
        {
            Debug.LogError("[ERROR] LeftUpperLeg の Transform が見つかりません。");
        }

        // センサ→回転変換クラスの初期化
        converter = new SensorToRotationConverter();

        // UDPクライアントのセットアップ
        udpClient = new UdpClient(listenPort);
        remoteEndPoint = new IPEndPoint(IPAddress.Any, listenPort);

        Debug.Log($"[DEBUG] Listening on port {listenPort}");

        StartReceiving();
    }

    private void StartReceiving()
    {
        isRunning = true;
        receiveThread = new Thread(() =>
        {
            while (isRunning)
            {
                try
                {
                    byte[] data = udpClient.Receive(ref remoteEndPoint);
                    string receivedData = Encoding.UTF8.GetString(data);
                    lock (dataLock)
                    {
                        latestReceivedData = receivedData;
                    }
                    Debug.Log($"[DEBUG] データ受信: {receivedData}");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[ERROR] UDP受信エラー: {ex.Message}");
                }
            }
        });
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    void Update()
    {
        // 受信データを取り出す
        string dataToProcess = null;
        lock (dataLock)
        {
            if (!string.IsNullOrEmpty(latestReceivedData))
            {
                dataToProcess = latestReceivedData;
                latestReceivedData = null;
            }
        }

        // センサ値があればパースし、回転を更新
        if (!string.IsNullOrEmpty(dataToProcess))
        {
            string[] splits = dataToProcess.Split(',');
            if (splits.Length == 2)
            {
                if (float.TryParse(splits[0], out float sensor1) &&
                    float.TryParse(splits[1], out float sensor2))
                {
                    // 6点補間に基づきターゲット回転を取得
                    Quaternion mappedRotation = converter.GetTargetRotation(sensor1, sensor2);
                    Debug.Log($"[DEBUG] Mapped Rotation: {mappedRotation}");

                    // 左足のターゲット回転 = 初期回転 × (マッピング結果)
                    targetLeftUpperLegRotation = initialLeftUpperLegRotation * mappedRotation;
                }
                else
                {
                    Debug.LogWarning("[WARN] センサ値を float に変換できませんでした。");
                }
            }
            else
            {
                Debug.LogWarning("[WARN] 受信データのフォーマットが正しくありません。");
            }
        }

        // 左足にスムーズに反映
        SmoothRotateBone(HumanBodyBones.LeftUpperLeg, targetLeftUpperLegRotation);
    }

    // ボーンを滑らかに回転させる
    private void SmoothRotateBone(HumanBodyBones bone, Quaternion targetRotation)
    {
        Transform boneTransform = animator.GetBoneTransform(bone);
        if (boneTransform != null)
        {
            Quaternion current = boneTransform.localRotation;
            Quaternion newRotation = Quaternion.Lerp(current, targetRotation, rotationSpeed * Time.deltaTime);
            boneTransform.localRotation = newRotation;
        }
    }

    void OnApplicationQuit()
    {
        isRunning = false;
        if (udpClient != null)
        {
            udpClient.Close();
        }
        if (receiveThread != null && receiveThread.IsAlive)
        {
            receiveThread.Join();
        }
    }
}
