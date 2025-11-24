using UnityEngine;

public class LimitMiniMapCamera : MonoBehaviour
{
    public GameObject player;
    public GameObject Bike;

    private void LateUpdate()
    {
        if (Bike.activeInHierarchy)
        {
            transform.position = new Vector3(Bike.transform.position.x, 40, Bike.transform.position.z);
            return;
        }
        transform.position = new Vector3(player.transform.position.x, 40, player.transform.position.z);
    }
}
