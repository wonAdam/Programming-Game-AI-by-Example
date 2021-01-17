using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MinersWife : MonoBehaviour, IStateMachine<MinersWife>, IMessageSender, IMessageReceiver
{

    StateMachine<MinersWife> stateMachine;
    public StateMachine<MinersWife> StateMachine { get => stateMachine; set { stateMachine = value; } }
    [SerializeField] private Transform kitchen;
    [SerializeField] private Transform toilet;
    [SerializeField] private Transform house;
    [SerializeField] TextMeshProUGUI textbox;
    [SerializeField] TextMeshProUGUI message;
    [SerializeField] public Miner myHusband;
    MessageDispatcher messageDispatcher;

    public Vector2 KitchenPos { get { return kitchen.position; } }
    public Vector2 ToiletPos { get { return toilet.position; } }
    public Vector2 HousePos { get { return house.position; } }
    private void OnEnable()
    {
        messageDispatcher = FindObjectOfType<MessageDispatcher>();
        RegisterToDispatcher();
    }
    private void OnDisable()
    {
        UnregisterToDispatcher();
    }
    private void Start()
    {
        StateMachine = new StateMachine<MinersWife>(this, new Householding(this));
    }

    // Update is called once per frame
    void Update()
    {
        StateMachine.Update();
    }
    
    public void SetTextBox(string message)
    {
        if (textbox.text == message) return;

        Debug.Log("Elsa: " + message);
        textbox.text = message;
    }

    public void SendMessage(MessageType msg, IMessageReceiver receiver = null)
    {
        message.text = textbox.text;
        Destroy(Instantiate(message.gameObject, textbox.transform), 1.5f);

        messageDispatcher.DispatchMessage(msg, this, receiver);
    }
    public bool ReceiveMessage(MessageType msg, IMessageSender sender)
    {
        if (StateMachine.currentState != null &&
            StateMachine.currentState.OnMessage(msg, sender)) return true;
        if (StateMachine.globalState != null &&
            StateMachine.globalState.OnMessage(msg, sender)) return true;

        return false;
    }
    public void RegisterToDispatcher() => messageDispatcher.Register(this);
    public void UnregisterToDispatcher() => messageDispatcher.Unregister(this);
}
