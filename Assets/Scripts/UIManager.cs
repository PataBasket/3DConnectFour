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
    
    [SerializeField] private AudioSource a1;
    [SerializeField] private List<AudioClip> soundEffects = new List<AudioClip>();
    
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
        SE1();
        // 0.5秒待ってからシーンをロード
        DOVirtual.DelayedCall(0.5f, () => {
            SceneManager.LoadScene("StartScene");
        });
    }

    private void OnClickNewGame()
    {
        SE1();
        // 0.5秒待ってからシーンをロード
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
    public void SE3()
    {
        a1.PlayOneShot(soundEffects[2]); //a3にアタッチしたAudioSourceの設定値でb3にアタッチした効果音を再生
    }
}
