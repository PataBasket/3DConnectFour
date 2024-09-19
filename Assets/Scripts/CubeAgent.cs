using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class CubeAgent : Agent
{
    private GridManager gridManager;
    private int playerID; // 白か黒のプレイヤーを判別
    private WinChecker winChecker;

    public GameObject whiteCube; // インスペクタで設定
    public GameObject blackCube; // インスペクタで設定

    public override void Initialize()
    {
        gridManager = FindObjectOfType<GridManager>();
        winChecker = new WinChecker();
        playerID = 1; // 白で初期化
    }

    public override void OnEpisodeBegin()
    {
        // グリッドとシーンのリセット
        gridManager.InitializeArray();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // グリッド状態を観測データとして収集（例えば、空の位置や配置されたキューブ）
        for (int x = 0; x < GridManager.SIZE; x++)
        {
            for (int z = 0; z < GridManager.SIZE; z++)
            {
                sensor.AddObservation(gridManager.GetAvailableHeight(x, z));
            }
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // 受け取ったアクションを基に、エージェントがキューブを配置
        int x = actions.DiscreteActions[0]; // x軸の選択
        int z = actions.DiscreteActions[1]; // z軸の選択

        int y = gridManager.GetAvailableHeight(x, z);
        if (y != -1)
        {
            Vector3 polePosition = gridManager.GetPolePosition(x, z);
            GameObject cube = playerID == 1 ? whiteCube : blackCube;
            gridManager.PlaceCube(polePosition, x, y, z, playerID, cube);

            if (winChecker.CheckWinCondition(gridManager.Grid, x, y, z, playerID))
            {
                SetReward(1.0f); // 勝利の報酬
                EndEpisode();
            }
            else if (gridManager.IsFull()) // 引き分け
            {
                SetReward(0.0f);
                EndEpisode();
            }
        }
        else
        {
            SetReward(-0.1f); // 無効な動き
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // 人間のプレイヤー用の操作（手動操作用、ここではランダムな動作を設定）
        var discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Random.Range(0, GridManager.SIZE); // x軸のランダムな選択
        discreteActions[1] = Random.Range(0, GridManager.SIZE); // z軸のランダムな選択
    }

    // GetNextMove メソッドを追加
    public (int x, int z) GetNextMove()
    {
        // ここでエージェントが選んだアクションを返す
        // 例としてランダムな動きを返す（実際にはモデルの出力に基づく）
        return (Random.Range(0, GridManager.SIZE), Random.Range(0, GridManager.SIZE));
    }
}
