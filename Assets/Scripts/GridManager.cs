using System.Collections.Generic;
using UnityEngine;

public class GridManager
{
    private const int EMPTY = 0;
    private const int SIZE = 4;

    // シングルトンのインスタンス
    public static GridManager Instance { get; private set; }

    public int[,,] Grid { get; private set; } = new int[SIZE, SIZE, SIZE];
    private Dictionary<GameObject, Vector2Int> poleToGridMap = new Dictionary<GameObject, Vector2Int>();

    public GridManager()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple instances of GridManager detected!");
        }
    }

    // グリッドを初期化
    public void InitializeArray()
    {
        for (int x = 0; x < SIZE; x++)
        {
            for (int y = 0; y < SIZE; y++)
            {
                for (int z = 0; z < SIZE; z++)
                {
                    Grid[x, y, z] = EMPTY;
                }
            }
        }
    }

    // ポールオブジェクトとグリッドインデックスを紐づけ
    public void InitializePoleMapping()
    {
        for (int x = 0; x < SIZE; x++)
        {
            for (int z = 0; z < SIZE; z++)
            {
                GameObject pole = GameObject.Find("pole_" + x + "_" + z);
                poleToGridMap[pole] = new Vector2Int(x, z);
            }
        }
    }

    // 空のグリッドの高さを取得
    public int GetAvailableHeight(int x, int z)
    {
        for (int y = 0; y < SIZE; y++)
        {
            if (Grid[x, y, z] == EMPTY)
            {
                return y;
            }
        }
        return -1; // グリッドが満杯の場合
    }

    // キューブをグリッドとシーンに配置
    public void PlaceCube(Vector3 polePosition, GameObject clickedPole, int x, int y, int z, int player, GameObject cubePrefab)
    {
        Grid[x, y, z] = player;
        GameObject cube = Object.Instantiate(cubePrefab);
        cube.transform.position = new Vector3(polePosition.x, y, polePosition.z);
        cube.transform.SetParent(clickedPole.transform);
    }

    // ポールに対応するグリッドインデックスを取得
    public Vector2Int GetGridIndex(GameObject pole)
    {
        return poleToGridMap[pole];
    }
}
