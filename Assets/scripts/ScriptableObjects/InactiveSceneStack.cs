using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Scriptable Objects/Inactive Scene Stack")]
public class InactiveSceneStack : ScriptableObject
{
    private List<Scene> scenes = new List<Scene>(8);

    public void Push(Scene scene)
    {
        scenes.Add(scene);

        foreach (var obj in scene.GetRootGameObjects())
        {
            obj.SetActive(false);
        }
    }

    public Scene Pop()
    {
        var scene = scenes[scenes.Count - 1];
        scenes.RemoveAt(scenes.Count - 1);

        foreach (var obj in scene.GetRootGameObjects())
        {
            obj.SetActive(true);
        }

        return scene;
    }
}
