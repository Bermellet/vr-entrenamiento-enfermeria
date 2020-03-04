using System;
using System.Collections.Generic;
using System.Linq;
using NT;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XNode;
using TMPro;
using System.Runtime.CompilerServices;
using System.Collections;

public class UGUIBaseNode :  MonoBehaviour, IDragHandler, IUGUINode, IContextItem {
    public Node node;
    public RuntimeGraph graph;

    public GameObject body;
    public List<UGUIPort> ports = new List<UGUIPort>();

    
    private Image baseImage;

    private void Awake() {
        if(node != null){
            List<string> ignored = new List<string>();
            Dictionary<string, GameObject> listNodes = new Dictionary<string, GameObject>();
            //List<GameObject> listNodes = new List<GameObject>();

            foreach (NodePort port in node.Ports)
            {
                GameObject portGO;
                if (CheckTypeIsGenericList(port.ValueType))
                {
                    portGO = Instantiate(graph.dynamicList, body.transform);
                    SetDefaultLabelText(portGO, port.fieldName);
                    listNodes.Add(port.fieldName, portGO);
                }
                else if (port.IsDynamic && port.fieldName.StartsWith("#List"))
                {
                    // Dynamic list elements
                    string[] portName = port.fieldName.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                    GameObject dl = listNodes[portName[1]];
                    // Instantiate port in list's Layout
                    var layout = dl.GetComponentInChildren<LayoutGroup>();
                    portGO = Instantiate(graph.dynamicPort, layout.transform);
                    SetDefaultLabelText(portGO, portName[2]);
                }
                else
                {
                    portGO = Instantiate(port.direction == NodePort.IO.Input ? graph.inputPort : graph.outputPort, body.transform);
                    SetDefaultLabelText(portGO, port.fieldName);
                    SetDefaultPortText(portGO, port);
                }
                ignored.Add(port.fieldName);
            }

            transform.Find("Header/Title").GetComponent<TextMeshProUGUI>().text = node.name;


            var d = ReflectionUtilities.DesgloseInBasicTypes(node.GetType(), ignored);

            foreach(KeyValuePair<Type, List<string>> kvp in d){
                foreach(string variable in kvp.Value){
                    GameObject variableGo =  Instantiate(graph.Property, body.transform);


                    GUIProperty  gp = variableGo.GetComponent<GUIProperty>();

                    object value = ReflectionUtilities.GetValueOf(variable.Split('/').ToList(), node);

                    if(kvp.Key.IsString()){
                        gp.SetData(value, variable, GUIProperty.PropertyType.String);
                    }
                    else if(kvp.Key.IsNumber())
                    {
                        gp.SetData(value, variable, GUIProperty.PropertyType.Number);
                    }
                    else if(kvp.Key.IsBool())
                    {
                        gp.SetData(value, variable, GUIProperty.PropertyType.Boolean);
                    }
                    else if(kvp.Key.IsEnum)
                    {
                        gp.SetData(value, variable, GUIProperty.PropertyType.Enumeration);
                    }
                    else if (kvp.Key is ITuple)
                    {
                        gp.SetData(value, variable, GUIProperty.PropertyType.Tuple);
                    }

                    gp.OnValueChanged.RemoveAllListeners();
                    gp.OnValueChanged.AddListener(PropertyChanged);
                }
            }

            if(node is NTNode){
                ( (RectTransform) transform).sizeDelta = new Vector2( ( (NTNode) node).GetWidth() , ((RectTransform) transform).sizeDelta.y) ;
            }
        }
        else
        {
            gameObject.SetActive(false);
        }

        ntNode = node as NTNode;
    }

    private void SetDefaultPortText(GameObject portGO, NodePort port)
    {
        UGUIPort guiport = portGO.GetComponentInChildren<UGUIPort>();
        guiport.fieldName = port.fieldName;
        guiport.node = node;
        guiport.name = port.fieldName;

        ports.Add(guiport);
    }

    private void SetDefaultLabelText(GameObject portGO, string labelText)
    {
        portGO.transform.Find("Label").GetComponent<Text>().text = labelText.NicifyString();
    }

    private void PropertyChanged(object value, string path)
    {
        object n = node;
        ReflectionUtilities.SetValueOf(ref n, value, new List<string>(path.Split('/')) );
    }

    public void OnDrag(PointerEventData eventData) {

    }

    public virtual UGUIPort GetPort(string fieldName, Node n)
    {
        for (int i = 0; i < ports.Count; i++) {
            if (ports[i].name == fieldName) return ports[i];
        }
        return null;
    }

    public void SetColor()
    {
        GetComponent<Image>().color = graph.GetColorFor(node.GetType());
    }

    public virtual void UpdateGUI()
    {
        //throw new NotImplementedException();
    }

    bool executing = false;
    NTNode ntNode;

    private void LateUpdate() {
        foreach (UGUIPort port in ports) port.UpdateConnectionTransforms();

        if(ntNode != null){
            if( ntNode.isExecuting && !executing){
                executing = true;
                GetComponent<Image>().color = Color.magenta;
            }

            if(!ntNode.isExecuting && executing){
                executing = false;
                SetColor();
            }
        }
    }

    public bool HasNode(Node node)
    {
        return this.node == node;
    }

    public Node GetNode()
    {
        return this.node;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public RuntimeGraph GetRuntimeGraph()
    {
        return graph;
    }

    public void SetPosition(Vector2 position)
    {
        node.position = position;
    }

    public void RemoveNode()
    {
	    graph.graph.RemoveNode(node);
    }

    public Node DuplicateNode()
    {
        Node newNode = graph.graph.CopyNode(node);
        newNode.position = node.position + new Vector2(100, 200);
        return newNode;
    }

    public void Remove()
    {
       RemoveNode();
    }

    public string GetKey()
    {
        return node.name;
    }

    public bool CheckTypeIsGenericList(Type t)
    {
        return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>);
    }
}