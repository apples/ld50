using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class LayerFeature : ScriptableObject
{
    [Range(0, 100)]
    public int chancePerLayer;

    public abstract void Spawn(CloudLayerGenerator generator, int level);
}
