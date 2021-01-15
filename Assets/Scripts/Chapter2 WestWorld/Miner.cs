using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Miner : MonoBehaviour
{

    StateMachine<Miner> stateMachine;
    public StateMachine<Miner> StateMachine { get => stateMachine; set => stateMachine = value; }

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
    [SerializeField] TextMeshProUGUI textbox;

    public Vector2 MinePos { get { return mine.position; } }
    public Vector2 HomePos { get { return home.position; } }
    public Vector2 BankPos { get { return bank.position; } }
    public Vector2 TavernPos { get { return tavern.position; } }

    private void Start()
    {
        StateMachine = new StateMachine<Miner>(this);
        StateMachine.SetCurrentState(new GoHomeAndSleepTilRested(this));
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
        Debug.Log("Bob: " + message);
        textbox.text = message;
    }
}
