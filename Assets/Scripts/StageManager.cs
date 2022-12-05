using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Katniss
{
    public class StageManager : MonoBehaviour
    {
        private int friendCount = 1;

        [SerializeField] private GameManager gameManager;

        void Start()
        {
            gameManager.joinEvent += new FriendEventHandler(GetNewFriend);
            gameManager.outEvent += new FriendEventHandler(LoseFriend);
        }

        void Update()
        {
            
        }

        private void GetNewFriend()
        {
            friendCount++;
            Debug.Log(friendCount);
        }

        private void LoseFriend()
        {
            friendCount--;
            Debug.Log(friendCount);
        }
    }
}