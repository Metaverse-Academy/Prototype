using UnityEngine;
using UnityEngine.SceneManagement;

public class TimelineCredits : MonoBehaviour
{
    public void ReturnToCredits()
    {
        SceneManager.LoadScene("CreditScene"); // Use your main menu scene name
    }
}
