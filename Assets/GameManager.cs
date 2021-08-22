using System;
using System.Collections;
using System.Collections.Generic;
using UIFramework;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private CanvasGroup uiMask;

    public bool HasBluetoothConnect { get; private set; } = false;
    public GameplayModeEnum gameplayModeEnum{get; private set;} = GameplayModeEnum.Normal;
    private manager bluetoothManager;

    public static GameManager GetInstance()
    {
        if (instance == null)
            instance = new GameObject("_GameManager").AddComponent<GameManager>();

        return instance;
    }

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitGameManager();

        uiMask = GameObject.Find("UIMask").GetComponent<CanvasGroup>();

        bluetoothManager = GetComponent<manager>();
    }

    private void InitGameManager()
    {
        if (SceneManager.GetActiveScene().name == "TitleScene")
        {
            UIManager.GetInstance().ShowUI("TitleMenu");

        }

        if (SceneManager.GetActiveScene().name == "GameplayScene")
        {
            UIManager.GetInstance().ShowUI("GameplayMenu");

        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(IELoadScene(sceneName));
        //SceneManager.LoadScene(sceneName);
    }

    public void LoadScene(string sceneName, Action<object> action)
    {
        StartCoroutine(IELoadScene(sceneName, action));
        //SceneManager.LoadScene(sceneName);
    }

    public void SetHasBluetoothConnect(bool isConnect)
    {
        HasBluetoothConnect = isConnect;
    }

    public void BluetoothConnect()
    {
        if (bluetoothManager)
            bluetoothManager.BluetoothConnect();
    }

    public void BluetoothDisConnect()
    {
        if (bluetoothManager)
            bluetoothManager.BluetoothDisconnect();
    }

    public void SetGameplayMode(GameplayModeEnum modeEnum){
        gameplayModeEnum = modeEnum;
    }


    private IEnumerator IELoadScene(string sceneName, Action<object> action = null)
    {
        if (uiMask.alpha == 1)
            uiMask.alpha = 0;

        uiMask.blocksRaycasts = true;

        while (uiMask.alpha < 1)
        {
            uiMask.alpha += Time.deltaTime * 3;
            yield return null;
        }

        if (action != null)
            action(gameObject);
        SceneManager.LoadScene(sceneName);

        while (uiMask.alpha > 0)
        {
            uiMask.alpha -= Time.deltaTime * 2;
            yield return null;
        }

        uiMask.blocksRaycasts = false;
    }
}

public enum GameplayModeEnum { Normal, Bluetooth }
