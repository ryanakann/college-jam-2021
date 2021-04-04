using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Graphs
{
    namespace Nodes
    {
        public delegate void NodeEvent(Node node);

        public class NodeSelection : MonoBehaviour
        {
            public enum NodeState
            {
                Normal,
                Hovering,
                Pressed,
                Selected,
                Disabled,
                Highlighted,
            }

            private NodeState mouseState;

            public Material mat;

            public Color normalColor;
            public Color highlightedColor;
            public Color pressedColor;
            public Color selectedColor;
            public Color disabledColor;
            [Range(1f, 5f)] public float colorMultiplier = 1f;
            [Range(0f, 1f)] public float fadeDuration = 0.1f;
            [SerializeField] private float fresnelAmount;
            private float fresnelAmountLF;

            public NodeEvent OnSelect;

            void Awake()
            {
                mouseState = NodeState.Normal;
                fresnelAmount = 0f;
                fresnelAmountLF = fresnelAmount;
                mat = GetComponent<MeshRenderer>().material;
                mat.SetColor("_Color", normalColor);
                mat.SetColor("_FresnelColor", Color.black);

                // OnSelect = new UnityEvent<Node>();
            }

            void Update()
            {
                if (MenuManager.instance.menuOpen) return;

                fresnelAmount = Mathf.Clamp01(fresnelAmount + Time.deltaTime / fadeDuration * (mouseState == NodeState.Selected ? 1 : -1));
                if (!Mathf.Approximately(fresnelAmountLF, fresnelAmount))
                {
                    mat.SetColor("_FresnelColor", Color.Lerp(Color.black, Color.white, fresnelAmount));
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    SetState(NodeState.Normal);
                }
            }

            public void SetState(NodeState state)
            {
                if (MenuManager.instance.menuOpen) return;
                mouseState = state;
                switch (state)
                {
                    case NodeState.Normal:
                        mat.SetColor("_Highlight", normalColor);
                        mat.SetColor("_FresnelColor", Color.black);
                        break;
                    case NodeState.Hovering:
                        mat.SetColor("_Highlight", highlightedColor);
                        break;
                    case NodeState.Pressed:
                        mat.SetColor("_Highlight", pressedColor);
                        break;
                    case NodeState.Selected:
                        mat.SetColor("_Highlight", selectedColor);
                        mat.SetColor("_FresnelColor", Color.white);
                        // OnSelect?.Invoke(GetComponent<Node>());
                        // PlayerController.instance.HandleClickNode(GetComponent<Node>());
                        break;
                    case NodeState.Highlighted:
                        mat.SetColor("_Highlight", selectedColor);
                        mat.SetColor("_FresnelColor", Color.white);
                        break;
                    case NodeState.Disabled:
                        mat.SetColor("_Highlight", disabledColor);
                        break;
                    default:
                        break;
                }
            }

            /*
            private void OnMouseDown()
            {
                if (mouseState == NodeState.Hovering)
                {
                    SetState(NodeState.Pressed);
                }
            }
            */

            private void OnMouseEnter()
            {
                if (mouseState == NodeState.Normal)
                {
                    SetState(NodeState.Hovering);
                }
            }

            private void OnMouseExit()
            {
                if (mouseState == NodeState.Hovering || mouseState == NodeState.Pressed)
                {
                    SetState(NodeState.Normal);
                }
            }

            private void OnMouseUpAsButton()
            {
                PlayerController.instance.HandleClickNode(GetComponent<Node>());
                // SetState(NodeState.Selected);
            }
        }
    }
}