using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RainbowController : MonoBehaviour
{
    public Transform startCloud;
    public Transform endCloud;

    public List<Reward> possibleRewards;
    public GameObject sparklePrefab;
    public GameObjectVariable itemContainer;
    public Vector3 rewardPosition;

    public GameObject particles;

    public float distFromCenterOfCloud = 1f;
    public Vector3 additionalOffset = new Vector3(0f, 0.5f, 0f);
    public float inclinationAtLowerCloud = 45f;
    public float width = 2f;
    public float angleStep = 5f;
    public float colliderThickness = 0.05f;
    public float pushArcLength = 1f;
    public float startingArcLength = 2f;

    private float percentPushed = 0.05f;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private List<BoxCollider> collisionBoxes = new List<BoxCollider>();

    private Hash128 lastCircleHash;
    private Vector3[] cachedVertices;
    private Vector3[] cachedNormals;
    private Vector2[] cachedUvs;
    private Color[] cachedColors;
    private int[] cachedTriangles;

    private Circle cachedCircle;

    private GameObject pusher;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        Debug.Assert(meshFilter != null);
        meshRenderer = GetComponent<MeshRenderer>();
        Debug.Assert(meshRenderer != null);
    }

    void Start()
    {
        ref readonly var circle = ref GetCircle();
        var endAngle = Mathf.Repeat(Vector3.SignedAngle(circle.start, circle.end, circle.normal), 360f);
        percentPushed = Mathf.Rad2Deg * startingArcLength / circle.radius / endAngle;

        Refresh();
    }

    void Update()
    {
        UpdatePushed();
        Refresh();

        if (percentPushed == 1f && particles != null)
        {
            var pos = endCloud.position + rewardPosition;

            var idx = Random.Range(0, possibleRewards.Count);
            var reward = possibleRewards[idx];

            var rot = reward.randomRotation ? Random.rotation : Quaternion.identity;

            Instantiate(sparklePrefab, pos, Quaternion.identity);

            Instantiate(reward.prefab, pos, rot, itemContainer.Value.transform);

            Destroy(particles);
            particles = null;
        }
    }

    public ref readonly Circle GetCircle()
    {
        var currentHash = GetPropsHash();

        if (currentHash != cachedCircle.hash)
        {
            cachedCircle = ComputeIdealCircle(inclinationAtLowerCloud);
            cachedCircle.hash = currentHash;
        }

        return ref cachedCircle;
    }

    void OnDrawGizmos()
    {
        ref readonly var circle = ref GetCircle();
        #if UNITY_EDITOR
        if (circle.score > 0.5f)
        {
            UnityEditor.Handles.color = Color.red;
        }
        else
        {
            UnityEditor.Handles.color = Color.white;
        }
        #endif


        var endAngle = Mathf.Repeat(Vector3.SignedAngle(circle.start, circle.end, circle.normal), 360f);

        #if UNITY_EDITOR
        UnityEditor.Handles.DrawWireArc(circle.center, circle.normal, circle.start, endAngle, circle.radius);
        #endif

    }

    public void StartPushing(GameObject gameObject)
    {
        pusher = gameObject;
    }

    public void StopPushing(GameObject gameObject)
    {
        Debug.Assert(gameObject == pusher);
        pusher = null;
    }

    private void UpdatePushed()
    {
        if (pusher == null) return;

        ref readonly var circle = ref GetCircle();

        var relPos = pusher.transform.position - circle.center;
        var posOnPlane = Vector3.ProjectOnPlane(relPos, circle.normal);
        var angle = Mathf.Repeat(Vector3.SignedAngle(circle.start, posOnPlane, circle.normal), 360f);
        var endAngle = Mathf.Repeat(Vector3.SignedAngle(circle.start, circle.end, circle.normal), 360f);

        var addedArc = Mathf.Rad2Deg * pushArcLength / circle.radius;

        var pusherProgress = Mathf.Clamp((angle + addedArc) / endAngle, 0f, 1f);

        if (pusherProgress > percentPushed)
        {
            percentPushed = pusherProgress;
        }
    }

    private Hash128 GetPropsHash()
    {
        var hash = new Hash128();
        if (startCloud == null || endCloud == null) return hash;
        hash.Append(startCloud.transform.position.x);
        hash.Append(startCloud.transform.position.y);
        hash.Append(startCloud.transform.position.z);
        hash.Append(endCloud.transform.position.x);
        hash.Append(endCloud.transform.position.y);
        hash.Append(endCloud.transform.position.z);
        hash.Append(distFromCenterOfCloud);
        hash.Append(additionalOffset.x);
        hash.Append(additionalOffset.y);
        hash.Append(additionalOffset.z);
        hash.Append(inclinationAtLowerCloud);
        hash.Append(width);
        hash.Append(angleStep);
        hash.Append(colliderThickness);
        hash.Append(percentPushed);
        return hash;
    }

    private void Refresh()
    {
        ref readonly var circle = ref GetCircle();

        if (circle.hash != lastCircleHash)
        {
            if (startCloud != null && endCloud != null)
            {
                transform.position = circle.center;

                GenerateVisualMesh(circle);
                GenerateCollision(circle);
                UpdateParticles(circle);
            }

            lastCircleHash = circle.hash;
        }
    }

    private Circle ComputeIdealCircle(float lowerInclineDeg)
    {
        // common and simple values

        var startDirection = Vector3.Scale(endCloud.position - startCloud.position, new Vector3(1, 0, 1)).normalized;
        var cloudOffset = startDirection * distFromCenterOfCloud;
        var start = startCloud.position + cloudOffset + additionalOffset;
        var end = endCloud.position - cloudOffset + additionalOffset;
        var startToEnd = end - start;
        var linearDist = startToEnd.magnitude;

        var lowerIncline = Mathf.Deg2Rad * lowerInclineDeg;

        // determine low and high

        var (low, high, horizontalDirection) = start.y < end.y ? (start, end, startDirection) : (end, start, -startDirection);

        // compute linear incline (the existing incline of low to high)

        var linearInclineSin = (high.y - low.y) / linearDist;
        var linearIncline = Mathf.Asin(linearInclineSin);

        // compute inscrible angles (angle between the incline directions and the direction to the circle center)

        var horizontalDirectionInscribedAngle = Mathf.PI * 0.5f - lowerIncline;
        var inscribedAngle = linearIncline + horizontalDirectionInscribedAngle;

        // compute circle values

        var radius = (linearDist * 0.5f) / Mathf.Cos(inscribedAngle);
        var lowNormal = Vector3.Cross(high - low, Vector3.up).normalized;

        var lowToCenterDir = Quaternion.AngleAxis(Mathf.Rad2Deg * -horizontalDirectionInscribedAngle, lowNormal) * horizontalDirection;

        var center = low + lowToCenterDir * radius;
        var normal = Vector3.Cross(start - end, Vector3.up).normalized;

        return new Circle
        {
            center = center,
            normal = normal,
            radius = radius,
            start = start - center,
            end = end - center,
            score = Mathf.Abs(2f * linearIncline / lowerIncline),
        };
    }

    private void GenerateVisualMesh(in Circle circle)
    {
        var endAngle = Mathf.Repeat(Vector3.SignedAngle(circle.start, circle.end, circle.normal), 360f);
        var angle = endAngle * percentPushed;

        var numSegments = Mathf.CeilToInt(angle / angleStep);

        if (numSegments == 0)
        {
            meshFilter.sharedMesh = null;
            return;
        }

        var trueAngleStep = angle / (float)numSegments;

        var chordLength = 2f * circle.radius * Mathf.Sin(Mathf.Deg2Rad * trueAngleStep / 2f);
        var sagitta = chordLength / 2f * Mathf.Tan(Mathf.Deg2Rad * trueAngleStep / 4f);

        var halfWidth = width / 2f;
        var extent = circle.normal * (halfWidth + sagitta);

        int numVertices = 2 * (numSegments + 1);
        int numTriangleIndices = 3 * 2 * numSegments;

        Vector3[] vertices;
        Vector3[] normals;
        Vector2[] uvs;
        Color[] colors;
        int[] triangles;

        if (cachedVertices != null && cachedVertices.Length == numVertices &&
            cachedNormals != null && cachedNormals.Length == numVertices &&
            cachedUvs != null && cachedUvs.Length == numVertices &&
            cachedColors != null && cachedColors.Length == numVertices &&
            cachedTriangles != null && cachedTriangles.Length == numTriangleIndices)
        {
            vertices = cachedVertices;
            normals = cachedNormals;
            uvs = cachedUvs;
            colors = cachedColors;
            triangles = cachedTriangles;
        }
        else
        {
            vertices = new Vector3[numVertices];
            normals = new Vector3[numVertices];
            uvs = new Vector2[numVertices];
            colors = new Color[numVertices];
            triangles = new int[numTriangleIndices];

            cachedVertices = vertices;
            cachedNormals = normals;
            cachedUvs = uvs;
            cachedColors = colors;
            cachedTriangles = triangles;
        }

        for (var i = 0; i <= numSegments; ++i)
        {
            var currentAngle = trueAngleStep * (float)i;
            var rot = Quaternion.AngleAxis(currentAngle, circle.normal);
            var pos = rot * circle.start;

            var direction = pos.normalized;

            var vertexBase = i * 2;

            vertices[vertexBase + 0] = pos + extent;
            vertices[vertexBase + 1] = pos - extent;

            normals[vertexBase + 0] = direction;
            normals[vertexBase + 1] = direction;

            uvs[vertexBase + 0] = new Vector2(0, 0);
            uvs[vertexBase + 1] = new Vector2(1, 0);

            colors[vertexBase + 0] = new Color(pos.x, pos.y, pos.z, 0);
            colors[vertexBase + 1] = new Color(pos.x, pos.y, pos.z, 0);

            if (i == 0) continue;

            var triangleBase = (i - 1) * 2 * 3;

            triangles[triangleBase + 0] = vertexBase + 0;
            triangles[triangleBase + 1] = vertexBase - 2;
            triangles[triangleBase + 2] = vertexBase - 1;
            triangles[triangleBase + 3] = vertexBase + 0;
            triangles[triangleBase + 4] = vertexBase - 1;
            triangles[triangleBase + 5] = vertexBase + 1;
        }

        var mesh = new Mesh
        {
            name = "Rainbow Visual Mesh",
            vertices = vertices,
            normals = normals,
            uv = uvs,
            colors = colors,
            triangles = triangles,
        };

        meshFilter.sharedMesh = mesh;
    }

    private void GenerateCollision(in Circle circle)
    {
        var endAngle = Mathf.Repeat(Vector3.SignedAngle(circle.start, circle.end, circle.normal), 360f);

        var angle = endAngle * percentPushed;

        var numSegments = Mathf.CeilToInt(angle / angleStep);

        if (collisionBoxes.Capacity < numSegments)
        {
            collisionBoxes.Capacity = numSegments;
        }

        for (var i = collisionBoxes.Count; i < numSegments; ++i)
        {
            var obj = new GameObject($"Rainbow Collision Box [{i}]");
            obj.transform.SetParent(this.transform);
            obj.layer = this.gameObject.layer;

            var collider = obj.AddComponent<BoxCollider>();
            collisionBoxes.Add(collider);
        }

        for (var i = numSegments; i < collisionBoxes.Count; ++i)
        {
            Destroy(collisionBoxes[i].gameObject);
        }
        collisionBoxes.RemoveRange(numSegments, collisionBoxes.Count - numSegments);

        if (numSegments == 0) return;

        var trueAngleStep = angle / (float)numSegments;
        var boxLength = 2f * circle.radius * Mathf.Sin(Mathf.Deg2Rad * trueAngleStep / 2f);

        // mutatable
        var nextPoint = circle.center + circle.start;

        for (var i = 0; i < collisionBoxes.Count; ++i)
        {
            var box = collisionBoxes[i];

            box.size = new Vector3(width, boxLength, colliderThickness);
            box.center = new Vector3(0, boxLength / 2f, colliderThickness / 2f);

            var pos = nextPoint;

            var nextAngle = trueAngleStep * (float)(i + 1);
            var rot = Quaternion.AngleAxis(nextAngle, circle.normal);
            nextPoint = circle.center + rot * circle.start;

            var toNextPoint = nextPoint - pos;

            var lookat = circle.center - toNextPoint / 2f;

            box.transform.position = pos;
            box.transform.LookAt(lookat, toNextPoint);
        }
    }

    private void UpdateParticles(in Circle circle)
    {
        var endAngle = percentPushed * Mathf.Repeat(Vector3.SignedAngle(circle.start, circle.end, circle.normal), 360f);
        var rot = Quaternion.AngleAxis(endAngle, circle.normal);
        var pos = circle.center + rot * circle.start;

        particles.transform.position = pos;
        particles.transform.rotation = Quaternion.LookRotation(Vector3.Cross(circle.normal, Vector3.up), Vector3.up);
    }

    public struct Circle
    {
        public Vector3 center;
        public Vector3 normal;
        public Vector3 start;
        public Vector3 end;
        public float radius;
        public float score;
        public Hash128 hash;
    }

    [System.Serializable]
    public class Reward
    {
        public GameObject prefab;
        public bool randomRotation;
    }

#if false
    // kept for posterity
    [Obsolete("Use ComputeIdealCircle instead")]
    private Circle ComputeCircleFromHeight(float heightFromHigherCloud)
    {
        var startDirection = Vector3.Scale(endCloud.position - startCloud.position, new Vector3(1, 0, 1)).normalized;
        var cloudOffset = startDirection * distFromCenterOfCloud;
        var start = startCloud.position + cloudOffset + additionalOffset;
        var end = endCloud.position - cloudOffset + additionalOffset;
        var startToEnd = end - start;
        var midpoint = start + startToEnd * 0.5f;
        var peak = midpoint;
        peak.y = Mathf.Max(start.y, end.y) + heightFromHigherCloud;

        var startToPeak = peak - start;
        var normal = Vector3.Cross(startToPeak, startToEnd).normalized;

        // the center must lie on the same plane as start, end, and peak, therefore:
        // center = start + k1 * startToPeak + k2 * startToEnd

        var dot1 = Vector3.Dot(startToPeak, startToPeak);
        var dot2 = Vector3.Dot(startToEnd, startToEnd);
        var dot3 = Vector3.Dot(startToPeak, startToEnd);
        var quot = dot1 * dot2 - dot3 * dot3;

        var k1 = 0.5f * dot2 * (dot1 - dot3) / quot;
        var k2 = 0.5f * dot1 * (dot2 - dot3) / quot;

        var center = start + k1 * startToPeak + k2 * startToEnd;

        var radius = (start - center).magnitude;

        return new Circle
        {
            center = center,
            normal = normal,
            start = start - center,
            end = end - center,
            radius = radius,
        };
    }
#endif
}
