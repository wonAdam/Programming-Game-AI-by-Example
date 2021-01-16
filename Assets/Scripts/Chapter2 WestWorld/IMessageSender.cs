using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMessageSender
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="msg">MessageDispatcher.cs의 public enum MessageType을 참고하십시오.</param>
    /// <param name="receiver">특정한 receiver에게만 보내는 것이 아니라면 default로 두시면 됩니다.</param>
    /// 
    void SendMessage(MessageType msg, IMessageReceiver receiver = null);
}
