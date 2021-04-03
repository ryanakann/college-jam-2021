using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUIList : MonoBehaviour
{
    public GameObject playerUIPrefab;
    public List<GameObject> playerObjects;
    public TMP_Dropdown dropdown;
    private int playerCount;

    // Start is called before the first frame update
    void Start()
    {
        playerObjects = new List<GameObject>();
        playerCount = 0;
        dropdown.onValueChanged.AddListener(SetPlayerCount);
        SetPlayerCount(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPlayerCount(int index)
    {
        int difference = (index + 2) - playerCount;
        playerCount += difference;
        if (difference > 0)
        {
            for (int i = 0; i < difference; i++)
            {
                playerObjects.Add(Instantiate(playerUIPrefab, transform));
            }
        }
        else if (difference < 0)
        {
            for (int i = 0; i < -difference; i++)
            {
                GameObject toRemove = playerObjects[playerObjects.Count - 1];
                playerObjects.RemoveAt(playerObjects.Count - 1);
                Destroy(toRemove);
            }
        }
    }

    public void SendPlayerInfo()
    {
        List<Player> players = new List<Player>();
        foreach (var playerObj in playerObjects)
        {
            PanelPlayer panelPlayer = playerObj.GetComponent<PanelPlayer>();
            Faction faction = Faction.factionList[panelPlayer.factionIndex];
            Color color = GameSettings.instance.colors[panelPlayer.colorIndex].color;
            bool isHuman = panelPlayer.typeDropdown.value == 0 ? true : false;
            Player player = new Player(faction, color, isHuman);
            players.Add(player);
        }
        GameSettings.instance.players = players;
    }
}
