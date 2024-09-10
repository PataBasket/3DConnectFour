using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    // 回転に関する変数
    private Vector3 previousMousePosition;
    public GameObject baseObject; // 土台となるオブジェクト
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

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
            // float rotationX = delta.y * 0.2f;
            float rotationY = -delta.x * 0.2f;

            // 土台を回転させる
            // baseObject.transform.Rotate(Vector3.right, rotationX, Space.World);
            baseObject.transform.Rotate(Vector3.up, rotationY, Space.World);

            previousMousePosition = Input.mousePosition;
        }
    }
}
