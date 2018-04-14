using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingIcon : MonoBehaviour
{
    Vector3 rotationEuler;

    void Start()
    {
        StartCoroutine(animateLoad());
    }

    private void Update()
    {
        if (this.isActiveAndEnabled)
        {
            StartCoroutine(animateLoad());
        }
    }

    IEnumerator animateLoad()
    {
        while (this.isActiveAndEnabled)
        {
            rotationEuler += Vector3.forward * 60 * 0.5F;
            this.transform.rotation = Quaternion.Euler(rotationEuler);
            yield return new WaitForSeconds(0);
        }
    }
}