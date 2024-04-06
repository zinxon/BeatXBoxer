using System.Collections;
using System.Collections.Generic;
using UIFramework;
using UnityEngine;

public class FinishMenu : UIBaseClass
{
    private CanvasGroup canvasGroup;
    private float loadingTime = 0;
    private bool isShowedTxt = false;

    private void OnEnable() {
        if(!canvasGroup)
            canvasGroup = GetComponent<CanvasGroup>();
        
        if(canvasGroup)
            canvasGroup.alpha = 0;
        loadingTime = 0;
        isShowedTxt = false;
    }

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        loadingTime = 0;
        isShowedTxt = false;
    }

    private void Update()
    {
        if (!isShowedTxt)
        {
            if (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += Time.deltaTime;
            }
            else
            {
                canvasGroup.alpha = 1;
                isShowedTxt = true;
            }
        } else {
            if(loadingTime < 1){
                loadingTime += Time.deltaTime;
            } else{
                ShowUI("ResultMenu");
                CloseUI("FinishMenu");
            }
        }
    }
}
