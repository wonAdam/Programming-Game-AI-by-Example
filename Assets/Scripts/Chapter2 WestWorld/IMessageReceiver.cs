using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMessageReceiver
{
    /// <summary>
    /// 처리할 MessageType에 대해 함수내부에서 처리하면 됩니다.
    /// </summary>
    /// <param name="msg">MessageDispatcher.cs의 public enum MessageType을 참고하십시오.</param>
    /// <returns>처리할 msg였다면 true를, 처리할 필요없는 msg였다면 false를 리턴하십시오.</returns>
    bool ReceiveMessage(MessageType msg, IMessageSender sender);
    void RegisterToDispatcher();
    void UnregisterToDispatcher();
}
