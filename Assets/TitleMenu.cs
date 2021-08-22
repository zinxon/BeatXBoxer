using System.Collections;
using System.Collections.Generic;
using TMPro;
using UIFramework;
using UnityEngine;
using UnityEngine.UI;
public class TitleMenu : UIBaseClass
{
    private Button startBtn, exitBtn, clickBtn;

    private Animator startBtnGroupAnim, modeBtnGroupAnim, blueBtnGroupAnim;
    private Button normalBtn, blueBtn, modeReturnBtn;
    private Button connectBtn, disconnectBtn, blueStartBtn, blueReturnBtn;
    private TextMeshProUGUI stateTxt;

    private void OnEnable()
    {
        if (startBtnGroupAnim)
            startBtnGroupAnim.Play("Pressed");
    }

    private void Start()
    {
        startBtn = UnityHelper.FindChildNode(gameObject, "StartBtn").GetComponent<Button>();
        exitBtn = UnityHelper.FindChildNode(gameObject, "ExitBtn").GetComponent<Button>();
        clickBtn = UnityHelper.FindChildNode(gameObject, "ClickBtn").GetComponent<Button>();

        startBtnGroupAnim = GameObject.Find("StartButtonGroup").GetComponent<Animator>();

        RigisterButtonOnClick("StartBtn", p =>
        {
            OpenButtonGroupAnim(startBtnGroupAnim, modeBtnGroupAnim);
        });
        RigisterButtonOnClick("ExitBtn", p => Application.Quit());
        RigisterButtonOnClick("ClickBtn", p => OpenButtonGroupAnim(clickBtn.gameObject, startBtnGroupAnim));

        modeBtnGroupAnim = GameObject.Find("ModeButtonGroup").GetComponent<Animator>();

        normalBtn = UnityHelper.FindChildNode(gameObject, "NormalModeBtn").GetComponent<Button>();
        blueBtn = UnityHelper.FindChildNode(gameObject, "BluetoothModeBtn").GetComponent<Button>();
        modeReturnBtn = UnityHelper.FindChildNode(gameObject, "ModeReturnBtn").GetComponent<Button>();

        RigisterButtonOnClick("NormalModeBtn", p =>
        {
            GameManager.GetInstance().SetGameplayMode(GameplayModeEnum.Normal);
            GameManager.GetInstance().LoadScene("GameplayScene", p => ShowUI("GameplayMenu"));
            CloseUI("TitleMenu");
        });
        RigisterButtonOnClick("BluetoothModeBtn", p =>
        {
            OpenButtonGroupAnim(modeBtnGroupAnim, blueBtnGroupAnim);
        });
        RigisterButtonOnClick("ModeReturnBtn", p =>
        {
            OpenButtonGroupAnim(modeBtnGroupAnim, startBtnGroupAnim);
        });

        blueBtnGroupAnim = GameObject.Find("BluetoothButtonGroup").GetComponent<Animator>();

        connectBtn = UnityHelper.FindChildNode(gameObject, "ConnectBtn").GetComponent<Button>();
        disconnectBtn = UnityHelper.FindChildNode(gameObject, "DisconnectBtn").GetComponent<Button>();
        blueStartBtn = UnityHelper.FindChildNode(gameObject, "BlueStartBtn").GetComponent<Button>();
        blueReturnBtn = UnityHelper.FindChildNode(gameObject, "BlueReturnBtn").GetComponent<Button>();

        stateTxt = UnityHelper.FindChildNode(gameObject, "StateTxt").GetComponent<TextMeshProUGUI>();

        RigisterButtonOnClick("ConnectBtn", p =>
        {
            GameManager.GetInstance().BluetoothConnect();
        });
        RigisterButtonOnClick("DisconnectBtn", p =>
        {
            GameManager.GetInstance().BluetoothDisConnect();
        });
        RigisterButtonOnClick("BlueStartBtn", p =>
        {
            GameManager.GetInstance().SetGameplayMode(GameplayModeEnum.Bluetooth);
            GameManager.GetInstance().LoadScene("GameplayScene", p => ShowUI("GameplayMenu"));
            CloseUI("TitleMenu");
        });
        RigisterButtonOnClick("BlueReturnBtn", p =>
        {
            GameManager.GetInstance().BluetoothDisConnect();
            OpenButtonGroupAnim(blueBtnGroupAnim, modeBtnGroupAnim);
        });
    }

    private void Update()
    {
        if (GameManager.GetInstance().HasBluetoothConnect)
        {
            if (connectBtn.gameObject.activeInHierarchy)
            {
                connectBtn.gameObject.SetActive(false);
                stateTxt.text = "State: Connect";
            }

            if (!disconnectBtn.gameObject.activeInHierarchy)
                disconnectBtn.gameObject.SetActive(true);

            if (!blueStartBtn.gameObject.activeInHierarchy)
                blueStartBtn.gameObject.SetActive(true);
        }
        else
        {
            if (!connectBtn.gameObject.activeInHierarchy)
            {
                connectBtn.gameObject.SetActive(true);
                stateTxt.text = "State: Disconnect";
            }

            if (disconnectBtn.gameObject.activeInHierarchy)
                disconnectBtn.gameObject.SetActive(false);

            if (blueStartBtn.gameObject.activeInHierarchy)
                blueStartBtn.gameObject.SetActive(false);
        }
    }

    private void OpenButtonGroupAnim(GameObject gameObject, Animator anim)
    {
        gameObject.SetActive(false);
        if (anim)
            anim.Play("DissolveToPressed");
    }

    private void OpenButtonGroupAnim(Animator anim1, Animator anim2)
    {
        if (anim1)
            anim1.Play("Dissolve");
        if (anim2)
            anim2.Play("DissolveToPressed");
    }
}
