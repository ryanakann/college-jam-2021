using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class NamedColor
{
    public string name;
    public Color color;
}

[System.Serializable]
public class MapSize
{
    public string label;
    public int nodeCount;
}

public class GameSettings : MonoBehaviour
{
    public static GameSettings instance;

    [Header("Map Size")]
    public List<MapSize> sizes;
    [HideInInspector]
    public MapSize mapSize;
    public TMP_Dropdown mapSizeDropdown;

    [Header("Player Count")]
    public Vector2Int playerCountRange;
    public int playerCount;
    public List<Player> players;
    public TMP_Dropdown playerCountDropdown;

    [Header("Player Info")]
    public List<NamedColor> colors;
    public TMP_Dropdown playerDropdown;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        List<string> options = new List<string>();

        mapSizeDropdown.ClearOptions();
        options.Clear();
        sizes.ForEach(x => options.Add(x.label));
        mapSizeDropdown.AddOptions(options);

        playerCountDropdown.ClearOptions();
        options.Clear();
        for (int i = playerCountRange.x; i <= playerCountRange.y; i++)
        {
            options.Add(i.ToString());
        }
        playerCountDropdown.AddOptions(options);

        mapSizeDropdown.onValueChanged.AddListener(UpdateMapSize);
        playerCountDropdown.onValueChanged.AddListener(UpdatePlayerCount);
        mapSizeDropdown.onValueChanged.Invoke(mapSizeDropdown.value);
        playerCountDropdown.onValueChanged.Invoke(playerCountDropdown.value);
    }

    public void UpdateMapSize(int index)
    {
        mapSize = sizes[index];
    }

    public void UpdatePlayerCount(int index)
    {
        playerCount = playerCountRange.x + index;
    }

    private void OnGUI()
    {
        string text = $"Map size: {mapSize.label} \t Player count: {playerCount}";
        GUI.Label(
           new Rect(
               5,                   // x, left offset
               Screen.height - 150, // y, bottom offset
               300f,                // width
               150f                 // height
           ),
           text,             // the display text
           GUI.skin.textArea        // use a multi-line text area
        );
    }
}
