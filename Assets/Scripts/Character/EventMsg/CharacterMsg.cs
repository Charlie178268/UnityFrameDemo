public enum CharacterEvent
{
    Initial = ManagerId.CharacterManager+1,
    Idle,
    walk,
    Jump,
    Run,
    Attack,
    AttackBig,
    Die,
    joyStickBegin,
    joyStick,
    joyStickEnd,
    looseBlood,
    MaxValue
}

public enum CharacterEventTwo
{
    Initial = CharacterEvent.MaxValue+1,
    ChangeMode,
    MaxValue
}