using System.Collections.Generic;
using UnityEngine;

namespace NT.SceneObjects{
    public struct MetallicBoxData{
        public List<SceneGameObjectReference> toolsList;
    }

    [CreateAssetMenu(fileName = "MetallicBoxSceneObject", menuName = "NT/Scene/MetallicBox")]
    public class MetallicBoxSceneObject : SceneObject<MetallicBoxData> {
        
        public override List<string> GetCallbacks()
        {
            return new List<string>() { "Grabbed" };
        }
    }
}