using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> resultTexts = new List<GameObject>();
    [SerializeField] private Button menuButton;
    [SerializeField] private Button newGameButton;
    
    // Start is called before the first frame update
    void Start()
    {
        menuButton.GetComponent<Button>().onClick.AddListener(() => OnClickMenu());
        newGameButton.GetComponent<Button>().onClick.AddListener(() => OnClickNewGame());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowResult(int resultIndex)
    {
        resultTexts[resultIndex].SetActive(true);
        menuButton.gameObject.SetActive(true);
        newGameButton.gameObject.SetActive(true);
        
        resultTexts[resultIndex].GetComponent<CanvasGroup>().DOFade(1, 1);
        menuButton.GetComponent<CanvasGroup>().DOFade(1, 1);
        newGameButton.GetComponent<CanvasGroup>().DOFade(1, 1);
    }

    private void OnClickMenu()
    {
        SceneManager.LoadScene("StartScene");
    }

    private void OnClickNewGame()
    {
        SceneManager.LoadScene("Main_Standard");
    }
}
