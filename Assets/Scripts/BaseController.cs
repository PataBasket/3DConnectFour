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
            
            float rotationY = -delta.x * 0.2f;
            baseObject.transform.Rotate(Vector3.up, rotationY, Space.World);

            previousMousePosition = Input.mousePosition;
        }
    }
}