using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject cloudPrefab;
    public GameObject cratePrefab;
    public GameObject balloonPrefab;
    public GameObject player;

    public float yOrigin;
    public float layerHeight;
    public float layerRadius;
    public float deadzoneRadius;
    public int cloudsPerLayer;
    public int layersAbovePlayer;
    public int layersBelowPlayer;

    public float startingMultiplier;
    public int lastMultiplierLayer;

    private List<GameObject> cloudPool = new List<GameObject>();

    private int currentMinLayer;
    private int currentMaxLayer;

    private List<Layer> layers = new List<Layer>();
    private List<Layer> layerPool = new List<Layer>();

    private LayerComparer comparer = new LayerComparer();

    void Start()
    {
        cloudPool.Capacity = Mathf.CeilToInt(cloudsPerLayer * startingMultiplier * 2);
        layers.Capacity = layersAbovePlayer + layersBelowPlayer;
        layerPool.Capacity = 2;

        currentMinLayer = 0;
        currentMaxLayer = 0;
        GenerateLayer(0);
    }

    void Update()
    {
        var playerLayer = GetLayer(player.transform);

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

    private int GetLayer(Transform transform)
    {
        return Mathf.FloorToInt((transform.position.y - yOrigin) / layerHeight);
    }

    private void DestroyLayer(int i)
    {
        Debug.Log($"Destroying layer {i}");

        var idx = LayerBinarySearch(i);
        
        if (idx < 0) return;

        var layer = layers[idx];
        Debug.Assert(layer.level == i);

        layers.RemoveAt(idx);

        foreach (var cloud in layer.clouds)
        {
            cloud.transform.SetParent(null);
            cloud.SetActive(false);
            cloudPool.Add(cloud);
        }

        foreach (var obj in layer.objects)
        {
            if (!obj.PreventDespawn)
            {
                Destroy(obj.gameObject);
            }
        }

        layer.clouds.Clear();
        layer.objects.Clear();

        layer.container.SetActive(false);

        layerPool.Add(layer);
    }

    private void GenerateLayer(int level)
    {
        Debug.Log($"Generating layer {level}");

        var multiplier = Mathf.Lerp(startingMultiplier, 1f, (float)level / (float)lastMultiplierLayer);

        var numClouds = (int)(multiplier * cloudsPerLayer);

        var expectedClouds = numClouds * 8; // actually expected to be 4 x, but doubled to minimize allocations

        var expectedObjects = numClouds * 4; // actually expected to be 2 x, but doubled to minimize allocations

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
                container = new GameObject(),
            };
            layer.container.transform.SetParent(transform);
        }

        for (int i = 0; i < numClouds; ++i)
        {
            var radius = (Random.value + Random.value) / 2f * (layerRadius - deadzoneRadius) + deadzoneRadius;
            var angleRad = Random.Range(0f, Mathf.PI * 2f);

            var x = Mathf.Sin(angleRad) * radius;
            var z = Mathf.Cos(angleRad) * radius;

            var coord = new Vector3(x, Random.Range(0f, layerHeight) + level * layerHeight + yOrigin, z);

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

        var idx = LayerBinarySearch(layer.level);
        Debug.Assert(idx < 0);
        layers.Insert(~idx, layer);

        Debug.Log($"Layer {level} stats: numClouds = {numClouds}, clouds.Count = {layer.clouds.Count}, objects.Count = {layer.objects.Count}");

        void SpawnCloud(Vector3 coord)
        {
            var rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

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

            if (Random.value < .1f)
            {
                var crate = Instantiate(cratePrefab, coord + new Vector3(0, 1, 0), Quaternion.identity);
                var obj = crate.GetComponent<LayerObject>();
                Debug.Assert(obj != null);
                layer.objects.Add(obj);
            }

            if (Random.value < .4f)
            {
                var balloon = Instantiate(balloonPrefab, coord + new Vector3(0, 3, 0), Quaternion.identity);
                var obj = balloon.GetComponent<LayerObject>();
                Debug.Assert(obj != null);
                layer.objects.Add(obj);
            }

            layer.clouds.Add(cloud);
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
