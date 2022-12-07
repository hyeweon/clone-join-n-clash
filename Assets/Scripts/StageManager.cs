using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Katniss
{
    public class StageManager : MonoBehaviour
    {
        public bool isStart = false;
        private bool isEncounterEnemy = false;

        [SerializeField] private static int stageFriendCount = 26;
        [SerializeField] private static int stageEnemyCount = 7;
        private int joinCount = 1;
        private float fov = 50f;

        private Vector3 camVec = new Vector3(0, 25, -40);
        [SerializeField] private Camera cam;
        [SerializeField] private Canvas canvas;
        [SerializeField] private Friend[] friends = new Friend[stageFriendCount];
        [SerializeField] private Enemy[] enemies = new Enemy[stageEnemyCount];

        void Start()
        {
            foreach(Friend friend in friends)
            {
                if (friend == null) return;

                friend.joinEvent += new FriendEventHandler(GetNewFriend);
                friend.outEvent += new FriendEventHandler(LoseFriend);
                friend.enemyEvent += new FriendEventHandler(DefenceEnemy);
            }
        }

        private void Update()
        {
            if (isStart == false && Input.GetMouseButtonDown(0))
            {
                CameraChange();
                return;
            }
        }

        void GetNewFriend()
        {
            joinCount++;
        }

        void LoseFriend()
        {
            joinCount--;
            Debug.Log(joinCount);
        }

        void DefenceEnemy()
        {
            StartCoroutine(assignFriends());
        }

        void CameraChange()
        {
            isStart = true;
            canvas.enabled = false;
            StartCoroutine(CameraMove());
        }

        IEnumerator CameraMove()
        {
            var camPos = cam.transform.localPosition;
            var camRot = cam.transform.localRotation;
            var camFov = cam.fieldOfView;

            var rot = Quaternion.Euler(22f, 0f, 0f);

            for (int i = 1; i < 101; i++)
            {
                cam.transform.localPosition = Vector3.Lerp(camPos, camVec, i / 100f);
                cam.transform.localRotation = Quaternion.Lerp(camRot, rot, i / 100f);
                cam.fieldOfView = Mathf.Lerp(camFov, fov, i / 100f);
                yield return null;
            }
            yield return null;
        }

        IEnumerator assignFriends()
        {
            int i = 0;
            for (int j = stageFriendCount - 1; j >= 0; j--)
            {
                if (friends[j] != null && friends[j].isJoining == true)
                {
                    friends[j].run2Enemy(enemies[i]);
                    
                    i++;
                    if (i >= stageEnemyCount) break;
                }

                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}