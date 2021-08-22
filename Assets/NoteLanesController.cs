using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using UnityEngine;

public class NoteLanesController : MonoBehaviour
{
    [Header("需要的場景物體")]
    [SerializeField] private Transform noteSpawnTrans;
    [SerializeField] private Transform noteEndTrans;

    [Header("內部變量")]
    private int pendingEventID = 0;
    private List<KoreographyEvent> noteEventList = new List<KoreographyEvent>();
    private Queue<NoteController> trackedNotes = new Queue<NoteController>();

    private void Update()
    {
        while (trackedNotes.Count > 0 && trackedNotes.Peek().IsNoteMissed())
        {
            LevelManager.GetInstance().SetCombo(false);
            LevelManager.GetInstance().SetMessageText(0);
            LevelManager.GetInstance().SetLevelScore(0);
            trackedNotes.Dequeue();
        }

        if (LevelManager.GetInstance().gameplayEnum != GameplayEnum.Playing)
            return;

        CheckNextNoteForSpawn();
        InputSetting();
    }

    private void InputSetting()
    {
        if (trackedNotes.Count <= 0)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // if (trackedNotes.Peek().NoteID == 1)
            //     CheckNoteHit();
            CheckNodeID("1");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // if (trackedNotes.Peek().NoteID == 2)
            //     CheckNoteHit();
            CheckNodeID("2");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // if (trackedNotes.Peek().NoteID == 3)
            //     CheckNoteHit();
            CheckNodeID("3");
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            // if (trackedNotes.Peek().NoteID == 4)
            //     CheckNoteHit();
            CheckNodeID("4");
        }
    }

    public void CheckNodeID(string id)
    {
        if (string.IsNullOrEmpty(id))
            return;

        if (trackedNotes.Count <= 0)
            return;

        int count = 0;
        int.TryParse(id, out count);

        if (trackedNotes.Peek().NoteID == count)
            CheckNoteHit();
    }

    private void CheckNextNoteForSpawn()
    {
        if (!noteEndTrans)
            return;

        int samplesToTarget = GetSpawnSampleOffset();
        int curTime = MusicPlayer.GetInstance().DelayedSampleTime;

        while (pendingEventID < noteEventList.Count && noteEventList[pendingEventID].StartSample < curTime + samplesToTarget)
        {
            KoreographyEvent evt = noteEventList[pendingEventID];
            NoteController note = null;

            note = MusicPlayer.GetInstance().SpawnNote(noteSpawnTrans.position, evt.GetIntValue());

            note.InitNoteController(evt, noteEndTrans.position);
            trackedNotes.Enqueue(note);
            pendingEventID++;
        }
    }

    private int GetSpawnSampleOffset()
    {
        if (!noteSpawnTrans || !noteEndTrans)
            return 0;

        float spawnPos = noteSpawnTrans.localPosition.z - noteEndTrans.localPosition.z;
        float timeOfStartToEnd = spawnPos / MusicPlayer.GetInstance().NoteSpeed;

        return (int)timeOfStartToEnd * MusicPlayer.GetInstance().SampleRate;
    }

    private void CheckNoteHit()
    {
        if (trackedNotes.Count > 0)
        {
            NoteController note = trackedNotes.Peek();
            if (note.HitOffset > 4000)
            {
                //MusicPlayer.GetInstance().PlayMusicplayerSource();
                trackedNotes.Dequeue();

                int hitLevel = note.NoteHittable();

                if (hitLevel > 0)
                {
                    LevelManager.GetInstance().SetCombo(true);
                    if (hitLevel == 2)
                    {
                        LevelManager.GetInstance().SetLevelScore(2);
                        LevelManager.GetInstance().SetScore(true);
                        LevelManager.GetInstance().SetMessageText(2);
                    }
                    else
                    {
                        LevelManager.GetInstance().SetScore(false);
                        LevelManager.GetInstance().SetLevelScore(1);
                        LevelManager.GetInstance().SetMessageText(1);
                    }
                }
                else
                {
                    LevelManager.GetInstance().SetCombo(false);
                    LevelManager.GetInstance().SetLevelScore(0);
                    LevelManager.GetInstance().SetMessageText(0);
                }

                note.PlayHideAnim();
            }
        }
    }

    public void AddNoteEvent(KoreographyEvent evt)
    {
        noteEventList.Add(evt);
    }
}
