using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuenchThirst : State<Miner>
{
    public QuenchThirst(Miner miner) : base(miner) { }

    public override void Enter(State<Miner> previousState)
    {
        _owner.transform.position = _owner.TavernPos;
        _owner.SetTextBox("목이 마르군! 술집으로 걸어간다.");

    }

    public override void Execute()
    {
        base.Execute();

        _owner.thirst = Mathf.Max(_owner.thirst - Time.deltaTime * 3f, 0f);

        _owner.SetTextBox("저게 홀짝 홀짝 마시기에 대단히 좋은 술이군.");


        if (DelayedEnough() && _owner.QuenchyEnough())
            _owner.StateMachine.ChangeState(new EnterMineAndDigForNugget(_owner));
    }

    public override void Exit(State<Miner> nextState)
    {
    }
}
