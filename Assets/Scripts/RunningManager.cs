using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningManager : MonoBehaviour
{
    // need to be modified

    public float posX;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            posX = Input.mousePosition.x;
        }
    }
}
