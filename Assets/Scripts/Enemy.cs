using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Katniss
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private bool isBoss = false;
        [SerializeField] private Rigidbody rig;

        public Vector3 pos;
        private Vector3 screenCenter;
        private Vector3 targetPos;
        public Animator animator;
        [SerializeField] private Camera cam;
        private Ray cameraRay;
        private RaycastHit cameraHit;
        private int groundLayerMask;

        public StageManager stageManager;

        void Start()
        {
            pos = transform.position;
            targetPos = new Vector3(-4f, 0f, 200f);

            groundLayerMask = 1 << LayerMask.NameToLayer("Ground");

            screenCenter = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);
        }

        public void Run()
        {
            animator.SetBool("isRunning", true);
        }

        public void Die()
        {
            animator.SetTrigger("Die");
        }

        public void ActivateBoss()
        {
            StartCoroutine(MoveBoss2Friends());
        }

        IEnumerator MoveBoss2Friends()
        {
            animator.SetBool("isRunning", true);

            for (float time = 0f; time < 2f; time += Time.deltaTime)
            {
                transform.position = Vector3.Lerp(pos, targetPos, time/2f);
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);
            StartCoroutine(MoveBoss());
        }

        IEnumerator MoveBoss()
        {
            animator.SetBool("isFighting", true);

            for (float time = 0f; time < 5f; time += Time.deltaTime)
            {
                cameraRay = Camera.main.ScreenPointToRay(screenCenter);

                if (Physics.Raycast(cameraRay, out cameraHit, Mathf.Infinity, groundLayerMask))
                {
                    pos = transform.position;
                    targetPos = cameraHit.point;
                    transform.position = Vector3.Lerp(pos, targetPos, time / 5f);
                }

                yield return null;
            }
        }
    }
}