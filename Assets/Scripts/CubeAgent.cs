using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies; // この行を追加
using UnityEngine;
using UnityEngine.SceneManagement;

public class CubeAgent : Agent
{
    private GridManager gridManager;
    private WinChecker winChecker;

    public GameObject whiteCube;
    public GameObject blackCube;
    public int playerID; // Player ID (1 または -1)

    // エージェントの行動をコントローラーに渡すためのプロパティ
    public bool HasAction { get; set; } = false;
    public int SelectedActionX { get; set; }
    public int SelectedActionZ { get; set; }

    public override void Initialize()
    {
        gridManager = FindObjectOfType<GridManager>();
        winChecker = new WinChecker();

        // BehaviorParameters を取得して TeamId を設定
        var behaviorParams = GetComponent<BehaviorParameters>();
        if (behaviorParams != null)
        {
            behaviorParams.TeamId = (playerID == 1) ? 0 : 1;
        }
        else
        {
            Debug.LogError("BehaviorParameters component is missing on the Agent.");
        }
    }

    public override void OnEpisodeBegin()
    {
        // エージェント側では環境のリセットを行わない
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // グリッド全体の状態を観測 (-1: 相手, 0: 空, 1: 自分)
        int[,,] grid = gridManager.Grid;

        // オリジナルの盤面を観測
        AddGridObservations(sensor, grid);

        // 盤面の回転と反転によるデータ拡張
        int[,,] transformedGrid;

        // 90度回転
        transformedGrid = RotateGrid(grid, 90);
        AddGridObservations(sensor, transformedGrid);

        // 180度回転
        transformedGrid = RotateGrid(grid, 180);
        AddGridObservations(sensor, transformedGrid);

        // 270度回転
        transformedGrid = RotateGrid(grid, 270);
        AddGridObservations(sensor, transformedGrid);

        // 水平方向に反転
        transformedGrid = FlipGrid(grid);
        AddGridObservations(sensor, transformedGrid);

        // 現在のプレイヤーのターンかどうか
        if (SceneManager.GetActiveScene().name == "Main_Standard")
        {
            sensor.AddObservation((GameController.Instance.currentPlayer == playerID) ? 1 : 0);
        }
        else if (SceneManager.GetActiveScene().name == "TrainingScene_1")
        {
            sensor.AddObservation((TrainingController.Instance.currentPlayer == playerID) ? 1 : 0);
        }
    }

    private void AddGridObservations(VectorSensor sensor, int[,,] grid)
    {
        for (int x = 0; x < GridManager.SIZE; x++)
        {
            for (int y = 0; y < GridManager.HEIGHT; y++)
            {
                for (int z = 0; z < GridManager.SIZE; z++)
                {
                    int cellValue = grid[x, y, z];
                    // 自分から見た相対的な値に変換
                    if (cellValue == playerID)
                        sensor.AddObservation(1);
                    else if (cellValue == 0)
                        sensor.AddObservation(0);
                    else
                        sensor.AddObservation(-1);
                }
            }
        }
    }

    private int[,,] RotateGrid(int[,,] grid, int angle)
    {
        int[,,] newGrid = new int[GridManager.SIZE, GridManager.HEIGHT, GridManager.SIZE];

        for (int x = 0; x < GridManager.SIZE; x++)
        {
            for (int y = 0; y < GridManager.HEIGHT; y++)
            {
                for (int z = 0; z < GridManager.SIZE; z++)
                {
                    int newX = x;
                    int newZ = z;

                    switch (angle)
                    {
                        case 90:
                            newX = z;
                            newZ = GridManager.SIZE - 1 - x;
                            break;
                        case 180:
                            newX = GridManager.SIZE - 1 - x;
                            newZ = GridManager.SIZE - 1 - z;
                            break;
                        case 270:
                            newX = GridManager.SIZE - 1 - z;
                            newZ = x;
                            break;
                    }

                    newGrid[newX, y, newZ] = grid[x, y, z];
                }
            }
        }

        return newGrid;
    }

    private int[,,] FlipGrid(int[,,] grid)
    {
        int[,,] newGrid = new int[GridManager.SIZE, GridManager.HEIGHT, GridManager.SIZE];

        for (int x = 0; x < GridManager.SIZE; x++)
        {
            for (int y = 0; y < GridManager.HEIGHT; y++)
            {
                for (int z = 0; z < GridManager.SIZE; z++)
                {
                    int newX = GridManager.SIZE - 1 - x;
                    newGrid[newX, y, z] = grid[x, y, z];
                }
            }
        }

        return newGrid;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // 行動をコントローラーに渡す
        int action = actions.DiscreteActions[0];
        (int x, int z) = ActionIndexToCoordinates(action);
        SelectedActionX = x;
        SelectedActionZ = z;
        HasAction = true; // 行動が決定されたことを通知
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;
        int action;
        do
        {
            action = Random.Range(0, GridManager.SIZE * GridManager.SIZE);
            (int x, int z) = ActionIndexToCoordinates(action);
            if (gridManager.GetAvailableHeight(x, z) != -1)
            {
                break;
            }
        } while (true);

        discreteActions[0] = action;
    }

    private (int, int) ActionIndexToCoordinates(int actionIndex)
    {
        int x = actionIndex / GridManager.SIZE;
        int z = actionIndex % GridManager.SIZE;
        return (x, z);
    }

    private int CoordinatesToActionIndex(int x, int z)
    {
        return x * GridManager.SIZE + z;
    }
}
