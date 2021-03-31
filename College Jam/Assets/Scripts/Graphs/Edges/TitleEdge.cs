using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graphs
{
    namespace Edges
    {
        public class TitleEdge : Edge
        {
            float t;
            float tweenT;
            float width;
            float maxLength;
            public float introTime = 1f;
            public AnimationCurve tweenCurve;

            private void Start()
            {
                t = 0;
            }

            private void LateUpdate()
            {
                tweenT = tweenCurve.Evaluate(t);
                width = Mathf.Lerp(0f, edgeWidth, tweenT);
                maxLength = (node1.transform.position - node2.transform.position).magnitude / 2f;
                transform.localScale = new Vector3(edgeWidth, Mathf.Lerp(0f, maxLength, tweenT), edgeWidth);
                t += Time.deltaTime / introTime;
            }
        }
    }
}