using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Graphs
{
    namespace Nodes
    {
        public class NodeSelection : MonoBehaviour
        {
            public enum MouseState
            {
                Normal,
                Highlighted,
                Pressed,
                Selected,
                Disabled,
            }

            private MouseState mouseState;

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

            public UnityEvent<Node> OnSelect;

            void Awake()
            {
                mouseState = MouseState.Normal;
                fresnelAmount = 0f;
                fresnelAmountLF = fresnelAmount;
                mat = GetComponent<MeshRenderer>().material;
                mat.SetColor("_Color", normalColor);
                mat.SetColor("_FresnelColor", Color.black);

                OnSelect = new UnityEvent<Node>();
            }

            void Update()
            {
                fresnelAmount = Mathf.Clamp01(fresnelAmount + Time.deltaTime / fadeDuration * (mouseState == MouseState.Selected ? 1 : -1));
                if (!Mathf.Approximately(fresnelAmountLF, fresnelAmount))
                {
                    mat.SetColor("_FresnelColor", Color.Lerp(Color.black, Color.white, fresnelAmount));
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    SetState(MouseState.Normal);
                }
            }

            public void SetState(MouseState state)
            {
                mouseState = state;
                switch (state)
                {
                    case MouseState.Normal:
                        mat.SetColor("_Highlight", normalColor);
                        mat.SetColor("_FresnelColor", Color.black);
                        break;
                    case MouseState.Highlighted:
                        mat.SetColor("_Highlight", highlightedColor);
                        break;
                    case MouseState.Pressed:
                        mat.SetColor("_Highlight", pressedColor);
                        break;
                    case MouseState.Selected:
                        mat.SetColor("_Highlight", selectedColor);
                        mat.SetColor("_FresnelColor", Color.white);
                        OnSelect?.Invoke(GetComponent<Node>());
                        break;
                    case MouseState.Disabled:
                        mat.SetColor("_Highlight", disabledColor);
                        break;
                    default:
                        break;
                }
            }

            private void OnMouseDown()
            {
                if (mouseState == MouseState.Highlighted)
                {
                    SetState(MouseState.Pressed);
                }
            }

            private void OnMouseEnter()
            {
                if (mouseState == MouseState.Normal)
                {
                    SetState(MouseState.Highlighted);
                }
            }

            private void OnMouseExit()
            {
                if (mouseState == MouseState.Highlighted || mouseState == MouseState.Pressed)
                {
                    SetState(MouseState.Normal);
                }
            }

            private void OnMouseUp()
            {
                if (mouseState == MouseState.Pressed)
                {
                    SetState(MouseState.Selected);
                }
            }
        }
    }
}