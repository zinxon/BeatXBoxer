using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;
using UnityEngine;

public class MusicPlayer : UnitySingleton<MusicPlayer>
{
    [Header("基本變量")]
    [SerializeField] private string eventID;
    public string EventID { get => eventID; }
    [SerializeField] private float noteSpeed = 1;
    public float NoteSpeed { get => noteSpeed; }
    [SerializeField, Range(0, 20)] private float leadInTime = 1;
    [SerializeField, Range(8, 300)] private float hitWindowRangeInMS;
    private int hitWindowRangeInSamples;
    public int HitWindowRangeInSamples { get => hitWindowRangeInSamples; }

    [Header("需要預制體")]
    [SerializeField] private Koreography playingKoreo; //暫時使用(List)
    [SerializeField] private List<NoteController> notePrefabList = new List<NoteController>();

    [Header("需要的場景物體")]
    [SerializeField] private List<NoteLanesController> noteLanesList = new List<NoteLanesController>();

    [Header("內部變量")]
    private float leadInTimeLeft;
    private float timeLeftToPlay;
    public int SampleRate { get => playingKoreo.SampleRate; }
    public int DelayedSampleTime { get => playingKoreo.GetLatestSampleTime() - (int)(SampleRate * leadInTimeLeft); }
    private bool isPlayAudio = false;

    [Header("組件")]
    public AudioSource audioSource;
    private AudioSource musicplayerSource;

    protected override void Awake()
    {
        base.Awake();
        musicplayerSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        InitLeadInTime();
        InitKoreographer();
    }

    private void InitLeadInTime()
    {
        if (leadInTime > 0)
        {
            leadInTimeLeft = leadInTime;
            timeLeftToPlay = leadInTime;
        }
        else
        {
            PlayKoreographerAudio();
        }
    }

    private void InitKoreographer()
    {
        playingKoreo = Koreographer.Instance.GetKoreographyAtIndex(0);
        KoreographyTrackBase track = playingKoreo.GetTrackByID(EventID);
        List<KoreographyEvent> rawEventList = track.GetAllEvents();

        if (noteLanesList.Count > 0)
        {
            for (int i = 0; i < rawEventList.Count; i++)
            {
                if (rawEventList[i].GetIntValue() % 2 == 0)
                {
                    noteLanesList[1].AddNoteEvent(rawEventList[i]);
                }
                else
                {
                    noteLanesList[0].AddNoteEvent(rawEventList[i]);
                }
            }
        }

        hitWindowRangeInSamples = (int)(0.001f * hitWindowRangeInMS * SampleRate);

        LevelManager.GetInstance().totalNoteCount = rawEventList.Count;
    }

    private void Update()
    {
        if (LevelManager.GetInstance().gameplayEnum == GameplayEnum.Playing)
        {
            TimeLeadingUpdate();
            LeadInTimeLeftUpdate();
        }
    }

    public void PauseAudioPlaying()
    {
        if (!audioSource)
            return;

        if (isPlayAudio)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
            }
            else
            {
                audioSource.Play();
            }
        }
    }

    private void TimeLeadingUpdate()
    {
        if (isPlayAudio)
            return;

        if (timeLeftToPlay > 0)
        {
            timeLeftToPlay -= Time.unscaledDeltaTime;
        }
        else
        {
            PlayKoreographerAudio();
            timeLeftToPlay = 0;
            isPlayAudio = true;
        }
    }

    private void LeadInTimeLeftUpdate()
    {
        if (leadInTimeLeft > 0)
            leadInTimeLeft = Mathf.Max(leadInTimeLeft - Time.unscaledDeltaTime, 0);
    }

    private void PlayKoreographerAudio()
    {
        if (audioSource && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public NoteController SpawnNote(Vector2 startPos, int count)
    {
        NoteController note;
        note = Instantiate(notePrefabList[count - 1], startPos, Quaternion.identity);
        return note;
    }

    public void PlayMusicplayerSource()
    {
        musicplayerSource.Play();
    }

    private float AudioSourceValue()
    {
        if (!audioSource)
            return 0;

        return audioSource.time / audioSource.clip.length;
    }
}
