using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MinersWife : MonoBehaviour
{

    StateMachine<MinersWife> stateMachine;
    public StateMachine<MinersWife> StateMachine { get => stateMachine; set => stateMachine = value; }
    public bool shouldGoToToilet = false;
    [SerializeField] private Transform kitchen;
    [SerializeField] private Transform toilet;
    [SerializeField] private Transform house;
    [SerializeField] TextMeshProUGUI textbox;

    public Vector2 KitchenPos { get { return kitchen.position; } }
    public Vector2 ToiletPos { get { return toilet.position; } }
    public Vector2 HousePos { get { return house.position; } }

    private void Start()
    {
        StateMachine = new StateMachine<MinersWife>(this, new Householding(this));
    }

    // Update is called once per frame
    void Update()
    {
        StateMachine.Update();
    }
    public void StartPeePeeCoroutine(ref Coroutine coroutine)
    {
        coroutine = StartCoroutine(PeePeeCoroutine());
    }
    private IEnumerator PeePeeCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            float prob = Random.Range(0f, 1f);
            Debug.Log("peepee probability:" + prob);
            if (prob < 0.1f)
            {
                shouldGoToToilet = true;
                break;
            }
        }
        Debug.Log("PeePeeCoroutine Out ");

    }
    public void StopPeePeeCoroutine(ref Coroutine coroutine)
    {
        shouldGoToToilet = false;
        StopCoroutine(coroutine);
    }

    public void SetTextBox(string message)
    {
        if (textbox.text == message) return;

        Debug.Log("Elsa: " + message);
        textbox.text = message;
    }
}
