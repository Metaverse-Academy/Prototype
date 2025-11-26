using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    public Checkpoints checkpointsManager;
    public GameObject thisTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            checkpointsManager.AdvanceCheckpoint(thisTrigger);
            gameObject.SetActive(false); // Disable this trigger after use
        }
    }
}