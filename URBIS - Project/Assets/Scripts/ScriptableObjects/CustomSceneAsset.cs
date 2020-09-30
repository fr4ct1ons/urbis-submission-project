using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Auxiliary class used to store a scene as an asset.
/// By using this ScriptableObject, you can change any scene name and by changing only this asset's name/index, every asset changes accordingly.
/// </summary>
[CreateAssetMenu(fileName = "New scene asset", menuName = "Scene asset")]
public class CustomSceneAsset : ScriptableObject
{
    [SerializeField] private int sceneIndex;

    [SerializeField] private string sceneName;

    public int SceneIndex => sceneIndex;
    public string SceneName => sceneName;
}
