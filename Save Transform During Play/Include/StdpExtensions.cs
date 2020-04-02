namespace Rito.Conveniences
{
    public static class StdpExtensions
    {
        public static void Reverse(ref this PositionSpace ps)
        {
            if (ps.Equals(PositionSpace.World)) ps = PositionSpace.Local;
            else if (ps.Equals(PositionSpace.Local)) ps = PositionSpace.World;
        }

        public static void Reverse(ref this RotationSpace rs)
        {
            if (rs.Equals(RotationSpace.World)) rs = RotationSpace.Local;
            else if (rs.Equals(RotationSpace.Local)) rs = RotationSpace.World;
        }

        public static bool XOR(in this bool lValue, in bool rValue)
        {
            if (lValue && !rValue) return true;
            if (!lValue && rValue) return true;
            return false;
        }
    }
}