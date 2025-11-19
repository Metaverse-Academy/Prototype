using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [Header("Teleporter Settings")]
    [Tooltip("اسحب هنا الكائن الفارغ الذي يمثل نقطة الوصول")]
    public Transform destination;

    // لم نعد بحاجة إلى objectToTeleport هنا

    private void OnTriggerEnter(Collider other)
    {
        // تحقق من أن الوجهة موجودة
        if (destination == null)
        {
            Debug.LogError("لم يتم تحديد الوجهة (Destination)!", this);
            return;
        }

        // تحقق مما إذا كان الكائن الذي دخل لديه Rigidbody (للتأكد من أنه كائن متحرك وليس جداراً)
        if (other.GetComponent<Rigidbody>() != null)
        {
            Debug.Log(other.name + " entered the portal. Teleporting...");

            // --- عملية النقل ---
            other.transform.position = destination.position;
            other.transform.rotation = destination.rotation;
        }
    }
}
