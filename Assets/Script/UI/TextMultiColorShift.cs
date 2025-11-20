using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TextMultiColorShift : MonoBehaviour
{
    [Header("Target Text")]
    [Tooltip("اسحب هنا كائن النص الذي تريد تغيير لونه")]
    public TextMeshProUGUI textToChange; // <-- التغيير الأول: أصبح public

    [Header("Color Cycle Settings")]
    [Tooltip("قائمة الألوان التي سيمر بها النص بالترتيب")]
    public List<Color> colorCycle;

    [Tooltip("المدة التي يستغرقها الانتقال إلى اللون التالي")]
    public float transitionDuration = 1.0f;

    [Tooltip("المدة التي يبقى فيها اللون ثابتاً قبل الانتقال")]
    public float delayBetweenTransitions = 0.5f;

    private int currentColorIndex = 0;

    void Start()
    {
        // تم حذف السطر الذي كان هنا: textToChange = GetComponent<TextMeshProUGUI>();

        // التأكد من أنك قمت بسحب النص وأن هناك ألوان في القائمة
        if (textToChange != null && colorCycle != null && colorCycle.Count > 0)
        {
            textToChange.color = colorCycle[0];
            StartCoroutine(CycleThroughColors());
        }
        else
        {
            // رسالة خطأ أوضح الآن
            Debug.LogWarning("الرجاء التأكد من سحب كائن النص (Text) إلى الخانة المخصصة وإضافة لون واحد على الأقل في القائمة.");
        }
    }

    private IEnumerator CycleThroughColors()
    {
        while (true)
        {
            Color startColor = textToChange.color;
            currentColorIndex = (currentColorIndex + 1) % colorCycle.Count;
            Color endColor = colorCycle[currentColorIndex];

            float elapsedTime = 0f;
            while (elapsedTime < transitionDuration)
            {
                float t = elapsedTime / transitionDuration;
                textToChange.color = Color.Lerp(startColor, endColor, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            textToChange.color = endColor;
            yield return new WaitForSeconds(delayBetweenTransitions);
        }
    }
}
