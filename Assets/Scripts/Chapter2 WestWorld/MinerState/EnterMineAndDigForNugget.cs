using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterMineAndDigForNugget : State<Miner>
{
    public EnterMineAndDigForNugget(Miner miner) : base(miner) { }

    public override void Enter(State<Miner> previousState)
    {
        _owner.transform.position = _owner.MinePos;
        _owner.SetTextBox("금광으로 걸어간다.");

    }

    public override void Execute()
    {
        base.Execute();

        _owner.goldCarried += Time.deltaTime;
        _owner.fatigue += Time.deltaTime;

        _owner.SetTextBox("금덩어리를 집는다.");

        if (_owner.PocketFull() && DelayedEnough())
            _owner.StateMachine.ChangeState(new VisitBankAndDepositGold(_owner));

        if(_owner.Thirsty() && DelayedEnough())
            _owner.StateMachine.ChangeState(new QuenchThirst(_owner));

    }

    public override void Exit(State<Miner> nextState)
    {
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
