using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using TMPro;
using UIFramework;
using UnityEngine;

public class GameplayMenu : UIBaseClass
{
    private TextMeshProUGUI stateTxt;
    private CanvasGroup stateCanvasGroup;

    private void Start() {
        stateTxt = UnityHelper.FindChildNode(gameObject, "StateTxt").GetComponent<TextMeshProUGUI>();
        stateCanvasGroup = UnityHelper.FindChildNode(gameObject, "State").GetComponent<CanvasGroup>();

        RigisterButtonOnClick("PauseButton", p => {
            ShowUI("PauseMenu");
            FindObjectOfType<MusicPlayer>().PauseAudioPlaying();
        });

        RigisterButtonOnClick("LeftBtn", p =>{
            LevelManager.GetInstance().SendMessageToNoteLanesController("1");
            LevelManager.GetInstance().SendMessageToNoteLanesController("3");
        });

        RigisterButtonOnClick("RightBtn", p =>{
            LevelManager.GetInstance().SendMessageToNoteLanesController("2");
            LevelManager.GetInstance().SendMessageToNoteLanesController("4");
        });
    }


    private void Update()
    {
        if(GameManager.GetInstance().gameplayModeEnum == GameplayModeEnum.Normal){
            if(stateCanvasGroup.alpha == 1){
                stateCanvasGroup.alpha = 0;
            }
        }
        
        if(GameManager.GetInstance().gameplayModeEnum == GameplayModeEnum.Bluetooth){
            if(stateCanvasGroup.alpha == 0){
                stateCanvasGroup.alpha = 1;
            }

            if(GameManager.GetInstance().HasBluetoothConnect){
                stateTxt.text = "State: Connect";
            } else{
                stateTxt.text = "State: Disconnect";
            }
        }

        if (LevelManager.GetInstance().gameplayEnum == GameplayEnum.Result && LevelManager.GetInstance().isResult == false)
        {
            ShowUI("FinishMenu");
            LevelManager.GetInstance().isResult = true;
        }
    }

    public void LoadScene(string sceneName)
    {
        GameManager.GetInstance().LoadScene(sceneName);
    }
}
