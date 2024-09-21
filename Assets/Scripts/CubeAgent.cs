using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class CubeAgent : Agent
{
    private GridManager gridManager;
    private WinChecker winChecker;

    public GameObject whiteCube;
    public GameObject blackCube;
    public int playerID; // Player ID (1 or -1)

    // エージェントの行動をGameControllerに渡すためのプロパティ
    public bool HasAction { get; set; } = false;
    public int SelectedActionX { get; set; }
    public int SelectedActionZ { get; set; }

    public override void Initialize()
    {
        gridManager = FindObjectOfType<GridManager>();
        winChecker = new WinChecker();
    }

    public override void OnEpisodeBegin()
    {
        // エージェント側では環境のリセットを行わない
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // グリッド全体の状態を観測 (-1: 相手, 0: 空, 1: 自分)
        for (int x = 0; x < GridManager.SIZE; x++)
        {
            for (int y = 0; y < GridManager.HEIGHT; y++)
            {
                for (int z = 0; z < GridManager.SIZE; z++)
                {
                    int cellValue = gridManager.Grid[x, y, z];
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

        // 現在のプレイヤーのターンかどうか
        sensor.AddObservation((GameController.Instance.currentPlayer == playerID) ? 1 : 0);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // 行動をGameControllerに渡す
        SelectedActionX = actions.DiscreteActions[0];
        SelectedActionZ = actions.DiscreteActions[1];
        HasAction = true; // 行動が決定されたことを通知
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // ランダムな行動を選択
        var discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Random.Range(0, GridManager.SIZE);
        discreteActions[1] = Random.Range(0, GridManager.SIZE);
    }
}
