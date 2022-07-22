using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Scriptable Objects/Layer Features/Door Cloud Platform")]
public class DoorCloudPlatformLayerFeature : LayerFeature
{
    [Range(0, 100)]
    public int chancePerLayer;

    public GameObject prefab;

    public float radiusRatio;

    public List<string> doorChallengeScenes;

    public override void TrySpawn(CloudLayerGenerator generator, int level)
    {
        if (chancePerLayer <= Random.value * 100f)
        {
            return;
        }

        Debug.Assert(doorChallengeScenes.Count > 0);

        int playerLevel = PersistentDataManager.Instance.Data.playerLevel;

        if (playerLevel >= 4)
        {
            var (minY, maxY) = generator.GetLayerBoundsY(level);

            var radius = (generator.layerRadius - generator.deadzoneRadius) * radiusRatio + generator.deadzoneRadius;
            var angleRad = Random.Range(0f, Mathf.PI * 2f);

            var x = Mathf.Sin(angleRad) * radius;
            var z = Mathf.Cos(angleRad) * radius;

            var coord = new Vector3(x, Random.Range(minY, maxY), z);
            var rotation = Quaternion.LookRotation(-(Vector3.Scale(coord, new Vector3(1, 0, 1))).normalized, Vector3.up);

            var obj = generator.CreateLayerStructure(prefab, coord, rotation, level);

            var sceneName = doorChallengeScenes[Random.Range(0, Mathf.Min((playerLevel / 2) - 1, doorChallengeScenes.Count))];

            obj.GetComponentInChildren<DoorController>().warpToScene = sceneName;
        }
        
    }
}
