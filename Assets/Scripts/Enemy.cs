using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Katniss
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private bool isBoss = false;

        public Vector3 pos;
        [SerializeField] private Animator animator;

        void Start()
        {
            pos = transform.position;
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
            animator.SetBool("isBossActivate", true);
        }
    }
}