using System.Collections;
using System.Collections.Generic;
using UIFramework;
using UnityEngine;

public class PauseMenu : UIBaseClass
{
    private void OnEnable()
    {
        LevelManager.GetInstance().SetGameplayEnum(GameplayEnum.Pause);
    }

    private void Start()
    {
        LevelManager.GetInstance().SetGameplayEnum(GameplayEnum.Pause);

        RigisterButtonOnClick("ResumeBtn", p =>
        {
            CloseUI("PauseMenu");
            FindObjectOfType<MusicPlayer>().PauseAudioPlaying();
            LevelManager.GetInstance().SetGameplayEnum(GameplayEnum.Playing);
        });

        RigisterButtonOnClick("PauseExitBtn", p =>
        {
            GameManager.GetInstance().LoadScene("TitleScene", p =>
            {
                GameManager.GetInstance().SetGameplayMode(GameplayModeEnum.Normal);
                GameManager.GetInstance().BluetoothDisConnect();
                CloseUI("PauseMenu");
                CloseUI("GameplayMenu");
                ShowUI("TitleMenu");
            });
        });
    }
}
