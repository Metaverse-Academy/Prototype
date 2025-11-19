using UnityEngine;

[CreateAssetMenu(fileName = "EnemydataSO", menuName = "Scriptable Objects/EnemydataSO")]
public class EnemydataSO : ScriptableObject
{
    public float moveSpeed;
    public float chaseSpeed;
    public float attackRange;
    public float chaseRange;
    public int damage;
    public float attackCooldown;
    public int maxHealth;

    public EnemyBehaviourBaseSO brain;

}
