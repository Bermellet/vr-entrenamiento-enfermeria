

using System;
using System.Collections.Generic;
using NT.Variables;
using UnityEngine;

namespace NT.SceneObjects
{
    public class NTSceneObject<T> : NTVariable<T>, ISceneObject
    {
        public List<string> GetCallbacks()
        {
            throw new NotImplementedException();
        }

        public List<Type> GetCompatibleNodes()
        {
            throw new NotImplementedException();
        }

        public GameObject GetModel()
        {
            throw new NotImplementedException();
        }

        public Vector2 GetSize()
        {
            throw new NotImplementedException();
        }
    }
}