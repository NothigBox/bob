using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    public Action<PoolObject> OnDisabled;

    private void OnDisable()
    {
        if (gameObject.activeSelf == false)
        {
            OnDisabled?.Invoke(this);
        }
    }
}
