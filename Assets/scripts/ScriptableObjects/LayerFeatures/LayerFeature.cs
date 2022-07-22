using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class LayerFeature : ScriptableObject
{
    public abstract void TrySpawn(CloudLayerGenerator generator, int level);
}
