using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Scriptable Objects/Inactive Scene Stack")]
public class InactiveSceneStack : ScriptableObject
{
    private List<SceneRep> scenes = new List<SceneRep>(8);

    public delegate void OnSceneResume();

    public void Push(Scene scene, OnSceneResume onSceneResume = null)
    {
        scenes.Add(new SceneRep{ scene = scene, onSceneResume = onSceneResume });

        foreach (var obj in scene.GetRootGameObjects())
        {
            obj.SetActive(false);
        }
    }

    public Scene Pop()
    {
        var sceneRep = scenes[scenes.Count - 1];
        scenes.RemoveAt(scenes.Count - 1);

        foreach (var obj in sceneRep.scene.GetRootGameObjects())
        {
            obj.SetActive(true);
        }

        if (sceneRep.onSceneResume != null)
        {
            sceneRep.onSceneResume();
        }

        return sceneRep.scene;
    }

    private struct SceneRep
    {
        public Scene scene;
        public OnSceneResume onSceneResume;
    }
}
