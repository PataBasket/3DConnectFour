using System.Collections;
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

    private const float cpuMoveDelay = 1.0f; // CPUの移動にかかる時間

    // Start is called before the first frame update
    void Start()
    {
        gridManager = new GridManager();
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
            gridManager.PlaceCube(polePosition, clickedPole, gridIndex.x, height, gridIndex.y, currentPlayer, cube);

            if (winChecker.CheckWinCondition(gridManager.Grid, gridIndex.x, height, gridIndex.y, currentPlayer))
            {
                Debug.Log(currentPlayer == WHITE ? "白の勝ち" : "黒の勝ち");
                return;
            }

            currentPlayer = (currentPlayer == WHITE) ? BLACK : WHITE;

            if (isCPUEnabled && currentPlayer == BLACK)
            {
                await PerformCPUMove();
                currentPlayer = WHITE; // プレイヤーに戻す
            }
        }
    }

    private async UniTask PerformCPUMove()
    {
        await UniTask.Delay((int)(cpuMoveDelay * 1000)); // CPUの移動まで待機

        List<Vector2Int> availablePositions = new List<Vector2Int>();
        for (int x = 0; x < GridManager.SIZE; x++)
        {
            for (int z = 0; z < GridManager.SIZE; z++)
            {
                if (gridManager.GetAvailableHeight(x, z) != -1)
                {
                    availablePositions.Add(new Vector2Int(x, z));
                }
            }
        }

        if (availablePositions.Count > 0)
        {
            Vector2Int randomPosition = availablePositions[Random.Range(0, availablePositions.Count)];
            GameObject randomPole = GameObject.Find("pole_" + randomPosition.x + "_" + randomPosition.y);
            int height = gridManager.GetAvailableHeight(randomPosition.x, randomPosition.y);
            Vector3 polePosition = randomPole.transform.position;
            GameObject cube = blackCube; // CPUは黒いキューブを置く

            gridManager.PlaceCube(polePosition, randomPole, randomPosition.x, height, randomPosition.y, BLACK, cube);

            if (winChecker.CheckWinCondition(gridManager.Grid, randomPosition.x, height, randomPosition.y, BLACK))
            {
                Debug.Log("黒の勝ち (CPU)");
            }

            currentPlayer = WHITE; // プレイヤーに戻す
        }
    }
}
