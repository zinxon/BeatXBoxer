using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using UnityEngine;

public class NoteController : MonoBehaviour
{
    [Header("內部變量")]
    private int noteID;
    public int NoteID { get => noteID; }
    [SerializeField] private int hitOffset;
    public int HitOffset { get => hitOffset; }
    private bool isRunning = true;
    private Vector3 targetPos;
    private KoreographyEvent trackedEvent;
    private Animator anim;

    private MusicPlayer musicPlayer;
    private Transform trans;

    private void Start()
    {
        anim = GetComponent<Animator>();

        musicPlayer = MusicPlayer.GetInstance();
        trans = GetComponent<Transform>();
    }

    public void InitNoteController(KoreographyEvent evt, Vector3 targetPos)
    {
        if (trackedEvent == null)
        {
            trackedEvent = evt;
            noteID = trackedEvent.GetIntValue();
        }
        this.targetPos = targetPos;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (LevelManager.GetInstance().gameplayEnum != GameplayEnum.Playing)
            return;

        UpdatePosition();
        GetHitOffset();

        if (trans.position.z <= targetPos.z - 10f)
        {
            HideNote();
        }
    }

    private Vector3 pos = Vector3.zero;

    private void UpdatePosition()
    {
        if (trackedEvent == null || !isRunning)
            return;

        pos = targetPos;
        pos.z -= (musicPlayer.DelayedSampleTime - trackedEvent.StartSample) / (float)musicPlayer.SampleRate * musicPlayer.NoteSpeed;
        trans.position = pos;
    }

    public void HideNote()
    {
        isRunning = false;
        LevelManager.GetInstance().SetDisableNote();
        gameObject.SetActive(false);
    }

    public void PlayHideAnim()
    {
        isRunning = false;
        if (anim)
        {
            anim.Play("Click");
            StartCoroutine(IEHideAnim());
        }
        else
        {
            HideNote();
        }
    }

    private IEnumerator IEHideAnim()
    {
        yield return new WaitForSeconds(1f);
        HideNote();
    }

    private int curTime, noteTime, hitWindow;

    private void GetHitOffset()
    {
        if (!isRunning)
            return;

        curTime = musicPlayer.DelayedSampleTime;
        noteTime = trackedEvent.StartSample;
        hitWindow = musicPlayer.HitWindowRangeInSamples;

        //hitOffset = hitWindow - Mathf.Abs(noteTime - curTime);
        hitOffset = hitWindow - (noteTime - curTime);
    }

    public int NoteHittable()
    {
        int hitLevel = 0;

        if (hitOffset >= -5000)
        {
            if (hitOffset > 15000)
            {
                hitLevel = 0;
            }
            else if (hitOffset > 5500 && hitOffset <= 15000)
            {
                hitLevel = 2;
            }
            else if (hitOffset > -5000 || hitOffset <= 5500)
            {
                hitLevel = 1;
            }
        }
        else
        {
            this.enabled = false;
        }

        return hitLevel;
    }

    public bool IsNoteMissed()
    {
        bool isMissed = true;

        if (enabled)
        {
            int curTime = musicPlayer.DelayedSampleTime;
            int noteTime = trackedEvent.StartSample;
            int hitWindow = musicPlayer.HitWindowRangeInSamples;

            isMissed = curTime - noteTime > hitWindow;
        }

        return isMissed;
    }
}
