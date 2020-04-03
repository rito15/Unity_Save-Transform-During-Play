
namespace Rito.Conveniences
{
    [System.Serializable]
    public class SpaceData : IJsonData
    {
        public bool isOn;
        public PositionSpace positionSpace;
        public RotationSpace rotationSpace;
        public ScaleSpace scaleSpace;

        public SpaceData(in bool on, in PositionSpace ps, in RotationSpace rs, in ScaleSpace ss)
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