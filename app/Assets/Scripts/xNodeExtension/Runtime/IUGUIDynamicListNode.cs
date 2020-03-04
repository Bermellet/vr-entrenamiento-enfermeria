using UnityEngine;
using XNode;

public interface IUGUIDynamicListNode {
    void DeleteInstanceInput(UGUIPort port);
    void DeleteInstanceInput(string portName);
    void AddRule();
}