using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // 4x4x4のint型3次元配列を定義
    private int[,,] grid = new int[4, 4, 4];

    // WHITE=1, BLACK=-1 で定義
    private const int EMPTY = 0;
    private const int WHITE = 1;
    private const int BLACK = -1;

    // 現在のプレイヤー(初期プレイヤーは白)
    private int currentPlayer = WHITE;

    // カメラ情報
    private Camera camera_object;
    private RaycastHit hit;

    // Prefabs
    public GameObject whiteCube;
    public GameObject blackCube;

    // ポールオブジェクトとそのグリッドインデックスを紐づける辞書
    private Dictionary<GameObject, Vector2Int> poleToGridMap = new Dictionary<GameObject, Vector2Int>();

    // Start is called before the first frame update
    void Start()
    {
        // カメラ情報を取得
        camera_object = GameObject.Find("Main Camera").GetComponent<Camera>();

        // 配列を初期化
        InitializeArray();

        // ポールオブジェクトをグリッドにマッピング
        InitializePoleMapping();

        // デバッグ用メソッド
        DebugArray();
    }

    // Update is called once per frame
    void Update()
    {
        // マウスがクリックされたとき
        if (Input.GetMouseButtonUp(0))
        {
            // マウスのポジションを取得してRayに代入
            Ray ray = camera_object.ScreenPointToRay(Input.mousePosition);

            // マウスのポジションからRayを投げて何かに当たったらhitに入れる
            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedPole = hit.collider.gameObject;

                // クリックされたオブジェクトがポールか確認し、グリッド座標を取得
                if (poleToGridMap.TryGetValue(clickedPole, out Vector2Int gridIndex))
                {
                    // 最初の空のy値を探す
                    for (int y = 0; y < 4; y++)
                    {
                        if (grid[gridIndex.x, y, gridIndex.y] == EMPTY)
                        {
                            // ポールの位置を取得
                            Vector3 polePosition = clickedPole.transform.position;

                            // キューブを配置
                            PlaceCube(polePosition, clickedPole, gridIndex.x, y, gridIndex.y);
                            break;
                        }
                    }
                }
            }
        }
    }

    // キューブを配置する
    private void PlaceCube(Vector3 polePosition, GameObject clickedPole, int x, int y, int z)
    {
        GameObject cube;

        // 白のターンのとき
        if (currentPlayer == WHITE)
        {
            // キューブの値を更新
            grid[x, y, z] = WHITE;

            // 白のキューブを生成
            cube = Instantiate(whiteCube);
            currentPlayer = BLACK;
        }
        // 黒のターンのとき
        else
        {
            // キューブの値を更新
            grid[x, y, z] = BLACK;

            // 黒のキューブを生成
            cube = Instantiate(blackCube);
            currentPlayer = WHITE;
        }

        // キューブをポールの上に追従させる
        cube.transform.position = new Vector3(polePosition.x, y, polePosition.z);

        // キューブをポールの子オブジェクトに設定
        cube.transform.SetParent(clickedPole.transform);
    }

    // 配列情報を初期化する
    private void InitializeArray()
    {
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                for (int z = 0; z < 4; z++)
                {
                    grid[x, y, z] = EMPTY;
                }
            }
        }
    }

    // ポールオブジェクトをグリッドにマッピングする
    private void InitializePoleMapping()
    {
        // ここでポールオブジェクトを手動で設定し、座標をマッピング
        for (int x = 0; x < 4; x++)
        {
            for (int z = 0; z < 4; z++)
            {
                GameObject pole = GameObject.Find("pole_" + x + "_" + z);
                poleToGridMap[pole] = new Vector2Int(x, z);
            }
        }
    }

    // 配列情報を確認する（デバッグ用）
    public void DebugArray()
    {
        Debug.Log("Start Debugging the Array");
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                for (int z = 0; z < 4; z++)
                {
                    Debug.Log("(x, y, z) = (" + x + "," + y + "," + z + ") = " + grid[x, y, z]);
                }
            }
        }
    }
}
