using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;

public class OrderManager : MonoBehaviour
{
    [Header("Order Settings")]
    public List<GameObject> orders; // Assign all order objects manually in the inspector
    public Transform deliveryLocation; // Where to deliver the orders
    public Transform player;
    public float collectDistance = 2f;
    public float deliverDistance = 2f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private AudioClip deliverSound;
    [SerializeField] private GameObject vfxCollect;
    [SerializeField] private GameObject vfxDeliver;
    [SerializeField] private GameObject ordereffect;
    [SerializeField] private GameObject delevereffect;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject playerCharacter;
    [SerializeField] private GameObject playerHUD;





    [Header("UI")]
    public TextMeshProUGUI evidenceinofficetext;
    public TextMeshProUGUI evidenceincarrytext;
    [SerializeField] private TextMeshProUGUI wontext;
    private List<GameObject> carriedOrders = new List<GameObject>();
    private int deliveredOrdersCount = 0;
    public bool allOrdersDelivered = false;
    void Awake()
    {
        Instantiate(delevereffect, deliveryLocation.position + Vector3.up * 0.1f, Quaternion.identity);
    }

    void Update()
    {

        // Pick up orders
        foreach (GameObject order in orders)
        {
            if (order.activeSelf && Vector3.Distance(player.position, order.transform.position) < collectDistance)
            {
                carriedOrders.Add(order);
                order.SetActive(false);
                Debug.Log("Order collected! Carrying " + carriedOrders.Count + " orders.");
                audioSource.PlayOneShot(collectSound);
                ordereffect.GetComponent<ParticleSystem>().Stop();
                UpdateUI();
                if (vfxCollect != null)
                {
                    Instantiate(vfxCollect, order.transform.position, Quaternion.identity);
                }
            }

            // Deliver orders
            if (carriedOrders.Count > 0 && Vector3.Distance(player.position, deliveryLocation.position) < deliverDistance)
            {
                deliveredOrdersCount += carriedOrders.Count;
                carriedOrders.Clear();
                Debug.Log("Orders delivered! Total delivered: " + deliveredOrdersCount);
                audioSource.PlayOneShot(deliverSound);
                UpdateUI();
            }
        }

        void UpdateUI()
        {
            if (evidenceinofficetext != null)
            {
                evidenceinofficetext.text = "Bring back the evidence to the office ";


            }
            if (evidenceincarrytext != null)
            {
                evidenceincarrytext.text = "Evidence in Carry: " + carriedOrders.Count + " out of " + orders.Count;

                if (deliveredOrdersCount >= orders.Count)
                {
                    wontext.gameObject.SetActive(true);
                    evidenceincarrytext.gameObject.SetActive(false);
                    evidenceinofficetext.gameObject.SetActive(false);
                    if (videoPlayer != null)
                    {
                        playerHUD.SetActive(false);
                        playerCharacter.SetActive(false);
                        videoPlayer.Play();
                        videoPlayer.loopPointReached += OnCutsceneEnd;
                    }
                }
            }
        }

        
    }
     private void OnCutsceneEnd(VideoPlayer vp)
        {
            // Unsubscribe to prevent multiple calls
            vp.loopPointReached -= OnCutsceneEnd;

            // Stop the video player
            vp.Stop();
            allOrdersDelivered = true;

            // Re-enable player and potentially load next mission elements
            playerCharacter.SetActive(true);
            playerHUD.SetActive(true);
            // Optionally, activate elements for the next mission here
            Debug.Log("Cutscene ended. Proceeding to next mission phase.");
        }
}