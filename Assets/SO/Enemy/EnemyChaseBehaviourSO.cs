using UnityEngine;

[CreateAssetMenu(fileName = "EnemyChaseBehaviourSO", menuName = "Scriptable Objects/EnemyChaseBehaviourSO")]
public class EnemyChaseBehaviourSO : EnemyBehaviourBaseSO
{
    [SerializeField] private float chaseRange = 3f;

    public override void Enter(EnemyBase enemy)
    {
        // Implementation for Enter
    }

    public override void Tick(EnemyBase enemy)
    {
        // Implementation for Tick
    }

    public override void F_Tick(EnemyBase enemy)
    {
        float distanceToTarget = Vector3.Distance(enemy.Target.position, enemy.transform.position);
        if (distanceToTarget <= chaseRange)
        {
            Vector3 direction = (enemy.Target.position - enemy.transform.position).normalized;
            enemy.transform.position += direction * enemy.GetComponent<EnemydataSO>().chaseSpeed * Time.fixedDeltaTime;
        }
    }
}