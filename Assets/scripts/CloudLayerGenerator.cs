using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudLayerGenerator : MonoBehaviour
{
    public GameObject cloudPrefab;
    public GameObject cratePrefab;
    public GameObject balloonPrefab;

    public GameObject player;
    public Rigidbody fogPlane;

    public float yOrigin;
    public float layerHeight;
    public float layerRadius;
    public float deadzoneRadius;
    public int cloudsPerLayer;
    public int layersAbovePlayer;
    public int layersBelowPlayer;

    public float startingMultiplier;
    public int lastMultiplierLayer;

    public List<LayerFeature> layerFeatures;

    private List<GameObject> cloudPool = new List<GameObject>();

    private int currentMinLayer;
    private int currentMaxLayer;

    private List<Layer> layers = new List<Layer>();
    private List<Layer> layerPool = new List<Layer>();

    private LayerComparer comparer = new LayerComparer();

    [SerializeField] private GameObjectVariable itemsContainer;
    [SerializeField] private GameObjectVariable cloudPoolContainer;

    public (float, float) GetLayerBoundsY(int layer) => (layer * layerHeight + yOrigin, layer * layerHeight + yOrigin + layerHeight);

    void Start()
    {
        Debug.Assert(itemsContainer != null);
        Debug.Assert(cloudPoolContainer != null);

        cloudPool.Capacity = Mathf.CeilToInt(cloudsPerLayer * startingMultiplier * 2);
        layers.Capacity = layersAbovePlayer + layersBelowPlayer;
        layerPool.Capacity = 2;

        currentMinLayer = 0;
        currentMaxLayer = 0;
        GenerateLayer(0);
    }

    void Update()
    {
        var playerLayer = player != null ? GetLayerOf(player.transform) : 0;

        if (playerLayer < 0) playerLayer = 0;

        var newMinLayer = Mathf.Max(playerLayer - layersBelowPlayer, 0);
        var newMaxLayer = playerLayer + layersAbovePlayer;

        for (int i = currentMinLayer; i < newMinLayer; ++i)
        {
            DestroyLayer(i);
        }

        for (int i = currentMaxLayer; i > newMaxLayer; --i)
        {
            DestroyLayer(i);
        }

        for (int i = newMinLayer; i < currentMinLayer; ++i)
        {
            GenerateLayer(i);
        }

        for (int i = newMaxLayer; i > currentMaxLayer; --i)
        {
            GenerateLayer(i);
        }

        currentMinLayer = newMinLayer;
        currentMaxLayer = newMaxLayer;
    }

    private int GetLayerOf(Transform transform)
    {
        return Mathf.FloorToInt((transform.position.y - yOrigin) / layerHeight);
    }

    private void DestroyLayer(int i)
    {
        var idx = LayerBinarySearch(i);
        
        if (idx < 0) return;

        var layer = layers[idx];
        Debug.Assert(layer.level == i);

        layers.RemoveAt(idx);

        foreach (var cloud in layer.clouds)
        {
            cloud.transform.SetParent(cloudPoolContainer.Value.transform);
            cloud.SetActive(false);
            cloudPool.Add(cloud);
        }

        foreach (var obj in layer.objects)
        {
            if (obj != null && !obj.PreventDespawn && obj.gameObject != null)
            {
                Destroy(obj.gameObject);
            }
        }

        foreach (var structure in layer.structures)
        {
            if (structure != null)
            {
                Destroy(structure);
            }
        }

        layer.clouds.Clear();
        layer.objects.Clear();
        layer.structures.Clear();

        layer.container.SetActive(false);

        layerPool.Add(layer);
    }

    private void GenerateLayer(int level)
    {
        var multiplier = Mathf.Lerp(startingMultiplier, 1f, (float)level / (float)lastMultiplierLayer);

        var numClouds = (int)(multiplier * cloudsPerLayer);

        var expectedClouds = numClouds * 8; // actually expected to be 4 x, but doubled to minimize allocations

        var expectedObjects = numClouds * 4; // actually expected to be 2 x, but doubled to minimize allocations

        var (minY, maxY) = GetLayerBoundsY(level);

        Layer layer;

        if (layerPool.Count > 0)
        {
            layer = layerPool[layerPool.Count - 1];
            layerPool.RemoveAt(layerPool.Count - 1);
            layer.level = level;
            if (layer.clouds.Capacity < expectedClouds)
            {
                layer.clouds.Capacity = expectedClouds;
            }
            if (layer.objects.Capacity < expectedObjects)
            {
                layer.objects.Capacity = expectedObjects;
            }
            layer.container.SetActive(true);
        }
        else
        {
            layer = new Layer
            {
                level = level,
                clouds = new List<GameObject>(expectedClouds),
                objects = new List<LayerObject>(expectedObjects),
                structures = new List<GameObject>(8),
                container = new GameObject(),
            };
            layer.container.transform.SetParent(transform);
        }

        var idx = LayerBinarySearch(layer.level);
        Debug.Assert(idx < 0);
        layers.Insert(~idx, layer);

        for (int i = 0; i < numClouds; ++i)
        {
            var radius = (Random.value + Random.value) / 2f * (layerRadius - deadzoneRadius) + deadzoneRadius;
            var angleRad = Random.Range(0f, Mathf.PI * 2f);

            var x = Mathf.Sin(angleRad) * radius;
            var z = Mathf.Cos(angleRad) * radius;

            var coord = new Vector3(x, Random.Range(minY, maxY), z);

            // skip clouds below the fog
            if (fogPlane != null && coord.y < fogPlane.transform.position.y)
            {
                continue;
            }

            SpawnCloud(coord);

            while (Random.value < .75f)
            {
                var dir = Vector3.Scale(Random.onUnitSphere, new Vector3(2.5f, 1f, 2.5f));

                if (dir == Vector3.zero) continue;

                dir += dir.normalized * 2.5f;

                coord += dir;

                if (coord.magnitude < deadzoneRadius) continue;

                SpawnCloud(coord);
            }
        }

        foreach (var layerFeature in layerFeatures)
        {
            if (Random.value * 100f < layerFeature.chancePerLayer)
            {
                layerFeature.Spawn(this, level);
            }
        }

        void SpawnCloud(Vector3 coord)
        {
            var rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

            GameObject cloud = CreateLayerCloud(coord, rotation, ref layer);

            if (Random.value < .1f)
            {
                CreateLayerItem(cratePrefab, coord + new Vector3(0, 1, 0), Quaternion.identity, ref layer);
            }

            if (Random.value < .4f)
            {
                CreateLayerItem(balloonPrefab, coord + new Vector3(0, 3, 0), Quaternion.identity, ref layer);
            }
        }
    }

    private bool TryGetLayer(int level, out Layer layer)
    {
        var idx = LayerBinarySearch(level);

        if (idx < 0)
        {
            layer = default;
            return false;
        }

        layer = layers[idx];
        Debug.Assert(layer.level == level);

        return true;
    }

    public GameObject CreateLayerCloud(Vector3 coord, Quaternion rotation, int level)
    {
        if (TryGetLayer(level, out var layer))
        {
            return CreateLayerCloud(coord, rotation, ref layer);
        }
        else
        {
            return null;
        }
    }

    private GameObject CreateLayerCloud(Vector3 coord, Quaternion rotation, ref Layer layer)
    {
        GameObject cloud;

        if (cloudPool.Count > 0)
        {
            cloud = cloudPool[cloudPool.Count - 1];
            cloudPool.RemoveAt(cloudPool.Count - 1);
            cloud.transform.SetParent(layer.container.transform);
            cloud.transform.position = coord;
            cloud.transform.rotation = rotation;
            cloud.SetActive(true);
        }
        else
        {
            cloud = Instantiate(cloudPrefab, coord, rotation, layer.container.transform);
        }

        layer.clouds.Add(cloud);

        return cloud;
    }

    public GameObject CreateLayerItem(GameObject prefab, Vector3 position, Quaternion rotation, int level)
    {
        if (TryGetLayer(level, out var layer))
        {
            return CreateLayerItem(prefab, position, rotation, ref layer);
        }
        else
        {
            return null;
        }
    }

    private GameObject CreateLayerItem(GameObject prefab, Vector3 position, Quaternion rotation, ref Layer layer)
    {
        var item = Instantiate(prefab, position, rotation, itemsContainer.Value.transform);

        var obj = item.GetComponent<LayerObject>();
        Debug.Assert(obj != null);

        layer.objects.Add(obj);

        return item;
    }

    public GameObject CreateLayerStructure(GameObject prefab, Vector3 position, Quaternion rotation, int level)
    {
        if (TryGetLayer(level, out var layer))
        {
            var obj = Instantiate(prefab, position, rotation, layer.container.transform);

            layer.structures.Add(obj);

            return obj;
        }
        else
        {
            return null;
        }
    }

    private int LayerBinarySearch(int layer)
    {
        return layers.BinarySearch(new Layer { level = layer }, comparer);
    }

    private struct Layer
    {
        public int level;
        public List<GameObject> clouds;
        public List<LayerObject> objects;
        public List<GameObject> structures;
        public GameObject container;
    }

    private class LayerComparer : IComparer<Layer>
    {
        public int Compare(Layer x, Layer y)
        {
            return x.level - y.level;
        }
    }
}
