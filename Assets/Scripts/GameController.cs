using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        int height = gridManager.GetAvailableHeight(gridIndex.x, gridIndex.y);

        if (height != -1)
        {
            Vector3 polePosition = clickedPole.transform.position;
            GameObject cube = currentPlayer == WHITE ? whiteCube : blackCube;
            gridManager.PlaceCube(polePosition, clickedPole, gridIndex.x, height, gridIndex.y, currentPlayer, cube);

            // 勝敗判定
            if (winChecker.CheckWinCondition(gridManager.Grid, gridIndex.x, height, gridIndex.y, currentPlayer))
            {
                Debug.Log(currentPlayer == WHITE ? "白の勝ち" : "黒の勝ち");
            }

            // プレイヤーの切り替え
            currentPlayer = (currentPlayer == WHITE) ? BLACK : WHITE;
        }
    }
}