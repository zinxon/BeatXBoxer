using System.Collections;
using System.Collections.Generic;
using TMPro;
using UIFramework;
using UnityEngine;

public class ResultMenu : UIBaseClass
{
    private TextMeshProUGUI perfectValueTxt, greatValueTxt, missValueTxt, totalComboTxt, maxScoreTxt;

    private void OnEnable()
    {
        LevelManager levelMgr = LevelManager.GetInstance();

        if (perfectValueTxt) perfectValueTxt.text = levelMgr.PerfectValue.ToString();
        if (greatValueTxt) greatValueTxt.text = levelMgr.GreatBValue.ToString();
        if (missValueTxt) missValueTxt.text = levelMgr.MissValue.ToString();
        if (totalComboTxt) totalComboTxt.text = (levelMgr.PerfectValue + levelMgr.GreatBValue).ToString();
        if (maxScoreTxt) maxScoreTxt.text = levelMgr.ScoreValue.ToString();
    }

    private void Start()
    {
        perfectValueTxt = UnityHelper.FindChildNode(gameObject, "PerfectValueTxt").GetComponent<TextMeshProUGUI>();
        greatValueTxt = UnityHelper.FindChildNode(gameObject, "GreatValueTxt").GetComponent<TextMeshProUGUI>();
        missValueTxt = UnityHelper.FindChildNode(gameObject, "MissValueTxt").GetComponent<TextMeshProUGUI>();
        totalComboTxt = UnityHelper.FindChildNode(gameObject, "TotalComboTxt").GetComponent<TextMeshProUGUI>();
        maxScoreTxt = UnityHelper.FindChildNode(gameObject, "MaxScoreTxt").GetComponent<TextMeshProUGUI>();

        LevelManager levelMgr = LevelManager.GetInstance();

        perfectValueTxt.text = levelMgr.PerfectValue.ToString();
        greatValueTxt.text = levelMgr.GreatBValue.ToString();
        missValueTxt.text = levelMgr.MissValue.ToString();
        totalComboTxt.text = (levelMgr.PerfectValue + levelMgr.GreatBValue).ToString();
        maxScoreTxt.text = levelMgr.ScoreValue.ToString();

        RigisterButtonOnClick("ResultExitBtn", p =>
        {
            GameManager.GetInstance().LoadScene("TitleScene", p =>
            {
                GameManager.GetInstance().BluetoothDisConnect();
                GameManager.GetInstance().SetGameplayMode(GameplayModeEnum.Normal);
                CloseUI("ResultMenu");
                CloseUI("GameplayMenu");
                ShowUI("TitleMenu");
            });

        });

        RigisterButtonOnClick("RetryBtn", p =>
        {
            GameManager.GetInstance().LoadScene("GameplayScene");
            CloseUI("ResultMenu");
        });
    }
}
