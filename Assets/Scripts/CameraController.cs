using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;  // カメラが回転する中心のオブジェクト
    public float rotationSpeed = 10f;  // 回転の速さ
    public float heightOffset = 5f;  // ターゲットよりカメラを高い位置に配置するオフセット
    public float distanceFromTarget = 10f;  // ターゲットからの距離
    private float verticalRotation = 0f;  // 縦方向の回転角度

    void Start()
    {
        // カメラの初期位置を設定
        verticalRotation = Mathf.Clamp(verticalRotation, 20f, 80f);
    }

    void Update()
    {
        // マウスのドラッグ操作の入力を取得
        if (Input.GetMouseButton(1)) // 左クリックを押しているとき
        {
            float mouseY = Input.GetAxis("Mouse Y");  // マウスのY軸の動き

            // 縦方向の回転を更新
            verticalRotation -= mouseY * rotationSpeed;
            verticalRotation = Mathf.Clamp(verticalRotation, 20f, 80f);  // 上下80度まで制限

            // ターゲットからの距離とオフセットを含む新しいカメラの位置を計算
            Vector3 offsetPosition = new Vector3(0, 0, -distanceFromTarget); // Z軸のみ距離を設定
            Quaternion rotation = Quaternion.Euler(verticalRotation, 0, 0); // 縦方向の回転のみを適用
            Vector3 newPosition = target.position + rotation * offsetPosition; // 回転を適用した新しい位置を計算

            // 高さのオフセットを適用
            newPosition.y += heightOffset;

            // カメラ位置を更新
            transform.position = newPosition;

            // カメラをターゲットに向ける。ただし、upベクトルは明示的にVector3.upを指定して反転を防止
            transform.LookAt(target.position, Vector3.up);
        }
    }
}