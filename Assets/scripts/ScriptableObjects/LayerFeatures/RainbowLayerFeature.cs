using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Scriptable Objects/Layer Features/Rainbow")]
public class RainbowLayerFeature : LayerFeature
{
    public GameObject prefab;

    public float maxCircleScore = 0.75f;
    public float minPlanarCloudDistance = 20f;
    public float maxPlanarCloudDistance = 50f;
    public int maxAttempts = 10;

    public override void Spawn(CloudLayerGenerator generator, int level)
    {
        var (minY, maxY) = generator.GetLayerBoundsY(level);

        var rainbowObj = generator.CreateLayerStructure(prefab, Vector3.zero, Quaternion.identity, level);
        var rainbow = rainbowObj.GetComponentInChildren<RainbowController>();
        Debug.Assert(rainbow != null);

        var radiusExtent = (generator.layerRadius - generator.deadzoneRadius);

        var distScale = new Vector3(1, 0, 1);

        bool IsGood()
        {
            if (rainbow.GetCircle().score > maxCircleScore) return false;

            var cloudPlanarDist = Vector3.Distance(Vector3.Scale(rainbow.startCloud.position, distScale), Vector3.Scale(rainbow.endCloud.position, distScale));
            
            if (cloudPlanarDist < minPlanarCloudDistance) return false;

            var endCloudRadius = Vector3.Distance(Vector3.Scale(rainbow.endCloud.position, distScale), Vector3.zero);

            if (endCloudRadius < generator.deadzoneRadius || endCloudRadius > generator.layerRadius) return false;

            return true;
        }

        Vector3 GenCoord(float extent, float deadzone)
        {
            var coord = Random.insideUnitCircle;
            coord = coord * extent + coord.normalized * deadzone;
            return new Vector3(coord.x, Random.Range(minY, maxY), coord.y);
        }

        for (var i = 0; i < maxAttempts; ++i)
        {
            rainbow.startCloud.position = GenCoord(radiusExtent, generator.deadzoneRadius);
            rainbow.endCloud.position = GenCoord(maxPlanarCloudDistance - minPlanarCloudDistance, minPlanarCloudDistance);

            if (IsGood()) return;
        }

        Destroy(rainbowObj);
    }
}
