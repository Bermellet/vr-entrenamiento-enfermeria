using NT.Atributes;
using UnityEngine;
using XNode;

namespace NT.Nodes.Other {

    public class CompareTypes : NTNode {

        [Input(ShowBackingValue.Never, ConnectionType.Override)] public SceneGameObject scneObject;
        
        [NTOutput] public bool result;

        [NTInput] public Tools tool;



        public override object GetValue(NodePort port) {
            if(port.fieldName == nameof(result) ){
                SceneGameObject scgo = GetInputValue<SceneGameObject>(nameof(scneObject), null);

                if(scgo != null && scgo is ITool){
                    ITool t = (ITool) scgo;

                    Debug.Log( t + " __ " + tool);

                    return t?.GetToolType() == tool;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return null;
            }
        }

        public override string GetDisplayName(){
            return "Object is type of";
        }

        public override int GetWidth(){
            return 300;
        }
    }
}
