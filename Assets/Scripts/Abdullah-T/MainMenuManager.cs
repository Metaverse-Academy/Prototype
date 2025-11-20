using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems; // <-- **مهم جداً:** أضفنا هذه المكتبة للأحداث

[RequireComponent(typeof(AudioSource))] // <-- يضمن وجود مكون الصوت
public class MainMenuManager : MonoBehaviour
{
    [Header("Button Texts")]
    public List<TextMeshProUGUI> buttonTexts;

    [Header("Animation Settings")]
    public float transitionDuration = 0.5f;
    public float delayBetweenCycles = 0.5f;

    [Header("Scene Navigation")]
    public string gameSceneName = "درافت ماب";

    [Header("Audio Settings")] // <-- **قسم جديد لإعدادات الصوت**
    [Tooltip("الصوت الذي سيتم تشغيله عند مرور الفأرة فوق أي زر")]
    public AudioClip hoverSound;

    private List<Color> targetColors;
    private AudioSource audioSource; // متغير لحفظ مكون الصوت
    public string cutsceneSceneName = "Cutscene1";

    void Start()
    {
        // الحصول على مكون الصوت تلقائياً
        audioSource = GetComponent<AudioSource>();

        // --- إضافة "مستمع" لكل زر لمعرفة متى يمر المؤشر فوقه ---
        foreach (var txt in buttonTexts)
        {
            // نصل إلى كائن الزر الأب للنص
            Button parentButton = txt.GetComponentInParent<Button>();
            if (parentButton != null)
            {
                // إضافة مكون EventTrigger إذا لم يكن موجوداً
                EventTrigger trigger = parentButton.gameObject.GetComponent<EventTrigger>();
                if (trigger == null)
                {
                    trigger = parentButton.gameObject.AddComponent<EventTrigger>();
                }

                // إنشاء حدث "دخول المؤشر"
                EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
                pointerEnter.eventID = EventTriggerType.PointerEnter;
                // ربط الحدث بدالة تشغيل الصوت
                pointerEnter.callback.AddListener((data) => { PlayHoverSound(); });
                
                // إضافة الحدث إلى قائمة الأحداث
                trigger.triggers.Add(pointerEnter);
            }
        }

        // بدء تأثير الألوان
        if (buttonTexts != null && buttonTexts.Count > 0)
        {
            targetColors = new List<Color>();
            foreach (var txt in buttonTexts)
            {
                targetColors.Add(txt.color);
            }
            StartCoroutine(SmoothColorCycleRoutine());
        }
    }

    // --- (كود تأثير الألوان يبقى كما هو) ---
    private IEnumerator SmoothColorCycleRoutine()
    {
        // ... الكود هنا لم يتغير ...
        while (true)
        {
            Color lastColor = targetColors[targetColors.Count - 1];
            for (int i = targetColors.Count - 1; i > 0; i--)
            {
                targetColors[i] = targetColors[i - 1];
            }
            targetColors[0] = lastColor;

            float elapsedTime = 0f;
            List<Color> startingColors = new List<Color>();
            foreach (var txt in buttonTexts)
            {
                startingColors.Add(txt.color);
            }

            while (elapsedTime < transitionDuration)
            {
                float t = elapsedTime / transitionDuration;
                for (int i = 0; i < buttonTexts.Count; i++)
                {
                    buttonTexts[i].color = Color.Lerp(startingColors[i], targetColors[i], t);
                }
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            for (int i = 0; i < buttonTexts.Count; i++)
            {
                buttonTexts[i].color = targetColors[i];
            }

            yield return new WaitForSeconds(delayBetweenCycles);
        }
    }

    // --- (دوال التحكم بالأزرار تبقى كما هي) ---
    public void StartGame()
    {
        SceneManager.LoadScene(cutsceneSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    // --- **الدالة الجديدة التي أضفناها لتشغيل الصوت** ---
    private void PlayHoverSound()
    {
        // تأكد من وجود مقطع صوتي قبل محاولة تشغيله
        if (hoverSound != null)
        {
            // PlayOneShot يسمح بتشغيل الصوت عدة مرات فوق بعضها
            // وهو مثالي لأصوات الواجهة
            audioSource.PlayOneShot(hoverSound);
        }
    }
}
