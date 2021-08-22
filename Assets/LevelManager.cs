using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : UnitySingleton<LevelManager>
{
    public GameplayEnum gameplayEnum { get; private set; } = GameplayEnum.Start;

    public int CombineValue { get; private set; }
    public int ScoreValue { get; private set; }

    private TextMeshProUGUI comboTxt, scoreTxt;
    private TextMeshPro msgTxt;
    private float msgTimer;
    private float msgMaxTime = 1;
    private int perfectValue, greatValue, missValue;
    public int PerfectValue { get => perfectValue; }
    public int GreatBValue { get => greatValue; }
    public int MissValue { get => missValue; }
    public bool isResult = false;
    private int disableNoteCount = 0;
    public int totalNoteCount = 0;
    [SerializeField] private List<NoteLanesController> lanesList = new List<NoteLanesController>();


    private void Start()
    {
        gameplayEnum = GameplayEnum.Playing;

        comboTxt = GameObject.Find("ComboTxt").GetComponent<TextMeshProUGUI>();
        if (comboTxt)
            comboTxt.text = "0";

        scoreTxt = GameObject.Find("ScoreTxt").GetComponent<TextMeshProUGUI>();
        if (scoreTxt)
            scoreTxt.text = "Score: 0";

        msgTxt = GameObject.Find("MsgTxt").GetComponent<TextMeshPro>();
        if (msgTxt)
            msgTxt.enabled = false;

        msgTimer = 0;

        lanesList = FindObjectsOfType<NoteLanesController>().ToList();
    }

    private void Update()
    {
        if (msgTimer > 0)
        {
            msgTimer -= Time.deltaTime;
            msgTxt.color = new Color(msgTxt.color.r, msgTxt.color.g, msgTxt.color.b, msgTimer);
        }
        else
        {
            if (msgTxt)
                msgTxt.enabled = false;
        }

        if(gameplayEnum == GameplayEnum.Playing && disableNoteCount == totalNoteCount)
            gameplayEnum = GameplayEnum.Result;
    }

    public void SetMessageText(int state)
    {
        msgTxt.enabled = true;
        switch (state)
        {
            case 0:
                msgTxt.text = "Miss";
                msgTxt.color = new Color(0.75f, 0.75f, 0.75f);
                break;
            case 1:
                msgTxt.color = new Color(0.95f, 0.83f, 0.33f);
                msgTxt.text = "Great";
                break;
            case 2:
                msgTxt.color = new Color(0.52f, 0.94f, 0.32f);
                msgTxt.text = "Perfect";
                break;
            default:
                break;
        }

        msgTimer = msgMaxTime;
    }

    public void SetCombo(bool isClickSuccess)
    {
        if (isClickSuccess)
            CombineValue++;
        else
            CombineValue = 0;

        if (comboTxt)
            comboTxt.text = CombineValue.ToString();
    }

    public void SetScore(bool isPerfect)
    {
        if (isPerfect)
        {
            ScoreValue += 200;
        }
        else
        {
            ScoreValue += 100;
        }

        if (scoreTxt)
            scoreTxt.text = string.Format("Score: {0}", ScoreValue);
    }

    public void SetLevelScore(int level)
    {
        switch (level)
        {
            case 0:
                missValue++;
                break;
            case 1:
                greatValue++;
                break;
            case 2:
                perfectValue++;
                break;
            default:
                missValue++;
                break;
        }
    }

    public void SetGameplayEnum(GameplayEnum gameplayEnum){
        this.gameplayEnum = gameplayEnum;
    }

    public void SetDisableNote(){
        disableNoteCount++;
    }

    public void SendMessageToNoteLanesController(string msg){
        foreach(NoteLanesController lanes in lanesList){
            lanes.CheckNodeID(msg);
        }
    }
}

public enum GameplayEnum { Start, Playing, Pause, Result }

