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
        [SerializeField] private bool isJoining;
        private bool isRunning;
        private int layerGround;
        private int layerJoin;
        private int layerEnemy;
        private int layerBlock;
        //private float posX;
        private float deltaX;
        private float angle;
        [SerializeField] private const float xSpeed = 0.00001f;
        [SerializeField] private const float ySpeed = 20f;
        [SerializeField] private const float angleSpeed = 1f;

        private Vector3 dir;
        [SerializeField] private GameObject rig;
        [SerializeField] private Animator animator;
        [SerializeField] private Rigidbody body;
        [SerializeField] private SkinnedMeshRenderer headMeshRenderer;
        [SerializeField] private SkinnedMeshRenderer bodyMeshRenderer;
        [SerializeField] private Material material;
        [SerializeField] private ParticleSystem particle;
        [SerializeField] private RunningManager runningManager;         // need to be modified

        public event FriendEventHandler joinEvent;
        public event FriendEventHandler outEvent;

        void Start()
        {
            layerGround = LayerMask.NameToLayer("Ground");
            layerJoin = LayerMask.NameToLayer("Join");
            layerEnemy = LayerMask.NameToLayer("Enemy");
            layerBlock = LayerMask.NameToLayer("Block");
        }

        void Update()
        {
            if (isJoining == true)
            {
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
            }

            if (isRunning == false && rig.transform.rotation != transform.rotation)
                rig.transform.rotation = Quaternion.Lerp(rig.transform.rotation, transform.rotation, angleSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == layerGround)
            {
                if (this.transform.position.x < 0)
                    runningManager.isOnLeftEdge = true;
                else
                    runningManager.isOnRightEdge = true;
            }

            if (other.gameObject.layer == layerJoin && isJoining == false)
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
            }

            else if (other.gameObject.layer == layerEnemy)
            {
                Out();
                other.gameObject.GetComponent<Collider>().enabled = false;
                StartCoroutine(OutByEnemy(other.gameObject));
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
            rig.transform.rotation = Quaternion.Lerp(rig.transform.rotation, Quaternion.Euler(0.0f, angle, 0.0f), angleSpeed * Time.deltaTime);

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

        }

        IEnumerator OutByBlock()
        {
            if (isPlayer == true) {
                isRunning = false;
                animator.SetBool("isRunning", isRunning);
            }
            else FinishRunning();

            particle.Play();

            //yield return new WaitForSeconds(0.2f);

            rig.SetActive(false);

            yield return null;
        }

        IEnumerator OutByEnemy(GameObject gameObject)
        {
            isRunning = false;
            //animator.SetBool("isOutByEnemy", true);

            var pos = transform.position;
            var targetPos = gameObject.transform.position;

            for (int i = 0; i < 100; i++)
            {
                transform.position = Vector3.Lerp(pos, targetPos, i / 200f);

                yield return null;
            }
        }
    }
}