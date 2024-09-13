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
        camera_object = GameObject.Find("Main Camera").GetComponent<Camera>();
        InitializeArray();
        InitializePoleMapping();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = camera_object.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedPole = hit.collider.gameObject;

                if (poleToGridMap.TryGetValue(clickedPole, out Vector2Int gridIndex))
                {
                    for (int y = 0; y < 4; y++)
                    {
                        if (grid[gridIndex.x, y, gridIndex.y] == EMPTY)
                        {
                            Vector3 polePosition = clickedPole.transform.position;
                            PlaceCube(polePosition, clickedPole, gridIndex.x, y, gridIndex.y);

                            // 勝敗判定を追加
                            if (CheckWinCondition(gridIndex.x, y, gridIndex.y))
                            {
                                if (currentPlayer == BLACK)
                                {
                                    Debug.Log("白の勝ち");
                                }
                                else
                                {
                                    Debug.Log("黒の勝ち");
                                }
                            }

                            break;
                        }
                    }
                }
            }
        }
    }

    private void PlaceCube(Vector3 polePosition, GameObject clickedPole, int x, int y, int z)
    {
        GameObject cube;

        if (currentPlayer == WHITE)
        {
            grid[x, y, z] = WHITE;
            cube = Instantiate(whiteCube);
            currentPlayer = BLACK;
        }
        else
        {
            grid[x, y, z] = BLACK;
            cube = Instantiate(blackCube);
            currentPlayer = WHITE;
        }

        cube.transform.position = new Vector3(polePosition.x, y, polePosition.z);
        cube.transform.SetParent(clickedPole.transform);
    }

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

    private void InitializePoleMapping()
    {
        for (int x = 0; x < 4; x++)
        {
            for (int z = 0; z < 4; z++)
            {
                GameObject pole = GameObject.Find("pole_" + x + "_" + z);
                poleToGridMap[pole] = new Vector2Int(x, z);
            }
        }
    }

    // 勝敗判定のメソッド
    private bool CheckWinCondition(int x, int y, int z)
    {
        int player = grid[x, y, z]; // プレイヤーの値を取得

        // 各方向の勝利条件をチェック（逆方向の斜めも含めて）
        return CheckLine(x, y, z, 1, 0, 0, player) || // x方向
               CheckLine(x, y, z, 0, 1, 0, player) || // y方向
               CheckLine(x, y, z, 0, 0, 1, player) || // z方向
               CheckLine(x, y, z, 1, 1, 0, player) || // xy斜め
               CheckLine(x, y, z, 1, 0, 1, player) || // xz斜め
               CheckLine(x, y, z, 0, 1, 1, player) || // yz斜め
               CheckLine(x, y, z, -1, 1, 0, player) || // xy逆斜め
               CheckLine(x, y, z, -1, 0, 1, player) || // xz逆斜め
               CheckLine(x, y, z, 0, -1, 1, player) || // yz逆斜め
               CheckLine(x, y, z, 1, 1, 1, player) || // xyz斜め
               CheckLine(x, y, z, -1, 1, 1, player) || // xyz逆斜め
               CheckLine(x, y, z, 1, 1, -1, player) || // xyz逆斜め
               CheckLine(x, y, z, 1, -1, -1, player);  // xyz逆方向の逆斜め
    }


    // 指定した方向に連続したキューブが4つ揃っているかをチェック
    private bool CheckLine(int x, int y, int z, int dx, int dy, int dz, int player)
    {
        int count = 0;

        for (int i = -3; i <= 3; i++)
        {
            int nx = x + i * dx;
            int ny = y + i * dy;
            int nz = z + i * dz;

            if (nx >= 0 && nx < 4 && ny >= 0 && ny < 4 && nz >= 0 && nz < 4 && grid[nx, ny, nz] == player)
            {
                count++;
                if (count == 4) return true;
            }
            else
            {
                count = 0;
            }
        }

        return false;
    }
}
