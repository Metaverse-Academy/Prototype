using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public Transform Target;
    [SerializeField] private EnemydataSO enemyData;
    private Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Target = GameObject.FindGameObjectWithTag("Player").transform;

    }
    private void Start()
    {
        enemyData.brain.Enter(this);

    }
    private void Update()
    {
        enemyData.brain.Tick(this);
    }
    private void FixedUpdate()
    {
        enemyData.brain.F_Tick(this);
    }
}
