using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    // 回転に関する変数
    private Vector3 previousMousePosition;
    public GameObject baseObject; // 土台となるオブジェクト

    // 縦回転の制限範囲
    public float minVerticalAngle = 0f;
    public float maxVerticalAngle = 30f;

    // Update is called once per frame
    void Update()
    {
        // 土台の回転処理
        RotateBase();
    }

    // 土台をドラッグで回転させるメソッド
    private void RotateBase()
    {
        if (Input.GetMouseButtonDown(1)) // 右クリックを押した瞬間
        {
            previousMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(1)) // 右クリックを押し続けている間
        {
            Vector3 delta = Input.mousePosition - previousMousePosition;

            // x方向のマウス移動で上下回転、y方向で左右回転
            float absDeltaX = Mathf.Abs(delta.x);
            float absDeltaY = Mathf.Abs(delta.y);

            if (absDeltaX > absDeltaY)
            {
                // 横回転のみ（y方向）
                float rotationY = -delta.x * 0.2f;
                baseObject.transform.Rotate(Vector3.up, rotationY, Space.World);
            }
            else
            {
                // // 縦回転のみ（x方向）
                // float rotationX = delta.y * 0.2f;
                //
                // // 現在の回転角を取得
                // float currentXRotation = baseObject.transform.eulerAngles.x;
                // if (currentXRotation > 180) currentXRotation -= 360; // 角度を-180～180度に変換
                //
                // // 新しい回転角度を制限
                // float newXRotation = Mathf.Clamp(currentXRotation + rotationX, minVerticalAngle, maxVerticalAngle);
                //
                // // 回転を適用
                // baseObject.transform.eulerAngles = new Vector3(newXRotation, baseObject.transform.eulerAngles.y, baseObject.transform.eulerAngles.z);
            }

            previousMousePosition = Input.mousePosition;
        }
    }
}