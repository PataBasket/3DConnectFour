using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private const int EMPTY = 0;
    public const int SIZE = 4;
    public const int HEIGHT = 4; // 高さの定義を追加

    public static GridManager Instance { get; private set; }

    // グリッドの状態を保持する3次元配列
    public int[,,] Grid { get; private set; } = new int[SIZE, HEIGHT, SIZE];
    private Dictionary<GameObject, Vector2Int> poleToGridMap = new Dictionary<GameObject, Vector2Int>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // シーンが切り替わらない場合は不要
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // グリッドの初期化
    public void InitializeArray()
    {
        for (int x = 0; x < SIZE; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                for (int z = 0; z < SIZE; z++)
                {
                    Grid[x, y, z] = EMPTY;
                }
            }
        }
    }

    // ポールとグリッド座標のマッピングを初期化
    public void InitializePoleMapping()
    {
        for (int x = 0; x < SIZE; x++)
        {
            for (int z = 0; z < SIZE; z++)
            {
                GameObject pole = GameObject.Find("pole_" + x + "_" + z);
                if (pole != null)
                {
                    poleToGridMap[pole] = new Vector2Int(x, z);
                }
            }
        }
    }

    // 指定した位置の利用可能な高さを取得
    public int GetAvailableHeight(int x, int z)
    {
        for (int y = 0; y < HEIGHT; y++)
        {
            if (Grid[x, y, z] == EMPTY)
            {
                return y;
            }
        }
        return -1; // 高さが利用できない場合
    }

    // キューブを配置
    public void PlaceCube(Vector3 polePosition, int x, int y, int z, int player, GameObject cubePrefab)
    {
        Grid[x, y, z] = player;
        GameObject cube = Instantiate(cubePrefab);
        cube.transform.position = new Vector3(polePosition.x, y, polePosition.z);
        cube.transform.SetParent(GameObject.Find("pole_" + x + "_" + z).transform);
    }

    // ポールからグリッドインデックスを取得
    public Vector2Int GetGridIndex(GameObject pole)
    {
        return poleToGridMap[pole];
    }

    // グリッド座標からポールの位置を取得
    public Vector3 GetPolePosition(int x, int z)
    {
        GameObject pole = GameObject.Find("pole_" + x + "_" + z);
        return pole.transform.position;
    }

    // グリッドが満杯かどうかを確認
    public bool IsFull()
    {
        for (int x = 0; x < SIZE; x++)
        {
            for (int z = 0; z < SIZE; z++)
            {
                if (GetAvailableHeight(x, z) != -1)
                {
                    return false;
                }
            }
        }
        return true;
    }
}
