using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class GameController : MonoBehaviour
{
    private GridManager gridManager;
    private InputHandler inputHandler;
    private WinChecker winChecker;

    private const int WHITE = 1;
    private const int BLACK = -1;
    private int currentPlayer = WHITE;

    public GameObject whiteCube;
    public GameObject blackCube;
    public bool isCPUEnabled = false; // インスペクタで設定する
    public CubeAgent cpuAgent; // エージェントのインスタンス

    private const float cpuMoveDelay = 1.0f; // CPUの移動にかかる時間

    // Start is called before the first frame update
    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        inputHandler = new InputHandler();
        winChecker = new WinChecker();

        gridManager.InitializeArray();
        gridManager.InitializePoleMapping();

        inputHandler.OnPoleClicked += HandlePoleClick;
    }
    
    void Update()
    {
        inputHandler.Update();
    }

    private void HandlePoleClick(GameObject clickedPole, Vector2Int gridIndex)
    {
        if (currentPlayer == WHITE || !isCPUEnabled)
        {
            ProcessPlayerMove(clickedPole, gridIndex);
        }
    }

    private async void ProcessPlayerMove(GameObject clickedPole, Vector2Int gridIndex)
    {
        int height = gridManager.GetAvailableHeight(gridIndex.x, gridIndex.y);

        if (height != -1)
        {
            Vector3 polePosition = clickedPole.transform.position;
            GameObject cube = currentPlayer == WHITE ? whiteCube : blackCube;
            gridManager.PlaceCube(polePosition, gridIndex.x, height, gridIndex.y, currentPlayer, cube);

            if (winChecker.CheckWinCondition(gridManager.Grid, gridIndex.x, height, gridIndex.y, currentPlayer))
            {
                Debug.Log(currentPlayer == WHITE ? "白の勝ち" : "黒の勝ち");
                return;
            }

            currentPlayer = (currentPlayer == WHITE) ? BLACK : WHITE;

            if (isCPUEnabled && currentPlayer == BLACK)
            {
                await ProcessAgentMove();
                currentPlayer = WHITE; // プレイヤーに戻す
            }
        }
    }

    private async UniTask ProcessAgentMove()
    {
        await UniTask.Delay((int)(cpuMoveDelay * 1000)); // CPUの移動まで待機

        // エージェントのアクションを取得
        var action = cpuAgent.GetNextMove();
        int x = action.x;
        int z = action.z;

        int height = gridManager.GetAvailableHeight(x, z);
        if (height != -1)
        {
            Vector3 polePosition = gridManager.GetPolePosition(x, z);
            GameObject cube = blackCube; // CPUは黒いキューブを置く

            gridManager.PlaceCube(polePosition, x, height, z, BLACK, cube);

            if (winChecker.CheckWinCondition(gridManager.Grid, x, height, z, BLACK))
            {
                Debug.Log("黒の勝ち (CPU)");
            }

            currentPlayer = WHITE; // プレイヤーに戻す
        }
    }
}
