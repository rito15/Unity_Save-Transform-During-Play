
namespace Rito.Conveniences
{
    [System.Serializable]
    public class SpaceData : IJsonData
    {
        public bool isOn;
        public Spaces positionSpace;
        public Spaces rotationSpace;
        public Spaces scaleSpace;

        public SpaceData(in bool on, in Spaces ps, in Spaces rs, in Spaces ss)
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