using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Householding : State<MinersWife>
{
    Coroutine shouldpeepee;
    bool shouldGoToToilet;
    public Householding(MinersWife wife) : base(wife) { }

    public override void Enter(State<MinersWife> previousState)
    {
        _owner.transform.position = _owner.HousePos;

        //_owner.StartPeePeeCoroutine(ref shouldpeepee);
        shouldpeepee = _owner.StartCoroutine(PeePeeCoroutine());
    }
    public override void Execute()
    {
        base.Execute();

        if (shouldGoToToilet && DelayedEnough())
            _owner.StateMachine.ChangeState(new VisitToilet(_owner));

        _owner.SetTextBox("바닥을 닦는다.");
    }

    public override void Exit(State<MinersWife> nextState)
    {
        shouldGoToToilet = false;
        _owner.StopCoroutine(shouldpeepee);

        if (nextState.GetType() == typeof(VisitToilet)) 
            _owner.SetTextBox("화장실을 가야겠어.");
        else if (nextState.GetType() == typeof(MakingStew))
            _owner.SetTextBox("");
    }

    private IEnumerator PeePeeCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            float prob = Random.Range(0f, 1f);
            Debug.Log("peepee probability:" + prob + " < 0.1f ? " + (prob < 0.1f));
            if (prob < 0.1f)
            {
                shouldGoToToilet = true;
                break;
            }
        }
        Debug.Log("PeePeeCoroutine Out ");

    }


}
