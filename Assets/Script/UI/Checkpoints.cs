using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    [SerializeField] private GameObject checkpoint1Prefab;
    [SerializeField] private GameObject checkpoint2Prefab;
    [SerializeField] private GameObject checkpoint3Prefab;
    [SerializeField] private GameObject checkpoint4Prefab;
    [SerializeField] private GameObject checkpointtrigger1;
    [SerializeField] private GameObject checkpointtrigger2;
    [SerializeField] private GameObject checkpointtrigger3;
    private GameObject currentCheckpoint;

    void Start()
    {
        currentCheckpoint = checkpoint1Prefab;
        UpdateCheckpoint();
    }

    void Update()
    {
//        Debug.Log("Current Checkpoint: " + currentCheckpoint.name);
    }

    public void AdvanceCheckpoint(GameObject triggered)
    {
        if (currentCheckpoint == checkpoint1Prefab && triggered == checkpointtrigger1)
        {
            currentCheckpoint = checkpoint2Prefab;
        }
        else if (currentCheckpoint == checkpoint2Prefab && triggered == checkpointtrigger2)
        {
            currentCheckpoint = checkpoint3Prefab;
        }
        else if (currentCheckpoint == checkpoint3Prefab && triggered == checkpointtrigger3)
        {
            currentCheckpoint = checkpoint4Prefab;
        }
        else if (currentCheckpoint == checkpoint4Prefab)
        {
            Debug.Log("All checkpoints completed!");
        }
        UpdateCheckpoint();
    }

    private void UpdateCheckpoint()
    {
        checkpoint1Prefab.SetActive(currentCheckpoint == checkpoint1Prefab);
        checkpoint2Prefab.SetActive(currentCheckpoint == checkpoint2Prefab);
        checkpoint3Prefab.SetActive(currentCheckpoint == checkpoint3Prefab);
        checkpoint4Prefab.SetActive(currentCheckpoint == checkpoint4Prefab);

        checkpointtrigger1.SetActive(currentCheckpoint == checkpoint1Prefab);
        checkpointtrigger2.SetActive(currentCheckpoint == checkpoint2Prefab);
        checkpointtrigger3.SetActive(currentCheckpoint == checkpoint3Prefab);
    }
}