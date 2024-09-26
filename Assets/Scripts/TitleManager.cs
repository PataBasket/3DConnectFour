using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private GameObject titleText;
    [SerializeField] private GameObject mainMenuButton;
    [SerializeField] private GameObject newGameButton;
    [SerializeField] private GameObject tutorialButton;
    [SerializeField] private GameObject baseObject;
    [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject rightArrow;
    [SerializeField] private GameObject backButton;
    [SerializeField] private List<GameObject> tutorialPages = new List<GameObject>();
    [SerializeField] private GameObject whiteCube;
    [SerializeField] private AudioSource a1;
    [SerializeField] private List<AudioClip> soundEffects = new List<AudioClip>();

    [SerializeField] 
    private Camera _mainCamera;
    
    // Tutorial中のCameraオブジェクトのTransform設定
    private Vector3 _newCameraPosition = new Vector3(0, 2.736161f, -7.517541f);
    private Vector3 _newCameraRotation = new Vector3(20, 0, 0);
    private Vector3 _newCameraScale = new Vector3(1, 1, 1);
    
    // Tutorial中のBaseオブジェクトのTransform設定
    private Vector3 _newBasePosition = new Vector3(0, 0, 0);
    private Vector3 _newBaseRotation = new Vector3(0, 0, 0);
    private Vector3 _newBaseScale = new Vector3(1, 1, 1);
    
    // Cameraのinitial transform設定
    private Vector3 _initialCameraPosition = new Vector3(-3.67f, 3.83f, -7.11f);
    private Vector3 _initialCameraRotation = new Vector3(20, 0, 0);
    private Vector3 _initialCameraScale = new Vector3(1, 1, 1);

    private TitleBaseController _titleBaseController;
    private InputHandler _inputHandler;
    private GridManager _gridManager;
    
    private const int WHITE = 1;
    private const int BLACK = -1;

    // Baseのドラッグ操作を可能にする変数
    private bool _isBaseActive = false;
    private int _tutorialIndex = 0;

    void Start()
    {
        // Tutorialボタンにクリックイベントを追加
        tutorialButton.GetComponent<Button>().onClick.AddListener(() => OnTutorialButtonClicked());
        backButton.GetComponent<Button>().onClick.AddListener(() => OnBackButtonClicked());
        newGameButton.GetComponent<Button>().onClick.AddListener(() => OnClickNewGame());
        
        _titleBaseController = gameObject.GetComponent<TitleBaseController>();
        _inputHandler = new InputHandler();
        _gridManager = GridManager.Instance;
        
        // チュートリアルUI要素を非表示に設定
        leftArrow.SetActive(false);
        rightArrow.SetActive(false);
        backButton.SetActive(false);
        
        _gridManager.InitializeArray();
        _gridManager.InitializePoleMapping();
        _inputHandler.OnPoleClicked += PlaceCubeObject;
    }

    private void Update()
    {
        if (_isBaseActive)
        {
            _inputHandler.Update();
        }
    }

    void OnTutorialButtonClicked()
    {
        SE1();
        _titleBaseController.StopAnimation();
        _tutorialIndex = 0;
        
        // 画面のタイトルやボタンをフェードアウトで非表示にする
        titleText.GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(() => titleText.SetActive(false));
        mainMenuButton.GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(() => mainMenuButton.SetActive(false));
        newGameButton.GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(() => newGameButton.SetActive(false));
        tutorialButton.GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(() => tutorialButton.SetActive(false));
        
        // BaseオブジェクトのTransformを変更しつつ移動と回転をスムーズに行う
        baseObject.transform.DOMove(_newBasePosition, 2f); 
        baseObject.transform.DORotate(_newBaseRotation, 2f);
        baseObject.transform.DOScale(_newBaseScale, 2f);

        // カメラオブジェクトのTransformを変更しつつ移動と回転をスムーズに行う
        _mainCamera.transform.DOMove(_newCameraPosition, 1f); 
        _mainCamera.transform.DORotate(_newCameraRotation, 1f);
        _mainCamera.transform.DOScale(_newCameraScale, 1f).OnComplete(() =>
        {
            // チュートリアルUI要素をフェードインで表示する
            rightArrow.SetActive(true);
            backButton.SetActive(true);
            rightArrow.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
            backButton.GetComponent<CanvasGroup>().DOFade(1, 0.5f).OnComplete(() => StartTutorial());
        });
    }

    private void StartTutorial()
    {
        // 1ページ目の処理
        tutorialPages[0].GetComponent<CanvasGroup>().DOFade(1, 0.5f);
        _mainCamera.GetComponent<CameraController>().enabled = true;
        _isBaseActive = true;
        _inputHandler.isClickable = false;
    }

    public void OnClickRightArrow()
    {
        SE2();
        if (_tutorialIndex == 0)
        {
            leftArrow.SetActive(true);
            leftArrow.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
        }
        tutorialPages[_tutorialIndex].GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(() =>
        {
            _tutorialIndex++;
            tutorialPages[_tutorialIndex].GetComponent<CanvasGroup>().DOFade(1, 1f);
            if (_tutorialIndex == 2)
            {
                rightArrow.GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(() => rightArrow.SetActive(false));
            }
            else if (_tutorialIndex == 1)
            {
                _inputHandler.isClickable = true;
            }
        });
    }
    
    public void OnClickLeftArrow()
    {
        SE2();
        if (_tutorialIndex == 2)
        {
            rightArrow.SetActive(true);
            rightArrow.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
        }
        tutorialPages[_tutorialIndex].GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(() =>
        {
            _tutorialIndex--;
            tutorialPages[_tutorialIndex].GetComponent<CanvasGroup>().DOFade(1, 1f);
            if (_tutorialIndex == 0)
            {
                leftArrow.GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(() => leftArrow.SetActive(false));
            }
            else if (_tutorialIndex == 1)
            {
                _inputHandler.isClickable = true;
            }
        });
    }
    
    void OnBackButtonClicked()
    {
        SE1();
        _mainCamera.GetComponent<CameraController>().enabled = false;
        _isBaseActive = false;
        
        // チュートリアルのテキストやボタンをフェードアウトで非表示にする
        tutorialPages[_tutorialIndex].GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(() =>
        {
            ClearCubes();
            leftArrow.GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(() => leftArrow.SetActive(false));
            rightArrow.GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(() => rightArrow.SetActive(false));
            backButton.GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(() =>
            {
                backButton.SetActive(false);
                
                _mainCamera.transform.DOMove(_initialCameraPosition, 1f); 
                _mainCamera.transform.DORotate(_initialCameraRotation, 1f);
                _mainCamera.transform.DOScale(_initialCameraScale, 1f).OnComplete(() => _titleBaseController.StartAnimation());

                titleText.SetActive(true);
                mainMenuButton.SetActive(true);
                newGameButton.SetActive(true);
                tutorialButton.SetActive(true);
                titleText.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
                mainMenuButton.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
                newGameButton.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
                tutorialButton.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
            });
        });
    }

    private void PlaceCubeObject(GameObject clickedPole, Vector2Int gridIndex)
    {
        SE3();
        int height = _gridManager.GetAvailableHeight(gridIndex.x, gridIndex.y);

        if (height != -1)
        {
            Vector3 polePosition = clickedPole.transform.position;
            GameObject cube = whiteCube;
            _gridManager.PlaceCube(polePosition, gridIndex.x, height, gridIndex.y, WHITE, cube);
        }
    }
    private void ClearCubes()
    {
        GameObject[] cubes = GameObject.FindGameObjectsWithTag("Cube");
        foreach (GameObject cube in cubes)
        {
            Destroy(cube);
        }
    }

    void OnClickNewGame()
    {
        SE1();
        DOVirtual.DelayedCall(0.5f, () => {
            SceneManager.LoadScene("Main_Standard");
        });
    }
    
    //自作の関数1
    private void SE1()
    {
        a1.PlayOneShot(soundEffects[0]);
    }

    //自作の関数2
    private void SE2()
    {
        a1.PlayOneShot(soundEffects[1]);
    }

    //自作の関数3
    private void SE3()
    {
        a1.PlayOneShot(soundEffects[2]); //a3にアタッチしたAudioSourceの設定値でb3にアタッチした効果音を再生
    }
}
