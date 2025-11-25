// UIManager.cs
using UnityEngine;
using UnityEngine.SceneManagement; // <-- مهم جداً لإدارة المشاهد

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    [Tooltip("اسحب هنا الـ Panel الخاص بشاشة الموت")]
    public GameObject deathScreenPanel;

    // دالة عامة لإظهار شاشة الموت
    public void ShowDeathScreen()
    {
        if (deathScreenPanel != null)
        {
            deathScreenPanel.SetActive(true);
        }
    }

    // دالة عامة لإعادة تحميل المشهد الحالي (لزر "إعادة")
    public void RestartLevel()
    {
        // SceneManager.GetActiveScene().buildIndex يعيد رقم المشهد الحالي
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // --- **الدالة الجديدة التي أضفناها** ---
    // دالة عامة للعودة إلى القائمة الرئيسية (لزر "خروج")
    public void ReturnToMainMenu()
    {
        // تأكد من أن اسم المشهد "MainMenu" مطابق تماماً للاسم في ملفات المشروع
        SceneManager.LoadScene("Main menu"); 
    }
    // ------------------------------------
}
