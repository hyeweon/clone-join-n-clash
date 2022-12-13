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
        public bool isBossDie = false;

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
        [SerializeField] private Canvas stageClearUI;
        [SerializeField] private GameObject hpBar;
        [SerializeField] private RectTransform rectTrans;
        [SerializeField] private Image hpFrame;
        [SerializeField] private Image hpFill;
        [SerializeField] private Image progressFill;
        [SerializeField] private Image hammerFill;
        [SerializeField] private Image button;
        [SerializeField] private TextMeshProUGUI tmp;

        [SerializeField] private Friend[] friends = new Friend[stageFriendCount];
        [SerializeField] private Enemy[] enemies = new Enemy[stageEnemyCount];

        public bool isHpShows = false;
        public Vector3 screenPos;
        public Vector3 bossPos;
        public GameObject boss;
        public Enemy bossEnemy;
        public ParticleSystem particle;
        public ParticleSystem bloodParticle;

        void Start()
        {
            playUI.enabled = false;
            stageClearUI.enabled = false;

            foreach (Friend friend in friends)
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
            bossEnemy.Die();
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

            playUI.enabled = false;

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
            const float movingTime = 2f;

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

                yield return new WaitForSeconds(0.3f);
            }
        }

        IEnumerator Fight()
        {
            int i;
            var randomTime = Random.Range(1f, 5f);

            for (var time = 0f; bossHp > 0 && isBossDie == false; time += Time.deltaTime)
            {
                if (time > randomTime)
                {
                    bossHp--;
                    ChangeHpBar();
                    time = 0;

                    i = Random.Range(0, maxExclusive: stageFriendCount);
                    if (friends[i] != null && friends[i].isJoining == true && Random.Range(0, maxExclusive: 2) == 0)
                    {
                        StartCoroutine(friends[i].OutByBlock());
                        friends[i].Out();
                    }
                }

                randomTime = Random.Range(1f, 5f);
                yield return null;
            }

            
            isBossDie = true;
            BossDie();
        }

        public IEnumerator FadeOutUI()
        {
            particle.Play();
            bloodParticle.Play();

            var color = hpFrame.color;

            for (var time = 0.5f; time >= 0; time -= Time.deltaTime)
            {
                color.a = time*2;
                tmp.alpha = time * 2;
                hpFrame.color = color;

                yield return null;
            }
            yield return new WaitForSeconds(0.1f);

            boss.SetActive(false);
            yield return new WaitForSeconds(1f);

            stageClearUI.enabled = true;
            StartCoroutine(FillUpHammer());
        }

        public IEnumerator FillUpHammer()
        {
            hammerFill.fillAmount = 0;

            for (float i = 0f; i <= 5.0f; i += Time.deltaTime)
            {
                hammerFill.fillAmount = i / 4.0f;
                i += Time.deltaTime;

                if (hammerFill.fillAmount >= 1.0f)
                    button.enabled = true;
            }

            yield return null;
        }
    }
}