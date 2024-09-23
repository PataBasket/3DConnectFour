using UnityEngine;
using Cysharp.Threading.Tasks;
using Unity.MLAgents;

public class TrainingController : MonoBehaviour
{
    private GridManager gridManager;
    private InputHandler inputHandler;
    private WinChecker winChecker;

    private const int WHITE = 1;
    private const int BLACK = -1;
    public int currentPlayer = WHITE; // publicにしてエージェントから参照可能に

    public GameObject whiteCube;
    public GameObject blackCube;
    public bool isCPUEnabled = false; // CPUエージェントを有効化
    public bool isTraining = false; // トレーニングモードの切り替え
    public CubeAgent cpuAgent1; // エージェント1（WHITE）
    public CubeAgent cpuAgent2; // エージェント2（BLACK）

    private const float cpuMoveDelay = 1.0f;
    private bool gameEnded = false; // ゲーム終了フラグ

    public static TrainingController Instance { get; private set; }

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

    void Start()
    {
        gridManager = GridManager.Instance;
        inputHandler = new InputHandler();
        winChecker = new WinChecker();

        cpuAgent1.playerID = WHITE; // エージェント1のプレイヤーIDを設定
        cpuAgent2.playerID = BLACK; // エージェント2のプレイヤーIDを設定

        gridManager.InitializeArray();
        gridManager.InitializePoleMapping();

        inputHandler.OnPoleClicked += HandlePoleClick;

        if (isTraining)
        {
            StartTraining().Forget(); // 非同期でトレーニングを開始
        }
    }

    void Update()
    {
        if (!isTraining)
        {
            inputHandler.Update();
        }
    }

    private void HandlePoleClick(GameObject clickedPole, Vector2Int gridIndex)
    {
        if (!isTraining && (currentPlayer == WHITE || !isCPUEnabled))
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
            GameObject cube = currentPlayer == WHITE ? whiteCube : blackCube;
            gridManager.PlaceCube(polePosition, gridIndex.x, height, gridIndex.y, currentPlayer, cube);

            if (winChecker.CheckWinCondition(gridManager.Grid, gridIndex.x, height, gridIndex.y, currentPlayer))
            {
                Debug.Log(currentPlayer == WHITE ? "白の勝ち" : "黒の勝ち");
                gameEnded = true;
                await UniTask.Delay(1000); // 少し待機してからリセット
                ResetGame();
                return;
            }

            currentPlayer = (currentPlayer == WHITE) ? BLACK : WHITE;

            if (isCPUEnabled && currentPlayer == BLACK)
            {
                await ProcessAgentMove(cpuAgent2);
            }
        }
    }

    private async UniTask ProcessAgentMove(CubeAgent agent)
    {
        agent.RequestDecision();

        // エージェントが行動を決定するまで待機
        await UniTask.WaitUntil(() => agent.HasAction);

        int x = agent.SelectedActionX;
        int z = agent.SelectedActionZ;

        int height = gridManager.GetAvailableHeight(x, z);
        if (height != -1)
        {
            // 相手がリーチを持っているかを確認
            CubeAgent opponentAgent = (agent == cpuAgent1) ? cpuAgent2 : cpuAgent1;
            bool opponentHadReach = winChecker.HasPlayerReach(gridManager.Grid, opponentAgent.playerID);

            Vector3 polePosition = gridManager.GetPolePosition(x, z);
            GameObject cube = agent.playerID == WHITE ? whiteCube : blackCube;

            gridManager.PlaceCube(polePosition, x, height, z, agent.playerID, cube);

            // 勝利条件のチェック
            if (winChecker.CheckWinCondition(gridManager.Grid, x, height, z, agent.playerID))
            {
                Debug.Log(agent.playerID == WHITE ? "白の勝ち (CPU)" : "黒の勝ち (CPU)");

                // 勝利したエージェントに正の報酬を与え、エピソードを終了
                agent.SetReward(1.0f);
                agent.EndEpisode();

                // 敗北したエージェントに負の報酬を与え、エピソードを終了
                opponentAgent.SetReward(-1.0f);
                opponentAgent.EndEpisode();

                gameEnded = true;
                await UniTask.Delay(1000); // 少し待機してからリセット
                ResetGame();
                return;
            }

            // 引き分け判定
            if (gridManager.IsFull())
            {
                // 両方のエージェントに引き分けの報酬を与え、エピソードを終了
                agent.SetReward(0.0f);
                agent.EndEpisode();

                opponentAgent.SetReward(0.0f);
                opponentAgent.EndEpisode();

                gameEnded = true;
                await UniTask.Delay(1000); // 少し待機してからリセット
                ResetGame();
                return;
            }

            // === 自分のリーチに対する報酬を追加 ===

            // 自分がリーチを持っているかを確認
            bool agentHasReach = winChecker.HasPlayerReach(gridManager.Grid, agent.playerID);

            if (agentHasReach)
            {
                // 自分のリーチを作成した場合の報酬
                agent.AddReward(0.1f); // 報酬値は適宜調整
            }

            // 相手のリーチを阻止したかを確認
            if (opponentHadReach)
            {
                bool opponentHasReachNow = winChecker.HasPlayerReach(gridManager.Grid, opponentAgent.playerID);

                if (!opponentHasReachNow)
                {
                    // 相手のリーチを阻止した場合の報酬
                    agent.AddReward(0.2f); // 報酬値は適宜調整
                }
            }

            agent.HasAction = false; // 行動フラグをリセット
            // ターンを切り替え
            currentPlayer = (currentPlayer == WHITE) ? BLACK : WHITE;
        }
        else
        {
            // 無効な行動へのペナルティ
            agent.SetReward(-1.0f);
            agent.EndEpisode();

            // 相手エージェントに勝利の報酬を与える
            CubeAgent opponentAgent = (agent == cpuAgent1) ? cpuAgent2 : cpuAgent1;
            opponentAgent.SetReward(1.0f);
            opponentAgent.EndEpisode();

            gameEnded = true;
            await UniTask.Delay(1000); // 少し待機してからリセット
            ResetGame();
            return;
        }
    }




    private void ResetGame()
    {
        gridManager.InitializeArray(); // ゲーム状態をリセット
        currentPlayer = WHITE; // WHITEから再開
        gameEnded = false;

        // エージェントのエピソードを終了する必要はない

        // キューブを削除
        ClearCubes();
    }


    private void ClearCubes()
    {
        // シーン内のすべてのキューブを削除
        GameObject[] cubes = GameObject.FindGameObjectsWithTag("Cube");
        foreach (GameObject cube in cubes)
        {
            Destroy(cube);
        }
    }

    private async UniTask StartTraining()
    {
        while (true) // ループして継続的に学習
        {
            ResetGame();
            await UniTask.Delay(1000); // 新しいゲームを開始する前に少し待つ

            while (!gridManager.IsFull() && !gameEnded) // ゲーム終了まで続ける
            {
                if (currentPlayer == WHITE)
                {
                    await ProcessAgentMove(cpuAgent1);
                    if (gameEnded) break;
                    currentPlayer = BLACK;
                }
                else
                {
                    await ProcessAgentMove(cpuAgent2);
                    if (gameEnded) break;
                    currentPlayer = WHITE;
                }
            }
        }
    }
}
