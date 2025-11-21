using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class MainMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    public List<TextMeshProUGUI> buttonTexts;
    public List<GameObject> buttonObjects;
    public GameObject mainMenuBackground;

    [Header("Cutscene Object")]
    public GameObject cutsceneObject;

    [Header("Animation Settings")]
    public float colorTransitionDuration = 0.5f;
    public float delayBetweenCycles = 0.5f;
    public float fadeOutDuration = 0.5f;
    [Tooltip("المدة التي تستغرقها الخلفية للرجوع للخلف")]
    public float backgroundMoveDuration = 1.0f; // <-- **إضافة جديدة: مدة حركة الخلفية**

    [Header("Audio Settings")]
    public AudioClip hoverSound;

    private List<Color> targetColors;
    private AudioSource audioSource;
    private bool isStartingGame = false;

    void Start()
    {
        if (cutsceneObject != null) cutsceneObject.SetActive(false);
        if (mainMenuBackground != null) mainMenuBackground.SetActive(true);
        audioSource = GetComponent<AudioSource>();

        // (بقية دالة Start كما هي)
        foreach (var btnObject in buttonObjects)
        {
            EventTrigger trigger = btnObject.GetComponent<EventTrigger>();
            if (trigger == null) trigger = btnObject.AddComponent<EventTrigger>();
            EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
            pointerEnter.eventID = EventTriggerType.PointerEnter;
            pointerEnter.callback.AddListener((data) => { PlayHoverSound(); });
            trigger.triggers.Add(pointerEnter);
        }
        if (buttonTexts != null && buttonTexts.Count > 0)
        {
            targetColors = new List<Color>();
            foreach (var txt in buttonTexts) { targetColors.Add(txt.color); }
            StartCoroutine(SmoothColorCycleRoutine());
        }
    }

    // --- (دالة SmoothColorCycleRoutine لم تتغير) ---
    private IEnumerator SmoothColorCycleRoutine()
    {
        while (true)
        {
            if (isStartingGame) yield break;
            // ... (الكود هنا لم يتغير)
            Color lastColor = targetColors[targetColors.Count - 1];
            for (int i = targetColors.Count - 1; i > 0; i--) { targetColors[i] = targetColors[i - 1]; }
            targetColors[0] = lastColor;
            float elapsedTime = 0f;
            List<Color> startingColors = new List<Color>();
            foreach (var txt in buttonTexts) { startingColors.Add(txt.color); }
            while (elapsedTime < colorTransitionDuration)
            {
                if (isStartingGame) yield break;
                float t = elapsedTime / colorTransitionDuration;
                for (int i = 0; i < buttonTexts.Count; i++) { buttonTexts[i].color = Color.Lerp(startingColors[i], targetColors[i], t); }
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            for (int i = 0; i < buttonTexts.Count; i++) { buttonTexts[i].color = targetColors[i]; }
            yield return new WaitForSeconds(delayBetweenCycles);
        }
    }

    public void StartGame()
    {
        if (isStartingGame) return;
        isStartingGame = true;
        StartCoroutine(TransitionToCutscene()); // استدعاء الـ Coroutine الجديد
    }

    // --- **تم تعديل هذا الـ Coroutine بالكامل** ---
    private IEnumerator TransitionToCutscene()
    {
        // --- الجزء الأول: تلاشي الأزرار ---
        List<CanvasGroup> buttonCGs = new List<CanvasGroup>();
        foreach (var btnObject in buttonObjects)
        {
            CanvasGroup cg = btnObject.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.interactable = false;
                buttonCGs.Add(cg);
            }
        }
        float fadeElapsed = 0f;
        while (fadeElapsed < fadeOutDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, fadeElapsed / fadeOutDuration);
            foreach (var cg in buttonCGs) { cg.alpha = alpha; }
            fadeElapsed += Time.deltaTime;
            yield return null;
        }
        foreach (var btnObject in buttonObjects) { btnObject.SetActive(false); }

        // --- الجزء الثاني: حركة الخلفية للوراء ---
        if (mainMenuBackground != null)
        {
            RectTransform bgRect = mainMenuBackground.GetComponent<RectTransform>();
            Vector3 originalScale = bgRect.localScale;
            Vector3 targetScale = new Vector3(1, 1, 1); // الحجم الطبيعي (يفترض أن الفيديو بحجم 1)
            
            // يمكنك تعديل هذا الموضع ليتناسب مع موضع الفيديو في المشهد
            Vector3 originalPosition = bgRect.localPosition;
            Vector3 targetPosition = Vector3.zero; // الموضع المستهدف (يفترض أن الفيديو في المركز)

            float moveElapsed = 0f;
            while (moveElapsed < backgroundMoveDuration)
            {
                float t = moveElapsed / backgroundMoveDuration;
                // استخدام EaseOut لتكون الحركة أبطأ في النهاية
                t = 1 - Mathf.Pow(1 - t, 3);

                bgRect.localScale = Vector3.Lerp(originalScale, targetScale, t);
                bgRect.localPosition = Vector3.Lerp(originalPosition, targetPosition, t);
                
                moveElapsed += Time.deltaTime;
                yield return null;
            }
            // التأكد من وصولها للموضع والحجم النهائي
            bgRect.localScale = targetScale;
            bgRect.localPosition = targetPosition;
        }

        // --- الجزء الثالث: تشغيل الـ Cutscene وإخفاء الخلفية ---
        if (cutsceneObject != null)
        {
            Debug.Log("Starting Cutscene...");
            cutsceneObject.SetActive(true);
        }
        if (mainMenuBackground != null)
        {
            mainMenuBackground.SetActive(false); // إخفاء الصورة بعد أن أخذ الفيديو مكانها
        }
    }

    public void QuitGame()
    {
        if (isStartingGame) return;
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    private void PlayHoverSound()
    {
        if (hoverSound != null && !isStartingGame)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }
}
