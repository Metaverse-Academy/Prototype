using UnityEngine;

public abstract class EnemyBehaviourBaseSO : ScriptableObject
{
    public abstract void Enter(EnemyBase enemy); // START
    public abstract void Tick(EnemyBase enemy); // UPDATE
    public abstract void F_Tick(EnemyBase enemy); // FIXED UPDATE
}
