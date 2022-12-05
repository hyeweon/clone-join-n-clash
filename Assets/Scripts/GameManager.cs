using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Katniss
{
    public delegate void FriendEventHandler();

    public class GameManager : MonoBehaviour
    {
        public event FriendEventHandler joinEvent;
        public event FriendEventHandler outEvent;

        public void invokeJoinEvent()
        {
            joinEvent();
        }

        public void invokeOutEvent()
        {
            outEvent();
        }
    }
}