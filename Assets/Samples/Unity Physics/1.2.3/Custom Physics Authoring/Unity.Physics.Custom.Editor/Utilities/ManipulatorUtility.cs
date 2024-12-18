using Unity.Mathematics;

namespace Unity.Physics.Editor
{
    internal enum MatrixState
    {
        UniformScale,
        NonUniformScale,
        ZeroScale,
        NotValidTRS
    }

    internal static class ManipulatorUtility
    {
        public static MatrixState GetMatrixState(ref float4x4 localToWorld)
        {
            if (
                localToWorld.c0.w != 0f
                || localToWorld.c1.w != 0f
                || localToWorld.c2.w != 0f
                || localToWorld.c3.w != 1f
            )
                return MatrixState.NotValidTRS;

            float3x3 m = new float3x3(localToWorld.c0.xyz, localToWorld.c1.xyz, localToWorld.c2.xyz);
            float3 lossyScale = new float3(math.length(m.c0.xyz), math.length(m.c1.xyz), math.length(m.c2.xyz));
            if (math.determinant(m) < 0f)
                lossyScale.x *= -1f;
            if (math.lengthsq(lossyScale) == 0f)
                return MatrixState.ZeroScale;
            return math.abs(math.cmax(lossyScale)) - math.abs(math.cmin(lossyScale)) > 0.000001f
                ? MatrixState.NonUniformScale
                : MatrixState.UniformScale;
        }
    }
}