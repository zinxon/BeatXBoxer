using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetecterController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if(other.GetComponentInParent<NoteController>())
            other.GetComponentInParent<NoteController>().HideNote();
    }
}
