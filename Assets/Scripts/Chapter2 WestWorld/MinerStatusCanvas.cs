using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MinerStatusCanvas : MonoBehaviour
{
    [SerializeField] Miner miner;

    [SerializeField] TextMeshProUGUI fatigue;
    [SerializeField] TextMeshProUGUI thirst;
    [SerializeField] TextMeshProUGUI goldCarried;
    [SerializeField] TextMeshProUGUI moneyInBank;

    private void Update()
    {
        fatigue.text = $"Fatigue: {(int)miner.fatigue}";
        thirst.text = $"Thirst: {(int)miner.thirst}";
        goldCarried.text = $"Gold Carried: {(int)miner.goldCarried}";
        moneyInBank.text = $"Gold In Bank: {(int)miner.moneyInBank}";
    }
}
