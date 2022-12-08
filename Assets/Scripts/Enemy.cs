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
        [SerializeField] private Animator animator;
        [SerializeField] private Camera cam;
        private Ray cameraRay;
        private RaycastHit cameraHit;
        private int groundLayerMask;

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
            StartCoroutine(MoveBoss());
        }

        IEnumerator MoveBoss()
        {
            for (float time = 0f; time < 3f; time += Time.deltaTime)
            {
                Debug.Log("check");
                transform.position = Vector3.Lerp(pos, targetPos, time / 3f);

                yield return null;
            }

            yield return new WaitForSeconds(0.5f);

            for (float time = 0f; time < 3f; time += Time.deltaTime)
            {
                cameraRay = Camera.main.ScreenPointToRay(screenCenter);

                if (Physics.Raycast(cameraRay, out cameraHit, Mathf.Infinity, groundLayerMask))
                {
                    Debug.Log("check2");
                    pos = transform.position;
                    targetPos = cameraHit.point;
                    targetPos.y = pos.y;
                    transform.position = Vector3.Lerp(pos, targetPos, time / 3f);
                }

                yield return null;
            }
        }
    }
}