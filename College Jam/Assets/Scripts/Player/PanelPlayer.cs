using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PanelPlayer : MonoBehaviour
{
    public static HashSet<int> takenColors;

    public TMP_Dropdown typeDropdown;
    public TMP_Dropdown factionDropdown;
    public TMP_Dropdown colorDropdown;

    public bool isHuman;
    public int factionIndex;
    public int colorIndex;

    private void Start()
    {
        List<string> options = new List<string>();

        factionDropdown.ClearOptions();
        options.Clear();
        Faction.factionList.ForEach(x => options.Add(x.name));
        factionDropdown.AddOptions(options);

        colorDropdown.ClearOptions();
        options.Clear();
        GameSettings.instance.colors.ForEach(x => options.Add(x.name));
        colorDropdown.AddOptions(options);

        SetType(typeDropdown.value);
        SetFaction(factionDropdown.value);

        if (takenColors == null)
        {
            takenColors = new HashSet<int>();
        }
        colorIndex = colorDropdown.value;
        if (takenColors.Contains(colorDropdown.value))
        {
            int index = colorDropdown.value;
            int tries = 0;
            int count = colorDropdown.options.Count;
            while (tries < count)
            {
                index = (index + 1) % count;
                if (!takenColors.Contains(index))
                {
                    colorIndex = index;
                    break;
                }
                tries++;
            }
        }

        colorDropdown.SetValueWithoutNotify(colorIndex);
        takenColors.Add(colorIndex);
        
        typeDropdown.onValueChanged.AddListener(SetType);
        factionDropdown.onValueChanged.AddListener(SetFaction);
        colorDropdown.onValueChanged.AddListener(SetColor);
    }

    private void SetType(int value)
    {
        isHuman = typeDropdown.value == 0 ? true : false;
        //GameSettings.instance
    }

    private void SetFaction(int value)
    {
        factionIndex = value;
    }

    private void SetColor(int value)
    {
        if (takenColors.Contains(value))
        {
            colorDropdown.SetValueWithoutNotify(colorIndex);
        }
        else
        {
            if (colorIndex >= 0)
            {
                takenColors.Remove(colorIndex);
            }
            colorIndex = value;
            takenColors.Add(colorIndex);
        }
    }

    private void OnDestroy()
    {
        takenColors.Remove(colorIndex);
    }
}
