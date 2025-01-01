using UnityEngine;
using System.Collections.Generic;

public class SensorToRotationConverter
{
    // 実際に測定した 6 点のセンサ値
    private Vector2[] sensorValues =
    {
        new Vector2(220, 114), // 回転: Quaternion.Euler(0, 0,  30)
        new Vector2(145, 165), // 回転: Quaternion.Euler(0, 0,   0)
        new Vector2(86, 165),  // 回転: Quaternion.Euler(0, 0, -45)
        new Vector2(237, 90),  // 回転: Quaternion.Euler(0, -45, 30)
        new Vector2(188, 92),  // 回転: Quaternion.Euler(0, -45,  0)
        new Vector2(97, 120)   // 回転: Quaternion.Euler(0, -45, 45)
    };

    // 上記 6 点に対応するクォータニオン
    private Quaternion[] targetRotations =
    {
        Quaternion.Euler(0, 0,   30),
        Quaternion.Euler(0, 0,    0),
        Quaternion.Euler(0, 0,  -45),
        Quaternion.Euler(0, -45, 30),
        Quaternion.Euler(0, -45,  0),
        Quaternion.Euler(0, -45, 45)
    };

    /// <summary>
    /// 与えられた (sensor1, sensor2) に対し、6 点のうち
    /// 「距離が近い 2 点」を探して補間した回転を返します。
    /// </summary>
    public Quaternion GetTargetRotation(float sensor1, float sensor2)
    {
        // 1) 各センサ値との距離を計算
        List<(int index, float dist)> distances = new List<(int, float)>();
        for (int i = 0; i < sensorValues.Length; i++)
        {
            float dx = sensor1 - sensorValues[i].x;
            float dy = sensor2 - sensorValues[i].y;
            float distSqr = dx * dx + dy * dy; // 距離^2
            distances.Add((i, distSqr));
        }

        // 2) 距離が小さい順にソート
        distances.Sort((a, b) => a.dist.CompareTo(b.dist));

        // 3) 最も近い点 (idx0) と、次に近い点 (idx1) を取得
        var idx0 = distances[0].index; // 1番目に近い
        var idx1 = distances[1].index; // 2番目に近い
        float d0 = distances[0].dist;  // 1番目に近い距離^2
        float d1 = distances[1].dist;  // 2番目に近い距離^2

        // 安全策: もし d0 + d1 == 0 に近い場合(同じ点の上など)は単一の点でOK
        if (Mathf.Approximately(d0 + d1, 0f))
        {
            return targetRotations[idx0];
        }

        // 4) 逆距離を重みとして補間 (2点)
        //    - dist が小さいほど重みを大きくする (逆距離加重)
        //    - distSqr を使っていますが、実際の距離にしたい場合は Mathf.Sqrt(d0), d1 を取ってもOKです
        float w0 = 1f / (1f + d0);
        float w1 = 1f / (1f + d1);
        float wSum = w0 + w1;

        Quaternion q0 = targetRotations[idx0];
        Quaternion q1 = targetRotations[idx1];

        // 各クォータニオンに対する正規化した重み (0～1)
        float t = w1 / wSum; 
        // または w0/(w0+w1), w1/(w0+w1) で2段階補間でも可

        // 球面線形補間 (Slerp) で 2点補間
        // Lerp でも構いませんが、回転の補間には Slerp が自然です
        Quaternion result = Quaternion.Slerp(q0, q1, t);

        return result;
    }
}
