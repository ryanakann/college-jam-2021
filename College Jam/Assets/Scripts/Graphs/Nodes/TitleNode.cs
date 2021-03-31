using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graphs
{
    namespace Nodes
    {
        public class TitleNode : Node
        {
            float t;
            float tweenT;
            float width;
            float maxWidth;
            public float introTime = 1f;
            public AnimationCurve tweenCurve;

            private void Start()
            {
                t = 0;
                width = 0f;
                maxWidth = transform.localScale.x;
            }

            private void LateUpdate()
            {
                tweenT = tweenCurve.Evaluate(t);
                width = Mathf.Lerp(0f, maxWidth, tweenT);
                transform.localScale = Vector3.one * width;
                t += Time.deltaTime / introTime;
            }
        }
    }
}