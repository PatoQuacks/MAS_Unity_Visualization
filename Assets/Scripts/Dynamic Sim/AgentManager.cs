using System;
using TMPro;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    [SerializeField] private GameObject agentPrefab;

    private string[] agentList;

    // Retrieval of data needed to generate AGVs
    void OnEnable()
    {
        NetworkReceiver.OnAgentListReceived += HandleAgentData;
    }

    void OnDisable()
    {
        NetworkReceiver.OnAgentListReceived -= HandleAgentData;
    }

    void HandleAgentData(string[] _agentList)
    {
        agentList = _agentList;
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        setAgents();
    }

    void setAgents()
    {
        foreach(string agentName in agentList)
        {
            // Creates the prefab and sets it as a child of the object running this script
            GameObject newAgent = Instantiate(agentPrefab, transform);
            newAgent.transform.localPosition = Vector3.zero;
            newAgent.name = agentName;
            /*
            // Set the # of the agent in model
            newAgent.GetComponentInChildren<TextMeshProUGUI>().text = $"{agentName[agentName.Length - 1]}";
            */
            Debug.Log($"Setup for {agentName}");
        }
    }

}
