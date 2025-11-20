using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextColorShift : MonoBehaviour
{
    private TextMeshProUGUI textToChange;

    [Tooltip("سرعة تغير ألوان الطيف")]
    public float rainbowSpeed = 0.5f;

    void Start()
    {
        textToChange = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        // Color.HSVToRGB يأخذ 3 قيم (Hue, Saturation, Value) ويعيد لوناً.
        // بتغيير قيمة الـ Hue (درجة اللون) مع مرور الوقت، يمكننا الدوران عبر جميع ألوان الطيف.
        
        // Time.time * rainbowSpeed يجعل الدوران مستمراً.
        // عامل القسمة المتبقية (Modulo) % 1f يضمن أن القيمة تبقى دائماً بين 0 و 1.
        float hue = (Time.time * rainbowSpeed) % 1f;
        
        // نستخدم Saturation = 1 و Value = 1 للحصول على ألوان زاهية ومشرقة.
        // يمكنك تعديل هذه القيم للحصول على ألوان باهتة أو أغمق.
        Color newColor = Color.HSVToRGB(hue, 1f, 1f);

        textToChange.color = newColor;
    }
}
