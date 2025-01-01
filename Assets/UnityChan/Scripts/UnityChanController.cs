using UnityEngine;

public class UnityChanController : MonoBehaviour
{
    private Animator animator;

    // 初期回転を保存する
    private Quaternion initialRightUpperArmRotation;
    private Quaternion initialLeftUpperArmRotation;
    private Quaternion initialRightUpperLegRotation;
    private Quaternion initialLeftUpperLegRotation;

    // ターゲットの回転
    private Quaternion targetRightUpperArmRotation;
    private Quaternion targetLeftUpperArmRotation;
    private Quaternion targetRightUpperLegRotation;
    private Quaternion targetLeftUpperLegRotation;

    // 補間速度
    public float rotationSpeed = 5.0f;

    void Start()
    {
        // Animatorコンポーネントを取得
        animator = GetComponent<Animator>();

        // 各ボーンの初期回転を保存
        initialRightUpperArmRotation = animator.GetBoneTransform(HumanBodyBones.RightUpperArm).localRotation;
        initialLeftUpperArmRotation = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm).localRotation;
        initialRightUpperLegRotation = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg).localRotation;
        initialLeftUpperLegRotation = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg).localRotation;

        // 初期のターゲット回転を設定
        targetRightUpperArmRotation = initialRightUpperArmRotation;
        targetLeftUpperArmRotation = initialLeftUpperArmRotation;
        targetRightUpperLegRotation = initialRightUpperLegRotation;
        targetLeftUpperLegRotation = initialLeftUpperLegRotation;
    }

    void Update()
    {
        // 腕の操作
        if (Input.GetKey(KeyCode.W))
        {
            targetRightUpperArmRotation = initialRightUpperArmRotation * Quaternion.Euler(45, 0, 0);
            targetLeftUpperArmRotation = initialLeftUpperArmRotation * Quaternion.Euler(45, 0, 0);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            targetRightUpperArmRotation = initialRightUpperArmRotation * Quaternion.Euler(0, 45, 0);
            targetLeftUpperArmRotation = initialLeftUpperArmRotation * Quaternion.Euler(0, -45, 0);
        }
        else
        {
            targetRightUpperArmRotation = initialRightUpperArmRotation;
            targetLeftUpperArmRotation = initialLeftUpperArmRotation;
        }

        // 足の操作
        if (Input.GetKey(KeyCode.S))
        {
            targetRightUpperLegRotation = initialRightUpperLegRotation * Quaternion.Euler(-45, 0, 0);
            targetLeftUpperLegRotation = initialLeftUpperLegRotation * Quaternion.Euler(-45, 0, 0);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            targetRightUpperLegRotation = initialRightUpperLegRotation * Quaternion.Euler(0, 45, 0);
            targetLeftUpperLegRotation = initialLeftUpperLegRotation * Quaternion.Euler(0, -45, 0);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            targetRightUpperLegRotation = initialRightUpperLegRotation * Quaternion.Euler(0, 0, 45);
            targetLeftUpperLegRotation = initialLeftUpperLegRotation * Quaternion.Euler(0, 0, 45);
        }
        else
        {
            targetRightUpperLegRotation = initialRightUpperLegRotation;
            targetLeftUpperLegRotation = initialLeftUpperLegRotation;
        }

        // 回転を適用
        SmoothRotateBone(HumanBodyBones.RightUpperArm, targetRightUpperArmRotation);
        SmoothRotateBone(HumanBodyBones.LeftUpperArm, targetLeftUpperArmRotation);
        SmoothRotateBone(HumanBodyBones.RightUpperLeg, targetRightUpperLegRotation);
        SmoothRotateBone(HumanBodyBones.LeftUpperLeg, targetLeftUpperLegRotation);
    }

    // ボーンを滑らかに回転させる
    void SmoothRotateBone(HumanBodyBones bone, Quaternion targetRotation)
    {
        Transform boneTransform = animator.GetBoneTransform(bone);
        if (boneTransform != null)
        {
            // 現在の回転から目標回転へ補間
            boneTransform.localRotation = Quaternion.Lerp(boneTransform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
