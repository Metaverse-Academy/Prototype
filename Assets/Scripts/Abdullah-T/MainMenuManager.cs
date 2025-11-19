using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MainMenuManager : MonoBehaviour
{
    [Header("Buttons")]
    [Tooltip("ضع هنا صور الأزرار بالترتيب من الأعلى للأسفل")]
    public List<Image> buttonImages;

    [Header("Animation Settings")]
    [Tooltip("المدة التي يستغرقها اللون للانتقال من زر إلى آخر")]
    public float transitionDuration = 0.5f;

    [Tooltip("المدة التي ينتظرها بعد اكتمال الانتقال قبل بدء الانتقال التالي")]
    public float delayBetweenCycles = 0.5f;

    private List<Color> targetColors; // قائمة لحفظ الألوان المستهدفة لكل زر

    void Start()
    {
        if (buttonImages != null && buttonImages.Count > 0)
        {
            // تهيئة قائمة الألوان المستهدفة بنفس الألوان الحالية
            targetColors = new List<Color>();
            foreach (var img in buttonImages)
            {
                targetColors.Add(img.color);
            }

            StartCoroutine(SmoothColorCycleRoutine());
        }
    }

    private IEnumerator SmoothColorCycleRoutine()
    {
        while (true)
        {
            // --- الخطوة الأولى: تحديد الألوان الجديدة ---
            
            // احفظ لون الزر الأخير
            Color lastColor = targetColors[targetColors.Count - 1];

            // أزح الألوان في قائمة الألوان المستهدفة
            for (int i = targetColors.Count - 1; i > 0; i--)
            {
                targetColors[i] = targetColors[i - 1];
            }
            // ضع لون الزر الأخير في بداية القائمة
            targetColors[0] = lastColor;


            // --- الخطوة الثانية: تنفيذ الانتقال السلس للألوان ---
            float elapsedTime = 0f;
            
            // حفظ الألوان الحالية قبل بدء الانتقال
            List<Color> startingColors = new List<Color>();
            foreach (var img in buttonImages)
            {
                startingColors.Add(img.color);
            }

            // حلقة الانتقال التدريجي
            while (elapsedTime < transitionDuration)
            {
                // حساب نسبة التقدم (من 0 إلى 1)
                float t = elapsedTime / transitionDuration;

                // تحديث لون كل زر باستخدام Lerp للانتقال من لونه الابتدائي إلى لونه المستهدف
                for (int i = 0; i < buttonImages.Count; i++)
                {
                    buttonImages[i].color = Color.Lerp(startingColors[i], targetColors[i], t);
                }

                // زيادة الوقت المنقضي والانتظار للإطار التالي
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // التأكد من أن جميع الأزرار وصلت إلى لونها المستهدف بالضبط
            for (int i = 0; i < buttonImages.Count; i++)
            {
                buttonImages[i].color = targetColors[i];
            }

            // --- الخطوة الثالثة: الانتظار قبل بدء الدورة التالية ---
            yield return new WaitForSeconds(delayBetweenCycles);
        }
    }
}
