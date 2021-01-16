using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Miner : MonoBehaviour, IStateMachine<Miner>, IMessageSender, IMessageReceiver
{

    StateMachine<Miner> stateMachine;
    public StateMachine<Miner> StateMachine { get => stateMachine; set { stateMachine = value; } }

    public float goldCarried;
    public float moneyInBank;
    public float thirst;
    public float fatigue;
    [SerializeField] float goldPocketFullAmount;
    [SerializeField] float richEnoughGoldAmount;
    [SerializeField] float thirsty;
    [SerializeField] private Transform mine;
    [SerializeField] private Transform home;
    [SerializeField] private Transform bank;
    [SerializeField] private Transform tavern;
    [SerializeField] private Transform table;
    [SerializeField] TextMeshProUGUI textbox;
    [SerializeField] TextMeshProUGUI message;
    [SerializeField] public MinersWife myWife;
    public Vector2 MinePos { get { return mine.position; } }
    public Vector2 HomePos { get { return home.position; } }
    public Vector2 BankPos { get { return bank.position; } }
    public Vector2 TavernPos { get { return tavern.position; } }
    public Vector2 TablePos { get { return table.position; } }

    MessageDispatcher messageDispatcher;

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
        StateMachine = new StateMachine<Miner>(this, new EnterMineAndDigForNugget(this));
    }

    // Update is called once per frame
    void Update()
    {
        thirst += Time.deltaTime;
        StateMachine.Update();
    }
    public bool Thirsty() => thirst >= thirsty;
    public bool QuenchyEnough() => thirst <= 0;
    public bool PocketFull() => goldCarried >= goldPocketFullAmount;
    public bool RestWell() => fatigue <= 0f;
    public bool AmIRichEnough() => moneyInBank >= richEnoughGoldAmount;
    public void SetTextBox(string message)
    {
        if (textbox.text == message) return;

        Debug.Log("Bob: " + message);
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
