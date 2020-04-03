
namespace Rito.Conveniences
{
    [System.Serializable]
    public class SpaceData : CommonData
    {
        public bool isOn;
        public PositionSpace positionSpace;
        public RotationSpace rotationSpace;
        public ScaleSpace scaleSpace;

        public SpaceData(bool on, PositionSpace ps, RotationSpace rs, ScaleSpace ss)
        {
            isOn = on;
            positionSpace = ps;
            rotationSpace = rs;
            scaleSpace = ss;
        }
        public SpaceData() : this(true, default, default, default) { }

        public override string ToString()
        {
            return $"SpaceData : {isOn} / {positionSpace} / {rotationSpace} / {scaleSpace}";
        }
    }
}