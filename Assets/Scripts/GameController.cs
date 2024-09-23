using UnityEngine;
using Cysharp.Threading.Tasks;
using Unity.MLAgents;

public class GameController : MonoBehaviour
{
    private GridManager gridManager;
    private InputHandler inputHandler;
    private WinChecker winChecker;

    private const int WHITE = 1;
    private const int BLACK = -1;
    public int currentPlayer = WHITE; // プレイヤーがWHITE

    public GameObject whiteCube;
    public GameObject blackCube;
    public CubeAgent cpuAgent; // エージェント（BLACK）

    private bool gameEnded = false; // ゲーム終了フラグ

    public static GameController Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // シーンが切り替わらない場合は不要
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        gridManager = GridManager.Instance;
        inputHandler = new InputHandler();
        winChecker = new WinChecker();

        cpuAgent.playerID = BLACK; // エージェントのプレイヤーIDを設定

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
        if (currentPlayer == WHITE)
        {
            ProcessPlayerMove(clickedPole, gridIndex).Forget();
        }
    }

    private async UniTaskVoid ProcessPlayerMove(GameObject clickedPole, Vector2Int gridIndex)
    {
        int height = gridManager.GetAvailableHeight(gridIndex.x, gridIndex.y);

        if (height != -1)
        {
            Vector3 polePosition = clickedPole.transform.position;
            GameObject cube = whiteCube;
            gridManager.PlaceCube(polePosition, gridIndex.x, height, gridIndex.y, currentPlayer, cube);

            if (winChecker.CheckWinCondition(gridManager.Grid, gridIndex.x, height, gridIndex.y, currentPlayer))
            {
                Debug.Log("プレイヤー（白）の勝ち");
                gameEnded = true;
                await UniTask.Delay(1000);
                ResetGame();
                return;
            }

            currentPlayer = BLACK;

            await ProcessAgentMove(cpuAgent);
        }
    }

    private async UniTask ProcessAgentMove(CubeAgent agent)
    {
        agent.RequestDecision(); // エージェントに行動を要求

        // エージェントが行動を決定するまで待機
        await UniTask.WaitUntil(() => agent.HasAction);

        int x = agent.SelectedActionX;
        int z = agent.SelectedActionZ;

        int height = gridManager.GetAvailableHeight(x, z);
        if (height != -1)
        {
            Vector3 polePosition = gridManager.GetPolePosition(x, z);
            GameObject cube = blackCube;

            gridManager.PlaceCube(polePosition, x, height, z, agent.playerID, cube);

            if (winChecker.CheckWinCondition(gridManager.Grid, x, height, z, agent.playerID))
            {
                Debug.Log("エージェント（黒）の勝ち");
                gameEnded = true;
                await UniTask.Delay(1000);
                ResetGame();
                return;
            }

            // 引き分け判定
            if (gridManager.IsFull())
            {
                Debug.Log("引き分け");
                gameEnded = true;
                await UniTask.Delay(1000);
                ResetGame();
                return;
            }

            agent.HasAction = false; // 行動フラグをリセット
            currentPlayer = WHITE;
        }
        else
        {
            // 無効な行動の場合、再度エージェントに行動を要求
            agent.HasAction = false;
            await ProcessAgentMove(agent);
        }
    }

    private void ResetGame()
    {
        gridManager.InitializeArray(); // ゲーム状態をリセット
        currentPlayer = WHITE; // プレイヤーから再開
        gameEnded = false;

        // キューブを削除
        ClearCubes();
    }

    private void ClearCubes()
    {
        GameObject[] cubes = GameObject.FindGameObjectsWithTag("Cube");
        foreach (GameObject cube in cubes)
        {
            Destroy(cube);
        }
    }
}
