using System;
using UnityEngine;

namespace Rito.Conveniences
{
    [Serializable]
    public class TransformData : IJsonData
    {
        public Vector3 localPosition;
        public Quaternion localRotation;
        public Vector3 localScale;

        public Vector3 globalPosition;
        public Quaternion globalRotation;
        public Vector3 globalScale;

        public TransformData() { }

        // 생성자
        public TransformData(in Transform transform)
        {
            localPosition = transform.localPosition;
            localRotation = transform.localRotation;
            localScale = transform.localScale;

            globalPosition = transform.position;
            globalRotation = transform.rotation;
            globalScale = transform.lossyScale;
        }

        /// <summary> Struct -> Transform </summary>
        public void ApplyToTransform(in Transform transform, in Spaces posSpace, in Spaces rotSpace)
        {
            if (posSpace.Equals(Spaces.Local))
                transform.localPosition = localPosition;
            else
                transform.position = globalPosition;

            if (rotSpace.Equals(Spaces.Local))
                transform.localRotation = localRotation;
            else
                transform.rotation = globalRotation;

            transform.localScale = localScale;
            // => if lossy : Some Adjustments
        }
    }
}