using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Katniss
{
    public class Friend : MonoBehaviour
    {
        [SerializeField] private bool isJoining;
        private bool isRunning;
        private int layerJoin;
        private float initialX;
        private float deltaX;
        [SerializeField] private const float speed = 20f;

        private Vector3 dir;
        [SerializeField] private Animator animator;
        [SerializeField] private SkinnedMeshRenderer headMeshRenderer;
        [SerializeField] private SkinnedMeshRenderer bodyMeshRenderer;
        [SerializeField] private Material material;

        [SerializeField] GameManager gameManager;

        void Start()
        {
            layerJoin = 1 << LayerMask.NameToLayer("Join");
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

        private void OnCollisionEnter(Collision collision)
        {
            if (isJoining == false)
            {
                Join();
            }
        }

        void StartRunning()
        {
            isRunning = true;
            animator.SetBool("isRunning", isRunning);

            initialX = Input.mousePosition.x;
        }

        void FinishRunnig()
        {
            isRunning = false;
            animator.SetBool("isRunning", isRunning);
        }

        void Run()
        {
            deltaX = Input.mousePosition.x - initialX;
            dir = new Vector3(deltaX * Time.deltaTime, 0, 1);

            transform.Translate(dir * speed * Time.deltaTime);
        }

        void Join()
        {
            gameManager.invokeJoinEvent();

            isJoining = true;
            
            headMeshRenderer.material = material;
            bodyMeshRenderer.material = material;
        }

        void Out()
        {
            gameManager.invokeOutEvent();

            isJoining = false;
        }
    }
}