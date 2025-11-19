using UnityEngine;

public class Rotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("سرعة الدوران بالدرجات في الثانية.")]
    public float rotationSpeed = 50f;

    [Tooltip("محور الدوران. (0, 1, 0) يعني الدوران حول المحور العمودي Y.")]
    public Vector3 rotationAxis = new Vector3(0, 1, 0);

    // يتم استدعاء هذه الدالة مرة واحدة في كل إطار
    void Update()
    {
        // قم بتدوير الكائن حول محوره المحدد بالسرعة المحددة
        // نضرب بـ Time.deltaTime لضمان أن الدوران سلس ومستقل عن معدل الإطارات
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
    }
}
