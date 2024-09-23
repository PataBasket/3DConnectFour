using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject titleText;
    [SerializeField] private GameObject mainMenuButton;
    [SerializeField] private GameObject newGameButton;
    [SerializeField] private GameObject tutorialButton;
    [SerializeField] private GameObject baseObject;
    [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject rightArrow;
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject tutorial1;

    [SerializeField] 
    private Camera _mainCamera;

    // Tutorial中のCameraオブジェクトのTransform設定
    private Vector3 _newCamearaPosition = new Vector3(0, 2.736161f, -7.517541f);
    private Vector3 _newCamearaRotation = new Vector3(20, 0, 0);
    private Vector3 _newCamearaScale = new Vector3(1, 1, 1);
    
    // Tutorial中のBaseオブジェクトのTransform設定
    private Vector3 _newBasePosition = new Vector3(0, 0, 0);
    private Vector3 _newBaseRotation = new Vector3(0, 0, 0);
    private Vector3 _newBaseScale = new Vector3(1, 1, 1);

    private TitleBaseController _titleBaseController;

    void Start()
    {
        // Tutorialボタンにクリックイベントを追加
        tutorialButton.GetComponent<Button>().onClick.AddListener(() => OnTutorialButtonClicked());
        backButton.GetComponent<Button>().onClick.AddListener(() => OnBackButtonClicked());
        
        _titleBaseController = gameObject.GetComponent<TitleBaseController>();

        // チュートリアルUI要素を非表示に設定
        leftArrow.SetActive(false);
        rightArrow.SetActive(false);
        backButton.SetActive(false);
    }

    void OnTutorialButtonClicked()
    {
        _titleBaseController.StopAnimation();
        
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
        _mainCamera.transform.DOMove(_newCamearaPosition, 1f); 
        _mainCamera.transform.DORotate(_newCamearaRotation, 1f);
        _mainCamera.transform.DOScale(_newCamearaScale, 1f).OnComplete(() =>
        {
            // チュートリアルUI要素をフェードインで表示する
            leftArrow.SetActive(true);
            rightArrow.SetActive(true);
            backButton.SetActive(true);
            leftArrow.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
            rightArrow.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
            backButton.GetComponent<CanvasGroup>().DOFade(1, 0.5f).OnComplete(() =>
            {
                StartTutorial();
            }); ;
        });
    }

    void OnBackButtonClicked()
    {
        // main menuに戻る処理はここに追加
        
    }

    private void StartTutorial()
    {
        tutorial1.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
    }
}
