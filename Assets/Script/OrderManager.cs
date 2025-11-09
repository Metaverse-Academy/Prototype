using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class OrderManager : MonoBehaviour
{
    [Header("Order Settings")]
    public List<GameObject> orders; // Assign all order objects manually in the inspector
    public Transform deliveryLocation; // Where to deliver the orders
    public Transform player;
    public float collectDistance = 2f;
    public float deliverDistance = 2f;

    [Header("UI")]
    public TextMeshProUGUI evidenceinofficetext; // Assign a UI Text element in the inspector
    public TextMeshProUGUI evidenceincarrytext; // Assign a UI Text element in the inspector
    private List<GameObject> carriedOrders = new List<GameObject>();
    private int deliveredOrdersCount = 0;

    void FixedUpdate()
    {
        UpdateUI();
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
            }
        }

        // Deliver orders
        if (carriedOrders.Count > 0 && Vector3.Distance(player.position, deliveryLocation.position) < deliverDistance)
        {
            deliveredOrdersCount += carriedOrders.Count;
            carriedOrders.Clear();
            Debug.Log("Orders delivered! Total delivered: " + deliveredOrdersCount);
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        if (evidenceinofficetext != null)
        {
            evidenceinofficetext.text = "Evidence in Office: " + deliveredOrdersCount;
        }
        if (evidenceincarrytext != null)
        {
            evidenceincarrytext.text = "Evidence in Carry: " + carriedOrders.Count;
        }
    }
}
