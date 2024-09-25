using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> resultTexts = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowResult(int resultIndex)
    {
        resultTexts[resultIndex].SetActive(true);
        resultTexts[resultIndex].GetComponent<CanvasGroup>().DOFade(1, 0.5f);
    }
}
