using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    public float timer = 0;
    public ParticleSystem ps;

    private void Start()
    {
        if (!ps)
            ps = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (timer >= 0.6f)
        {
            gameObject.SetActive(false);
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    public virtual void OnObjectReuse()
    {
        if (!ps)
        {
            ps = GetComponent<ParticleSystem>();
        }

        timer = 0;
        ps.Play();
    }
}
