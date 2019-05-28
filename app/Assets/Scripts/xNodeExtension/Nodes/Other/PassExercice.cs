
using NT.Atributes;

namespace NT.Nodes.Other {
    
    public class PassExercice : FlowNode {

        [NTInput] public int grade; 
        
        public object GetValue() {
            return GetInputValue<object>("input");
        }

        public override string GetDisplayName(){
            return "Pass Exercice";
        }
    }
}
