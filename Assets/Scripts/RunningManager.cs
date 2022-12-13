using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Katniss
{

    public class RunningManager : MonoBehaviour
    {
        // need to be modified

        public bool isStageEnded = false;
        public bool isOnLeftEdge;
        public bool isOnRightEdge;
        public float posX;

        public bool isEncounterBoss = false;
        public bool isBossActivate = false;

        public Vector3 screenPos;
        public Vector3 bossPos;
        public Enemy boss;

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                posX = Input.mousePosition.x;
            }

            if (isStageEnded == true && isBossActivate == false)
            {
                isBossActivate = true;
                boss.ActivateBoss();
            }
        }
    }
}