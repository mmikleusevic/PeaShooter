using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Unity.Physics.Authoring
{
    // put static UnityObject buffers in separate utility class so other methods can Burst compile
    internal static class PhysicsShapeExtensions_NonBursted
    {
        internal static readonly List<PhysicsBodyAuthoring> s_PhysicsBodiesBuffer = new(16);
        internal static readonly List<PhysicsShapeAuthoring> s_ShapesBuffer = new(16);
        internal static readonly List<Rigidbody> s_RigidbodiesBuffer = new(16);
        internal static readonly List<UnityEngine.Collider> s_CollidersBuffer = new(16);
    }

    public static partial class PhysicsShapeExtensions
    {
        // avoids drift in axes we're not actually changing
        public const float kMinimumChange = HashableShapeInputs.k_DefaultLinearPrecision;

        private const float k_HashFloatTolerance = 0.01f;

        // used for de-skewing basis vectors; default priority assumes primary axis is z, secondary axis is y
        public static readonly int3 k_DefaultAxisPriority = new(2, 1, 0);

        private static readonly int[] k_NextAxis = { 1, 2, 0 };
        private static readonly int[] k_PrevAxis = { 2, 0, 1 };

        // matrix to transform point from shape's local basis into world space
        public static float4x4 GetBasisToWorldMatrix(
            float4x4 localToWorld, float3 center, quaternion orientation, float3 size
        )
        {
            return math.mul(localToWorld, float4x4.TRS(center, orientation, size));
        }

        private static float4 DeskewSecondaryAxis(float4 primaryAxis, float4 secondaryAxis)
        {
            float4 n0 = math.normalizesafe(primaryAxis);
            float dot = math.dot(secondaryAxis, n0);
            return secondaryAxis - n0 * dot;
        }

        // priority is determined by length of each size dimension in the shape's basis after applying localToWorld transformation
        public static int3 GetBasisAxisPriority(float4x4 basisToWorld)
        {
            float3 basisAxisLengths = basisToWorld.DecomposeScale();
            float max = math.cmax(basisAxisLengths);
            float min = math.cmin(basisAxisLengths);
            if (max == min)
                return k_DefaultAxisPriority;

            int imax = max == basisAxisLengths.x ? 0 : max == basisAxisLengths.y ? 1 : 2;

            basisToWorld[k_NextAxis[imax]] = DeskewSecondaryAxis(basisToWorld[imax], basisToWorld[k_NextAxis[imax]]);
            basisToWorld[k_PrevAxis[imax]] = DeskewSecondaryAxis(basisToWorld[imax], basisToWorld[k_PrevAxis[imax]]);

            basisAxisLengths = basisToWorld.DecomposeScale();
            min = math.cmin(basisAxisLengths);
            int imin = min == basisAxisLengths.x ? 0 : min == basisAxisLengths.y ? 1 : 2;
            if (imin == imax)
                imin = k_NextAxis[imax];
            int imid = k_NextAxis[imax] == imin ? k_PrevAxis[imax] : k_NextAxis[imax];

            return new int3(imax, imid, imin);
        }

        [Conditional(CompilationSymbols.CollectionsChecksSymbol)]
        [Conditional(CompilationSymbols.DebugChecksSymbol)]
        private static void CheckBasisPriorityAndThrow(int3 basisPriority)
        {
            if (
                basisPriority.x == basisPriority.y
                || basisPriority.x == basisPriority.z
                || basisPriority.y == basisPriority.z
            )
                throw new ArgumentException(nameof(basisPriority));
        }

        // matrix to transform point on a primitive from bake space into space of the shape
        internal static float4x4 GetPrimitiveBakeToShapeMatrix(
            float4x4 localToWorld, float4x4 shapeToWorld, ref float3 center, ref EulerAngles orientation, float3 scale,
            int3 basisPriority, bool bakeUniformScale = true
        )
        {
            CheckBasisPriorityAndThrow(basisPriority);

            float4x4 localToBasis = float4x4.TRS(center, orientation, scale);
            // correct for imprecision in cases of no scale to prevent e.g., convex radius from being altered
            if (scale.Equals(new float3(1f)))
            {
                localToBasis.c0 = math.normalizesafe(localToBasis.c0);
                localToBasis.c1 = math.normalizesafe(localToBasis.c1);
                localToBasis.c2 = math.normalizesafe(localToBasis.c2);
            }

            float4x4 bakeToShape;

            if (localToWorld.HasNonUniformScale() || localToWorld.HasShear())
            {
                float4x4 localToBake = math.mul(localToWorld, localToBasis);
                // deskew second longest axis with respect to longest axis
                localToBake[basisPriority[1]] =
                    DeskewSecondaryAxis(localToBake[basisPriority[0]], localToBake[basisPriority[1]]);

                // recompute third axes from first two
                float4 n2 = math.normalizesafe(
                    new float4(math.cross(localToBake[basisPriority[0]].xyz, localToBake[basisPriority[1]].xyz), 0f)
                );
                localToBake[basisPriority[2]] = n2 * math.dot(localToBake[basisPriority[2]], n2);

                bakeToShape = math.mul(math.inverse(shapeToWorld), localToBake);
            }
            else
            {
                if (bakeUniformScale)
                {
                    float4x4 localToBake = math.mul(localToWorld, localToBasis);
                    bakeToShape = math.mul(math.inverse(shapeToWorld), localToBake);
                }
                else
                {
                    bakeToShape = localToBasis;
                }
            }

            // transform baked center/orientation (i.e. primitive basis) into shape space
            orientation.SetValue(
                quaternion.LookRotationSafe(bakeToShape[basisPriority[0]].xyz, bakeToShape[basisPriority[1]].xyz)
            );
            center = bakeToShape.c3.xyz;

            return bakeToShape;
        }

        internal static CollisionFilter GetFilter(this PhysicsShapeAuthoring shape)
        {
            // TODO: determine optimal workflow for specifying group index
            return new CollisionFilter
            {
                BelongsTo = shape.BelongsTo.Value,
                CollidesWith = shape.CollidesWith.Value
            };
        }

        internal static Material GetMaterial(this PhysicsShapeAuthoring shape)
        {
            // TODO: TBD how we will author editor content for other shape flags
            return new Material
            {
                Friction = shape.Friction.Value,
                FrictionCombinePolicy = shape.Friction.CombineMode,
                Restitution = shape.Restitution.Value,
                RestitutionCombinePolicy = shape.Restitution.CombineMode,
                CollisionResponse = shape.CollisionResponse,
                CustomTags = shape.CustomTags.Value
            };
        }

        public static GameObject FindTopmostEnabledAncestor<T>(GameObject shape, List<T> buffer) where T : Component
        {
            // include inactive in case the supplied shape GameObject is a prefab that has not been instantiated
            shape.GetComponentsInParent(true, buffer);
            GameObject result = null;
            for (int i = buffer.Count - 1; i >= 0; --i)
                if (
                    (buffer[i] as UnityEngine.Collider)?.enabled ??
                    (buffer[i] as MonoBehaviour)?.enabled ?? true
                )
                {
                    result = buffer[i].gameObject;
                    break;
                }

            buffer.Clear();
            return result;
        }

        public static GameObject GetPrimaryBody(this PhysicsShapeAuthoring shape)
        {
            return GetPrimaryBody(shape.gameObject);
        }

        public static GameObject GetPrimaryBody(GameObject shape)
        {
            GameObject pb = ColliderExtensions.FindFirstEnabledAncestor(shape,
                PhysicsShapeExtensions_NonBursted.s_PhysicsBodiesBuffer);
            GameObject rb = ColliderExtensions.FindFirstEnabledAncestor(shape,
                PhysicsShapeExtensions_NonBursted.s_RigidbodiesBuffer);

            if (pb != null)
                return rb == null ? pb.gameObject :
                    pb.transform.IsChildOf(rb.transform) ? pb.gameObject : rb.gameObject;

            if (rb != null)
                return rb.gameObject;

            // for implicit static shape, first see if it is part of static optimized hierarchy
            ColliderExtensions.FindTopmostStaticEnabledAncestor(shape, out GameObject topStatic);
            if (topStatic != null)
                return topStatic;

            // otherwise, find topmost enabled Collider or PhysicsShapeAuthoring
            GameObject topCollider =
                FindTopmostEnabledAncestor(shape, PhysicsShapeExtensions_NonBursted.s_CollidersBuffer);
            GameObject topShape = FindTopmostEnabledAncestor(shape, PhysicsShapeExtensions_NonBursted.s_ShapesBuffer);

            return topCollider == null
                ? topShape == null ? shape.gameObject : topShape
                : topShape == null
                    ? topCollider
                    : topShape.transform.IsChildOf(topCollider.transform)
                        ? topCollider
                        : topShape;
        }

        public static BoxGeometry GetBakedBoxProperties(this PhysicsShapeAuthoring shape)
        {
            BoxGeometry box = shape.GetBoxProperties(out EulerAngles orientation);
            return box.BakeToBodySpace(shape.transform.localToWorldMatrix, shape.GetShapeToWorldMatrix(), orientation);
        }

        internal static BoxGeometry BakeToBodySpace(
            this BoxGeometry box, float4x4 localToWorld, float4x4 shapeToWorld, EulerAngles orientation,
            bool bakeUniformScale = true
        )
        {
            using (NativeArray<BoxGeometry> geometry = new NativeArray<BoxGeometry>(1, Allocator.TempJob) { [0] = box })
            {
                BakeBoxJob job = new BakeBoxJob
                {
                    Box = geometry,
                    LocalToWorld = localToWorld,
                    ShapeToWorld = shapeToWorld,
                    Orientation = orientation,
                    BakeUniformScale = bakeUniformScale
                };
                job.Run();
                return geometry[0];
            }
        }

        public static void SetBakedBoxSize(this PhysicsShapeAuthoring shape, float3 size, float bevelRadius)
        {
            BoxGeometry box = shape.GetBoxProperties(out EulerAngles orientation);
            float3 center = box.Center;
            float3 prevSize = math.abs(box.Size);
            size = math.abs(size);

            float4x4 bakeToShape = BakeBoxJobExtension.GetBakeToShape(shape, center, orientation);
            float3 scale = bakeToShape.DecomposeScale();

            size /= scale;

            if (math.abs(size[0] - prevSize[0]) < kMinimumChange) size[0] = prevSize[0];
            if (math.abs(size[1] - prevSize[1]) < kMinimumChange) size[1] = prevSize[1];
            if (math.abs(size[2] - prevSize[2]) < kMinimumChange) size[2] = prevSize[2];

            box.BevelRadius = bevelRadius;
            box.Size = size;

            shape.SetBox(box, orientation);
        }

        internal static CapsuleGeometryAuthoring GetBakedCapsuleProperties(this PhysicsShapeAuthoring shape)
        {
            CapsuleGeometryAuthoring capsule = shape.GetCapsuleProperties();
            return capsule.BakeToBodySpace(shape.transform.localToWorldMatrix, shape.GetShapeToWorldMatrix());
        }

        public static void SetBakedCylinderSize(this PhysicsShapeAuthoring shape, float height, float radius,
            float bevelRadius)
        {
            CylinderGeometry cylinder = shape.GetCylinderProperties(out EulerAngles orientation);
            float3 center = cylinder.Center;

            float4x4 bakeToShape = BakeCylinderJobExtension.GetBakeToShape(shape, center, orientation);
            float3 scale = bakeToShape.DecomposeScale();

            float newRadius = radius / math.cmax(scale.xy);
            if (math.abs(cylinder.Radius - newRadius) > kMinimumChange) cylinder.Radius = newRadius;
            if (math.abs(cylinder.BevelRadius - bevelRadius) > kMinimumChange) cylinder.BevelRadius = bevelRadius;


            float newHeight = math.max(0, height / scale.z);
            if (math.abs(cylinder.Height - newHeight) > kMinimumChange) cylinder.Height = newHeight;
            shape.SetCylinder(cylinder, orientation);
        }

        internal static SphereGeometry GetBakedSphereProperties(this PhysicsShapeAuthoring shape,
            out EulerAngles orientation)
        {
            SphereGeometry sphere = shape.GetSphereProperties(out orientation);
            return sphere.BakeToBodySpace(shape.transform.localToWorldMatrix, shape.GetShapeToWorldMatrix(),
                ref orientation);
        }

        internal static void GetBakedPlaneProperties(
            this PhysicsShapeAuthoring shape, out float3 vertex0, out float3 vertex1, out float3 vertex2,
            out float3 vertex3
        )
        {
            float4x4 bakeToShape = shape.GetLocalToShapeMatrix();
            shape.GetPlaneProperties(out float3 center, out float2 size, out EulerAngles orientation);
            BakeToBodySpace(
                center, size, orientation, bakeToShape,
                out vertex0, out vertex1, out vertex2, out vertex3
            );
        }

        public static void GetBakedConvexProperties(this PhysicsShapeAuthoring shape, NativeList<float3> pointCloud)
        {
            shape.GetConvexHullProperties(pointCloud, true, default, default, default, default);
            shape.BakePoints(pointCloud.AsArray());
        }

        public static void GetBakedMeshProperties(
            this PhysicsShapeAuthoring shape, NativeList<float3> vertices, NativeList<int3> triangles,
            HashSet<UnityEngine.Mesh> meshAssets = null
        )
        {
            shape.GetMeshProperties(vertices, triangles, true, default, meshAssets);
            shape.BakePoints(vertices.AsArray());
        }

        // used to hash convex hull generation properties in a way that is robust to imprecision
        public static uint GetStableHash(
            this ConvexHullGenerationParameters generationParameters,
            ConvexHullGenerationParameters hashedParameters,
            float tolerance = k_HashFloatTolerance
        )
        {
            float3 differences = new float3(
                generationParameters.BevelRadius - hashedParameters.BevelRadius,
                generationParameters.MinimumAngle - hashedParameters.MinimumAngle,
                generationParameters.SimplificationTolerance - hashedParameters.SimplificationTolerance
            );
            return math.cmax(math.abs(differences)) < tolerance
                ? unchecked((uint)hashedParameters.GetHashCode())
                : unchecked((uint)generationParameters.GetHashCode());
        }

        // used to hash an array of points in a way that is robust to imprecision
        public static unsafe uint GetStableHash(
            this NativeList<float3> points, NativeArray<float3> hashedPoints, float tolerance = k_HashFloatTolerance
        )
        {
            if (points.Length != hashedPoints.Length)
                return math.hash(points.GetUnsafePtr(), UnsafeUtility.SizeOf<float3>() * points.Length);

            for (int i = 0, count = points.Length; i < count; ++i)
                if (math.cmax(math.abs(points[i] - hashedPoints[i])) > tolerance)
                    return math.hash(points.GetUnsafePtr(), UnsafeUtility.SizeOf<float3>() * points.Length);
            return math.hash(hashedPoints.GetUnsafePtr(), UnsafeUtility.SizeOf<float3>() * hashedPoints.Length);
        }

        public static int GetMaxAxis(this float3 v)
        {
            float cmax = math.cmax(v);
            return cmax == v.z ? 2 : cmax == v.y ? 1 : 0;
        }

        public static int GetDeviantAxis(this float3 v)
        {
            float3 deviation = math.abs(v - math.csum(v) / 3f);
            return math.cmax(deviation) == deviation.z ? 2 : math.cmax(deviation) == deviation.y ? 1 : 0;
        }
    }
}