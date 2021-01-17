using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MessageType { HiHoneyImHome, StewReady }


public class MessageDispatcher : MonoBehaviour
{
    private static MessageDispatcher instance;
    public static MessageDispatcher Instance 
    {
        get { return instance; }
    }
    private List<IMessageReceiver> receivers = new List<IMessageReceiver>();

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
            
        instance = this;
    }
    
    public void DispatchMessage(MessageType msg, IMessageSender sender, IMessageReceiver receiver)
    {
        // msg를 모든 receiver에게 보냅니다.
        if(receiver == null)
        {
            foreach(var r in this.receivers)
            {
                if (r == null)
                {
                    this.receivers.Remove(r);
                    continue;
                }
                else
                {
                    r.ReceiveMessage(msg, sender);
                }
            }
        }
        // msg를 특정한 receiver에게만 보냅니다.
        else
        {
            foreach(var r in this.receivers)
                if(r != null && r == receiver)
                {
                    r.ReceiveMessage(msg, sender);
                    return;
                }
                else if(r == null)
                {
                    this.receivers.Remove(r);
                }
        }
    }

    public void Register(IMessageReceiver receiver)
    {
        receivers.Add(receiver);
    }

    public void Unregister(IMessageReceiver receiver)
    {
        receivers.Remove(receiver);
    }
}
