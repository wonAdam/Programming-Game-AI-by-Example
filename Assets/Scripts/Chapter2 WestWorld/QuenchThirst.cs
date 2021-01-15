using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuenchThirst : State<Miner>
{
    Miner _miner;
    public QuenchThirst(Miner miner)
    {
        _miner = miner;
    }
    public override void Enter()
    {
        _miner.transform.position = _miner.TavernPos;
        _miner.SetTextBox("목이 마르군! 술집으로 걸어간다.");

    }

    public override void Execute()
    {
        base.Execute();

        _miner.thirst = Mathf.Max(_miner.thirst - Time.deltaTime * 3f, 0f);

        _miner.SetTextBox("저게 홀짝 홀짝 마시기에 대단히 좋은 술이군.");


        if (DelayedEnough() && _miner.QuenchyEnough())
            _miner.StateMachine.ChangeState(new EnterMineAndDigForNugget(_miner));
    }

    public override void Exit()
    {
    }
}
