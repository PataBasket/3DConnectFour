using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private const int EMPTY = 0;
    public const int SIZE = 4;

    public static GridManager Instance { get; private set; }

    public int[,,] Grid { get; private set; } = new int[SIZE, SIZE, SIZE];
    private Dictionary<GameObject, Vector2Int> poleToGridMap = new Dictionary<GameObject, Vector2Int>();

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

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

    public int GetAvailableHeight(int x, int z)
    {
        for (int y = 0; y < SIZE; y++)
        {
            if (Grid[x, y, z] == EMPTY)
            {
                return y;
            }
        }
        return -1;
    }

    public void PlaceCube(Vector3 polePosition, int x, int y, int z, int player, GameObject cubePrefab)
    {
        Grid[x, y, z] = player;
        GameObject cube = Instantiate(cubePrefab);
        cube.transform.position = new Vector3(polePosition.x, y, polePosition.z);
        cube.transform.SetParent(GameObject.Find("pole_" + x + "_" + z).transform);
    }

    public Vector2Int GetGridIndex(GameObject pole)
    {
        return poleToGridMap[pole];
    }

    public Vector3 GetPolePosition(int x, int z)
    {
        GameObject pole = GameObject.Find("pole_" + x + "_" + z);
        return pole.transform.position;
    }

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
