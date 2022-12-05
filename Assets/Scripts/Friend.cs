using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// join layer 연산 해제
// enemy layer 만들기
// animator isOut 만들기
// player의 rigidbody iskinematic 체크

namespace Katniss
{
    public delegate void FriendEventHandler();

    public class Friend : MonoBehaviour
    {
        [SerializeField] private bool isJoining;
        private bool isRunning;
        private int layerJoin;
        private int layerEnemy;
        //private float posX;
        private float deltaX;
        [SerializeField] private const float xSpeed = 0.0001f;
        [SerializeField] private const float ySpeed = 20f;

        private Vector3 dir;
        [SerializeField] private Animator animator;
        [SerializeField] private Rigidbody rigidbody;
        [SerializeField] private SkinnedMeshRenderer headMeshRenderer;
        [SerializeField] private SkinnedMeshRenderer bodyMeshRenderer;
        [SerializeField] private Material material;
        [SerializeField] private RunningManager runningManager;         // need to be modified

        public event FriendEventHandler joinEvent;
        public event FriendEventHandler outEvent;

        void Start()
        {
            layerJoin = LayerMask.NameToLayer("Join");
            //layerJoin = 1 << LayerMask.NameToLayer("Join");
            //layerEnemy = 1 << LayerMask.NameToLayer("Enemy");
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
                    FinishRunnig();
                    return;
                }
            }
        }
        /*
        private void OnCollisionEnter(Collision collision)
        {
            if (isJoining == false)
            {
                Join();
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
                    StartRunning();
            }
        }
        */
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == layerJoin && isJoining == false)
            {
                Join();
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
                    StartRunning();
            }
            /*
            if(other.gameObject.layer == layerBlock){
                Out();
                //animator.SetBool("isOutByBlock", true);
            }
            else if (other.gameObject.layer == layerEnemy)
            {
                Out();
                //animator.SetBool("isOutByEnemy", true);
            }
            */
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
            dir = new Vector3(deltaX / Time.deltaTime * xSpeed, 0, 1);
            transform.Translate(dir * ySpeed * Time.deltaTime, Space.World);
        }

        void FinishRunnig()
        {
            isRunning = false;
            animator.SetBool("isRunning", isRunning);
        }

        void Join()
        {
            joinEvent();

            isJoining = true;

            this.gameObject.layer = layerJoin;
            //rigidbody.isKinematic = true;
            
            headMeshRenderer.material = material;
            bodyMeshRenderer.material = material;
        }

        void Out()
        {
            outEvent();

            isJoining = false;
        }

        void FinishStage()
        {

        }
    }
}