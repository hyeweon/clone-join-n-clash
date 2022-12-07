using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// join layer ???? ????
// enemy layer ??????
// animator isOut ??????
// player?? rigidbody iskinematic ????

namespace Katniss
{
    public delegate void FriendEventHandler();

    public class Friend : MonoBehaviour
    {
        [SerializeField] private bool isPlayer;
        public bool isJoining;
        private bool isRunning;
        private bool is2Boss = false;

        private int layerGround;
        private int layerJoin;
        private int layerDisjoin;
        private int layerEnemy;
        private int layerBlock;
        private int layerFinishLine;
        //private float posX;
        private float deltaX;
        private float angle;
        [SerializeField] private const float xSpeed = 0.0001f;
        [SerializeField] private const float ySpeed = 25f;
        [SerializeField] private const float angleSpeed = 2f;

        private Vector3 dir;
        [SerializeField] private GameObject rig;
        [SerializeField] private Animator animator;
        [SerializeField] private Rigidbody body;
        [SerializeField] private CapsuleCollider capsuleCollider;
        [SerializeField] private SkinnedMeshRenderer headMeshRenderer;
        [SerializeField] private SkinnedMeshRenderer bodyMeshRenderer;
        [SerializeField] private Material material;
        [SerializeField] private ParticleSystem particle;
        [SerializeField] private RunningManager runningManager;         // need to be modified
        [SerializeField] private Enemy bossEnemy;

        public event FriendEventHandler joinEvent;
        public event FriendEventHandler outEvent;
        public event FriendEventHandler enemyEvent;

        void Start()
        {
            layerGround = LayerMask.NameToLayer("Ground");
            layerJoin = LayerMask.NameToLayer("Join");
            layerDisjoin = LayerMask.NameToLayer("Disjoin");
            layerEnemy = LayerMask.NameToLayer("Enemy");
            layerBlock = LayerMask.NameToLayer("Block");
            layerFinishLine = LayerMask.NameToLayer("FinishLine");
        }

        void Update()
        {
            if (isJoining == false)
                return;

            if (runningManager.isStageEnded)
            {
                if (is2Boss == false)
                {
                    StartCoroutine(FinishLine());
                    is2Boss = true;
                }
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                StartRunning();
                return;
            }
            if (Input.GetMouseButton(0))
            {
                Run();
                return;
            }
            if (Input.GetMouseButtonUp(0))
            {
                FinishRunning();
                return;
            }

            if (isRunning == false && rig.transform.rotation != transform.rotation)
                rig.transform.rotation = Quaternion.Lerp(rig.transform.rotation, transform.rotation, angleSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == layerGround)
            {
                Debug.Log("ground");
                if (this.transform.position.x < 0)
                    runningManager.isOnLeftEdge = true;
                else
                    runningManager.isOnRightEdge = true;
            }

            if (other.gameObject.layer == layerJoin && isJoining == false &&this.gameObject.layer==layerDisjoin)
            {
                Join();
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
                    StartRunning();

                return;
            }
            else if(other.gameObject.layer == layerBlock){
                Debug.Log("blocked");
                StartCoroutine(OutByBlock());

                Out();

                return;
            }

            else if (other.gameObject.layer == layerEnemy)
            {
                Debug.Log("enemy");
                other.gameObject.GetComponent<Collider>().enabled = false;
                
                enemyEvent();

                return;
            }

            else if (other.gameObject.layer == layerFinishLine)
            {
                Debug.Log("finish");
                other.gameObject.GetComponent<Collider>().enabled = false;

                FinishStage();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == layerGround)
            {
                if (this.transform.position.x < 0)
                    runningManager.isOnLeftEdge = false;
                else
                    runningManager.isOnRightEdge = false;
            }
        }

        void StartRunning()
        {
            isRunning = true;
            animator.SetBool("isRunning", isRunning);

            //posX = Input.mousePosition.x;
        }

        void Run()
        {
            deltaX = Input.mousePosition.x - runningManager.posX;
            if (runningManager.isOnLeftEdge == true)
            {
                if (deltaX < 0) deltaX = 0;
            }
            else if (runningManager.isOnRightEdge == true)
            {
                if (deltaX > 0) deltaX = 0;
            }
            dir = new Vector3(deltaX / Time.deltaTime * xSpeed, 0, 1);

            angle = Mathf.Atan2(deltaX, 0) * Mathf.Rad2Deg;
            rig.transform.rotation = Quaternion.Lerp(rig.transform.rotation, Quaternion.Euler(0.0f, angle/2, 0.0f), angleSpeed * Time.deltaTime);

            transform.Translate(dir * ySpeed * Time.deltaTime, Space.World);
        }

        void FinishRunning()
        {
            isRunning = false;
            animator.SetBool("isRunning", isRunning);
        }

        void Join()
        {
            joinEvent();

            isJoining = true;

            this.gameObject.layer = layerJoin;
            
            headMeshRenderer.material = material;
            bodyMeshRenderer.material = material;
        }

        void Out()
        {
            outEvent();

            if (isPlayer == false) isJoining = false;

        }

        void FinishStage()
        {
            runningManager.isStageEnded = true;
        }

        public void run2Enemy(Enemy enemy)
        {
            Out();
            StartCoroutine(OutByEnemy(enemy));
        }

        IEnumerator OutByBlock()
        {
            if (isPlayer == true) {
                isRunning = false;
                animator.SetBool("isRunning", isRunning);
            }
            else FinishRunning();

            particle.Play();
            rig.SetActive(false);

            yield return null;
        }

        IEnumerator OutByEnemy(Enemy enemy)
        {
            var enemyRunTime = 2f;

            var pos = transform.position;
            var targetPos = enemy.pos;

            Vector3 enemyPos;
            Vector3 myPos;

            enemy.Run();
            animator.speed = 3.0f;

            for (var time = 0f; time < enemyRunTime; time += Time.deltaTime)
            {
                enemy.gameObject.transform.position = Vector3.Lerp(targetPos, pos, time / 3f);
                transform.position = Vector3.Lerp(pos, targetPos, time / 2f);

                if (rig.transform.rotation != transform.rotation)
                    rig.transform.rotation = Quaternion.Lerp(rig.transform.rotation, transform.rotation, angleSpeed * Time.deltaTime);

                enemyPos = enemy.gameObject.transform.position;
                myPos = transform.position;
                if ((enemyPos - myPos).magnitude < 0.5f)
                {
                    isRunning = false;

                    animator.SetTrigger("Die");
                    enemy.Die();
                    break;
                }

                yield return null;
            }
        }

        IEnumerator FinishLine()
        {
            var finishingTime = 3f;

            var pos = transform.position;
            var targetPos = bossEnemy.pos;
            targetPos.x = (pos.x + targetPos.x) / 2f;

            Vector3 myPos;

            for (var time = 0f; time < finishingTime; time += Time.deltaTime)
            {
                transform.position = Vector3.Lerp(pos, targetPos, time);

                myPos = transform.position;
                if ((targetPos - myPos).magnitude < 15)
                {
                    runningManager.isEncounterBoss = true;
                }

                if (runningManager.isEncounterBoss)
                {
                    this.body.isKinematic = false;
                    this.capsuleCollider.radius = 1.2f;
                    this.capsuleCollider.isTrigger = false;
                    animator.SetBool("isRunning", false);
                    break;
                }
                yield return null;
            }

            StartCoroutine(AttackBoss());
        }

        IEnumerator AttackBoss()
        {
            var attackTime = 30f;

            var pos = transform.position;
            var targetPos = bossEnemy.pos;

            for (var time = 0f; time < attackTime; time += Time.deltaTime)
            {
                pos = transform.position;
                targetPos = bossEnemy.gameObject.transform.position;
                body.AddForce(targetPos - pos);

                yield return null;
            }
        }
    }
}