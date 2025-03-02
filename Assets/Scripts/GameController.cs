using UnityEngine;
using Cysharp.Threading.Tasks;
using Unity.MLAgents;
using DG.Tweening;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    private GridManager gridManager;
    private InputHandler inputHandler;
    private WinChecker winChecker;
    private UIManager _uiManager;

    private const int WHITE = 1;
    private const int BLACK = -1;
    private const int DANGER = -2;
    public int currentPlayer = WHITE; // プレイヤーがWHITE

    public GameObject whiteCube;
    public GameObject blackCube;
    public GameObject dangerCube;
    public CubeAgent cpuAgent; // エージェント（BLACK）

    private bool gameEnded = false; // ゲーム終了フラグ
    private GameObject _mainCanvas;
    private GameObject _mainCamera;
    private GameObject _baseObject;
    
    public static GameController Instance { get; private set; }

    public enum GameMode
    {
        Normal,
        DangerCase1,
        DangerCase2,
    }

    [SerializeField] private GameMode gameMode;
    
    // 【変更・追加箇所】 DangerCase2用：ハイライト中のキューブの元のマテリアルを保持するための辞書
    private Dictionary<GameObject, Material> originalMaterials = new Dictionary<GameObject, Material>();
    // 【変更】DangerCase2用: 赤色マテリアルへの参照をインスペクターから設定
    public Material redMaterial;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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
        
        _mainCanvas = GameObject.Find("Canvas");
        _uiManager = _mainCanvas.GetComponent<UIManager>();

        _mainCamera = GameObject.Find("Main Camera");
        _baseObject = GameObject.Find("Base");
    }

    void Update()
    {
        inputHandler.Update();
    }

    private void HandlePoleClick(GameObject clickedPole, Vector2Int gridIndex)
    {
        if (currentPlayer == WHITE)
        {
            _uiManager.SE3();
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

                inputHandler.isClickable = false;
                _mainCamera.GetComponent<CameraController>().enabled = false;
                _mainCamera.transform.DORotate(new Vector3(20, 0, 0), 0.5f);
                _mainCamera.transform.DOMove(new Vector3(0, 3.736161f, -7.517541f), 0.5f);
                
                _uiManager.ShowResult(0);
                // ResetGame();
                return;
            }
            
            // 【変更・追加箇所】 DangerCase2 の場合、ユーザーの配置によって相手のリーチが防がれた際はハイライト解除
            if (gameMode == GameMode.DangerCase2)
            {
                ClearDangerHighlight();
            }

            currentPlayer = BLACK;

            await ProcessAgentMove(cpuAgent);
        }
    }

    private async UniTask ProcessAgentMove(CubeAgent agent)
    {
        // 1秒間の待機
        await UniTask.Delay(1500);
        _uiManager.SE3();
        
        // Check if the opponent (WHITE) is about to win
        var (reachX, reachZ) = winChecker.FindOpponentReach(gridManager.Grid, WHITE);

        int x, z;

        if (reachX != -1 && reachZ != -1)
        {
            // Block the opponent's reach
            x = reachX;
            z = reachZ;
            Debug.Log("エージェントが相手のリーチを防ぎます");
        }
        else
        {
            // Normal agent decision
            agent.RequestDecision(); // エージェントに行動を要求
            await UniTask.WaitUntil(() => agent.HasAction);

            x = agent.SelectedActionX;
            z = agent.SelectedActionZ;
        }

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
                
                inputHandler.isClickable = false; 
                _mainCamera.GetComponent<CameraController>().enabled = false;
                _mainCamera.transform.DORotate(new Vector3(20, 0, 0), 0.5f);
                _mainCamera.transform.DOMove(new Vector3(0, 3.736161f, -7.517541f), 0.5f);
                
                _uiManager.ShowResult(1);
                // ResetGame();
                return;
            }

            // 引き分け判定
            if (gridManager.IsFull())
            {
                Debug.Log("引き分け");
                gameEnded = true;
                await UniTask.Delay(1000);
                
                inputHandler.isClickable = false;
                _mainCamera.GetComponent<CameraController>().enabled = false;
                _mainCamera.transform.DORotate(new Vector3(20, 0, 0), 0.5f);
                _mainCamera.transform.DOMove(new Vector3(0, 3.736161f, -7.517541f), 0.5f);
                
                _uiManager.ShowResult(2);
                // ResetGame();
                return;
            }

            agent.HasAction = false; // 行動フラグをリセット
            
            // Find Danger Position
            var (dangerX, dangerZ) = winChecker.FindOpponentReach(gridManager.Grid, BLACK);

            int danger_x, danger_z;
        
            if (dangerX != -1 && dangerZ != -1 && gameMode == GameMode.DangerCase1)
            {
                // Block the opponent's reach
                danger_x = dangerX;
                danger_z = dangerZ;
                Debug.Log("There is a danger position");

                int dangerHeight = gridManager.GetAvailableHeight(danger_x, danger_z);
                if (dangerHeight != -1)
                {
                    Vector3 danger_polePosition = gridManager.GetPolePosition(danger_x, danger_z);
                    GameObject danger_cube = dangerCube;

                    gridManager.PlaceCube(danger_polePosition, danger_x, dangerHeight, danger_z, DANGER, danger_cube);
                }
            }
            
            // 【変更・追加箇所】 DangerCase2 の場合、エージェントのリーチ状態を検出してキューブのマテリアルを赤色に変更する処理
            if (gameMode == GameMode.DangerCase2)
            {
                var reachPositions = winChecker.FindAgentReachCubePositions(gridManager.Grid, BLACK);
                if (reachPositions.Count > 0)
                {
                    Debug.Log("エージェントのリーチ状態が検出されました。キューブを赤色にハイライトします。");
                    foreach (var pos in reachPositions)
                    {
                        GameObject cubeObj = gridManager.GetCubeAt(pos.x, pos.y, pos.z);
                        if (cubeObj != null)
                        {
                            Renderer rend = cubeObj.GetComponent<Renderer>();
                            // 既にハイライトしていなければ元のマテリアルを保存
                            if (!originalMaterials.ContainsKey(cubeObj))
                            {
                                originalMaterials[cubeObj] = rend.material;
                            }
                            // 赤いマテリアルに切り替え
                            rend.material = redMaterial;
                        }
                    }
                }
            }
            
            currentPlayer = WHITE;
        }
        else
        {
            // 無効な行動の場合、再度エージェントに行動を要求
            agent.HasAction = false;
            await ProcessAgentMove(agent);
        }
    }
    
    // 【変更・追加箇所】 DangerCase2 のハイライトを解除するためのメソッド
    private void ClearDangerHighlight()
    {
        foreach (var kvp in originalMaterials)
        {
            if (kvp.Key != null)
            {
                Renderer rend = kvp.Key.GetComponent<Renderer>();
                rend.material = kvp.Value;
            }
        }
        originalMaterials.Clear();
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