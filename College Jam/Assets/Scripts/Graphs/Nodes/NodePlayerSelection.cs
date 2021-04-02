using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Graphs
{
    namespace Nodes
    {
        public class NodePlayerSelection : MonoBehaviour
        {
            NodeUI nodeUI;
            bool selected;

            int playerType, factionType;

            public TMP_Text playerTypeText, factionTypeText;


            // Start is called before the first frame update
            void Start()
            {
                nodeUI = GetComponent<NodeUI>();
            }

            public void Select(bool selected)
            {
                if (selected != this.selected)
                {
                    this.selected = selected;
                    nodeUI.SetDetailVisibility(this.selected);
                    if (this.selected)
                    {
                        playerTypeText.text = PlayerSettings.playerTypeMap[(PlayerSettings.PlayerType)playerType];
                        factionTypeText.text = PlayerSettings.factionTypeMap[(PlayerSettings.FactionType)factionType];
                        PlayerSelectManager.instance.RegisterPlayer(this);
                    }
                    else
                    {
                        playerType = 0;
                        factionType = 0;
                        PlayerSelectManager.instance.DeregisterPlayer(this);
                    }
                }
            }

            public void UpdatePlayerType(bool right)
            {
                playerType += (right) ? 1 : -1;
                if (playerType < 0)
                    playerType = PlayerSettings.PlayerTypeCount - 1;
                else if (playerType >= PlayerSettings.PlayerTypeCount)
                    playerType = 0;
                playerTypeText.text = PlayerSettings.playerTypeMap[(PlayerSettings.PlayerType)playerType];
            }

            public void UpdateFactionType(bool right)
            {
                factionType += (right) ? 1 : -1;
                if (factionType < 0)
                    factionType = PlayerSettings.FactionTypeCount - 1;
                else if (factionType >= PlayerSettings.FactionTypeCount)
                    factionType = 0;
                factionTypeText.text = PlayerSettings.factionTypeMap[(PlayerSettings.FactionType)factionType];
            }
        }
    }
}

public static class PlayerSettings
{
    public enum PlayerType { Human, AI };
    public static int PlayerTypeCount = 2;
    public enum FactionType { Standard };
    public static int FactionTypeCount = 1;


    public static Dictionary<PlayerType, string> playerTypeMap = new Dictionary<PlayerType, string>()
    {
        { PlayerType.Human, "Human" },
        { PlayerType.AI, "AI"},
    };

    public static Dictionary<FactionType, string> factionTypeMap = new Dictionary<FactionType, string>()
    {
        { FactionType.Standard, "Standard" },
    };
}