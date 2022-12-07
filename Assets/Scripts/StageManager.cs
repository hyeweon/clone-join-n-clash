using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Katniss
{
    public class StageManager : MonoBehaviour
    {
        public bool isStart = false;
        private bool isEncounterEnemy = false;

        [SerializeField] private static int stageFriendCount = 26;
        [SerializeField] private static int stageEnemyCount = 7;
        private int joinCount = 1;
        private const int defaultBossHp = 30;
        private int bossHp = defaultBossHp;
        private float fov = 50f;

        private Vector3 camVec = new Vector3(0, 25, -40);
        [SerializeField] private Camera cam;
        [SerializeField] private Canvas canvas;
        [SerializeField] private Canvas canvas2;
        [SerializeField] private Image hpFrame;
        [SerializeField] private Image hpFill;
        [SerializeField] private TextMeshProUGUI tmp;
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

        void BossDie()
        {
            //ani
            StartCoroutine(FadeOutUI());
        }

        void ChangeHpBar()
        {
            tmp.text = $"{bossHp}";
            hpFill.fillAmount = bossHp / defaultBossHp;
        }

        IEnumerator CameraMove()
        {
            const float movingTime = 2f;

            var camPos = cam.transform.localPosition;
            var camRot = cam.transform.localRotation;
            var camFov = cam.fieldOfView;

            var rot = Quaternion.Euler(22f, 0f, 0f);

            for (var time = 0f; time < movingTime; time += Time.deltaTime)
            {
                cam.transform.localPosition = Vector3.Lerp(camPos, camVec, time / movingTime);
                cam.transform.localRotation = Quaternion.Lerp(camRot, rot, time / movingTime);
                cam.fieldOfView = Mathf.Lerp(camFov, fov, time / movingTime);
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

                yield return new WaitForSeconds(0.5f);
            }
        }

        IEnumerator Fight()
        {
            for (var time = 0f; bossHp <= 0; time += Time.deltaTime)
            {
                if (time > 1f)
                {
                    bossHp--;
                    ChangeHpBar();
                }
                yield return null;
            }

            BossDie();
        }

        IEnumerator FadeOutUI()
        {
            var hpFillColor = hpFill.color;
            var hpFillAlpha = hpFillColor.a;

            for (var time = 0f; hpFillColor.a <= 0; time += Time.deltaTime)
            {
                hpFillColor.a = Mathf.Lerp(hpFillAlpha, 0f, time);
                hpFill.color = hpFillColor;

                yield return null;
            }
        }
    }
}