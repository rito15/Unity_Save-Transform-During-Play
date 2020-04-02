using System;
using UnityEngine;

namespace Rito.Conveniences
{
    [Serializable]
    public struct TransformData
    {
        public Vector3 localPosition;
        public Quaternion localRotation;
        public Vector3 localScale;

        public Vector3 globalPosition;
        public Quaternion globalRotation;

        // 생성자
        public TransformData(in Transform transform)
        {
            localPosition = transform.localPosition;
            localRotation = transform.localRotation;
            localScale = transform.localScale;

            globalPosition = transform.position;
            globalRotation = transform.rotation;
        }

        /// <summary> Struct -> Transform </summary>
        public void Load(in Transform transform, PositionSpace posSpace, RotationSpace rotSpace, ScaleSpace sclSpace)
        {
            if (posSpace.Equals(PositionSpace.Local))
                transform.localPosition = localPosition;
            else
                transform.position = globalPosition;

            if (rotSpace.Equals(RotationSpace.Local))
                transform.localRotation = localRotation;
            else
                transform.rotation = globalRotation;

            if (sclSpace.Equals(ScaleSpace.Local))
                transform.localScale = localScale;
        }

        // null 패턴
        private static TransformData _null
            = new TransformData
            {
                localPosition = Vector3.negativeInfinity,
                localRotation = Quaternion.identity,
                localScale = Vector3.one,
                globalPosition = Vector3.negativeInfinity,
                globalRotation = Quaternion.identity,
            };
        public static TransformData Null => _null;
    }
}