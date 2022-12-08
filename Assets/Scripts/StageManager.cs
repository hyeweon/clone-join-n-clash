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
        private const float defaultBossHp = 30f;
        public int bossHp = (int)defaultBossHp;
        private const float defaultStageTime = 10f;
        private float stageTime = 0;
        private float fov = 50f;

        private Vector3 camVec = new Vector3(0, 25, -40);
        [SerializeField] private Camera cam;
        [SerializeField] private Canvas canvas;
        [SerializeField] private Canvas canvas2;
        [SerializeField] private Canvas playUI;
        [SerializeField] private GameObject hpBar;
        [SerializeField] private RectTransform rectTrans;
        [SerializeField] private Image hpFrame;
        [SerializeField] private Image hpFill;
        [SerializeField] private Image progressFill;
        [SerializeField] private TextMeshProUGUI tmp;

        [SerializeField] private Friend[] friends = new Friend[stageFriendCount];
        [SerializeField] private Enemy[] enemies = new Enemy[stageEnemyCount];

        public bool isHpShows = false;
        public Vector3 screenPos;
        public Vector3 bossPos;
        public GameObject boss;
        public Enemy bossEnemy;

        void Start()
        {
            playUI.enabled = false;

            foreach(Friend friend in friends)
            {
                if (friend == null) return;

                friend.joinEvent += new FriendEventHandler(GetNewFriend);
                friend.outEvent += new FriendEventHandler(LoseFriend);
                friend.enemyEvent += new FriendEventHandler(DefenceEnemy);
                friend.finalEvent += new FriendEventHandler(AttackBoss);
            }
        }

        private void Update()
        {
            if (isStart == true)
            {
                progressFill.fillAmount = stageTime / defaultStageTime;
                stageTime += Time.deltaTime;
            }

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
            playUI.enabled = true;
            canvas.enabled = false;
            StartCoroutine(CameraMove());
        }

        void AttackBoss()
        {
            Debug.Log("attackBoss");
            StartCoroutine(FinalCameraMove());
            StartCoroutine(Fight());
        }

        void BossDie()
        {
            bossEnemy.animator.SetTrigger("Die");
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

        IEnumerator FinalCameraMove()
        {
            const float movingTime = 1f;

            var camRot = cam.transform.localRotation;

            var rot = Quaternion.Euler(35f, 0f, 0f);

            for (var time = 0f; time < movingTime; time += Time.deltaTime)
            {
                cam.transform.localRotation = Quaternion.Lerp(camRot, rot, time / movingTime);
                yield return null;
            }
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
            int i;

            for (var time = 0f; bossHp > 0; time += Time.deltaTime)
            {
                if (time > 0.5f)
                {
                    bossHp--;
                    ChangeHpBar();
                    time -= 0.5f;

                    for(int j = 0; j < 3; j++)
                    {
                        i = Random.Range(0, maxExclusive: stageFriendCount);
                        if (friends[i] != null && friends[i].isJoining == true)
                        {
                            StartCoroutine(friends[i].OutByBlock());
                            friends[i].Out();
                        }
                        yield return new WaitForSeconds(Random.Range(0f, 0.5f));
                    }
                }
                yield return null;
            }

            BossDie();
        }

        public IEnumerator FadeOutUI()
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