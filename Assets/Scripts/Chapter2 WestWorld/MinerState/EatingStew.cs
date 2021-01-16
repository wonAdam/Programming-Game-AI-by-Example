using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatingStew : State<Miner>
{
    public EatingStew(Miner miner) : base(miner) { }

    public override void Enter(State<Miner> previousState)
    {
        _owner.transform.position = _owner.TablePos;
        _owner.SetTextBox("잘 먹겠습니다.");
    }
    public override void Execute()
    {
        base.Execute();

        _owner.SetTextBox("냠냠냠");

        _owner.fatigue = Mathf.Max(_owner.fatigue - Time.deltaTime, 0f);

        if (_owner.RestWell() && DelayedEnough())
            _owner.StateMachine.ChangeState(new EnterMineAndDigForNugget(_owner));
    }

    public override void Exit(State<Miner> nextState)
    {
        _owner.SetTextBox("잘 먹었다.");
    }

    public override bool OnMessage(MessageType type, IMessageSender sender)
    {
        switch (type)
        {
            default:
                return false;
        }
    }
}
