using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TaskManager : MonoBehaviour
{
    [SerializeField] private List<string> tasks = new List<string>();
    [SerializeField] private TextMeshProUGUI taskListText;
    [SerializeField] private EquipmentManager equipmentManager;
    private string currentTask;
    private int currentTaskIndex = 0;

    private void UpdateTaskListUI()
    {
        currentTask = tasks[currentTaskIndex];
        taskListText.text = "Current Task: " + currentTask;
        currentTaskIndex++;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeTasks();
    }

    private void InitializeTasks()
    {
        tasks.Add("Mission: Scout homes for Evidence boxes and bring them back to office");
    }

    // Update is called once per frame
    void Update()
    {
        if(equipmentManager.currentWeapon != null)
        {
            UpdateTaskListUI();
        }
    }
}
