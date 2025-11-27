using UnityEngine;
using UnityEngine.SceneManagement;

public class TimelineEndMenuLoader : MonoBehaviour
{
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Main menu"); // Use your main menu scene name
    }
}
