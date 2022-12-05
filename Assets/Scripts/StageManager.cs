using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Katniss
{
    public class StageManager : MonoBehaviour
    {
        [SerializeField] private static int stageFriendCount = 26;
        private int joinCount = 1;
        private int finishCount = 0;

        [SerializeField] private Friend[] friends = new Friend[stageFriendCount];

        void Start()
        {
            foreach(Friend friend in friends)
            {
                if (friend == null) return;

                friend.joinEvent += new FriendEventHandler(GetNewFriend);
                friend.outEvent += new FriendEventHandler(LoseFriend);
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

        void CountFinish()
        {
            finishCount++;
        }
    }
}