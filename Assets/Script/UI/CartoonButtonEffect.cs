using UnityEngine;
using UnityEngine.EventSystems; // <-- مهم جداً للتحكم بأحداث الفأرة على الواجهة
using System.Collections;

public class CartoonButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Animation Settings")]
    [Tooltip("مقدار تكبير الزر عند مرور الفأرة فوقه")]
    public float hoverScale = 1.1f;

    [Tooltip("مقدار تصغير الزر عند الضغط عليه")]
    public float clickScale = 0.9f;

    [Tooltip("سرعة تنفيذ التحريكات (كلما زادت القيمة، كان أسرع)")]
    public float animationSpeed = 10f;

    private Vector3 originalScale;
    private Vector3 targetScale;

    void Start()
    {
        // حفظ الحجم الأصلي للزر عند بدء اللعبة
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    void Update()
    {
        // تحديث حجم الزر بسلاسة نحو الحجم المستهدف في كل إطار
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
    }

    // --- هذه الدوال يتم استدعاؤها تلقائياً بفضل الواجهات (Interfaces) ---

    // يتم استدعاؤها عند دخول مؤشر الفأرة إلى منطقة الزر
    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * hoverScale;
        // يمكنك إضافة تأثير اهتزاز هنا إذا أردت
        StartCoroutine(ShakeEffect(0.1f, 0.02f));
    }

    // يتم استدعاؤها عند خروج مؤشر الفأرة من منطقة الزر
    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
    }

    // يتم استدعاؤها عند الضغط على الزر (لحظة النقر)
    public void OnPointerDown(PointerEventData eventData)
    {
        targetScale = originalScale * clickScale;
    }

    // يتم استدعاؤها عند رفع الإصبع عن زر الفأرة (بعد النقر)
    public void OnPointerUp(PointerEventData eventData)
    {
        targetScale = originalScale * hoverScale; // يعود لحجم التمرير طالما الفأرة فوقه
    }

    // دالة إضافية لعمل تأثير اهتزاز خفيف
    IEnumerator ShakeEffect(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null; // انتظر الإطار التالي
        }

        transform.localPosition = originalPos; // أعد الزر إلى مكانه الأصلي
    }
}
