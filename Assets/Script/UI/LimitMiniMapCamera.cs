using UnityEngine;

public class LimitMiniMapCamera : MonoBehaviour
{
    public GameObject player;
    public GameObject playeronBike;

    private void LateUpdate()
    {
        if (playeronBike.activeInHierarchy)
        {
            transform.position = new Vector3(playeronBike.transform.position.x, 40, playeronBike.transform.position.z);
        }
        else
        {
            transform.position = new Vector3(player.transform.position.x, 40, player.transform.position.z);
        }
    }
}
