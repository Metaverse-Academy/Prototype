using UnityEngine;
using UnityEngine.Playables; // Add this

public class Timeline : MonoBehaviour
{
    [SerializeField] private string timelineName;
    [SerializeField] private PlayableDirector playableDirector; // Add this
    [SerializeField] private TMPro.TextMeshProUGUI currentmissionText;
    [SerializeField] private TMPro.TextMeshProUGUI nextmissionText;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayTimeline(timelineName);
        }
    }
    public void PlayTimeline(string timelineName)
    {
        Debug.Log("Playing timeline: " + timelineName);
        if (playableDirector != null)
        {
            playableDirector.Play();
            currentmissionText.gameObject.SetActive(false);
            nextmissionText.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("PlayableDirector not assigned!");
        }
    }
}