using System;
using UnityEngine;
using System.Collections.Generic;
using WyrmTale;
using UnityEngine.UI;

public class EditorSelectorNode : EditorNodeBase {

	protected override void init() {
		base.init();
		valueText = gameObject.GetComponentInChildren<Text>();
		valueInputField = gameObject.GetComponentInChildren<InputField>();
	}

	void Awake() {
		base.init ();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		base.BaseUpdate();
	}

	public override object ConvertFromJson(JSON json) {
		return (EditorSelectorNode)json;
	}

	public static explicit operator EditorSelectorNode(JSON value) {
		checked {
			JSON jsTransform = value.ToJSON("transform");
			string type = (string)value[NodeTypeKey];
			
			GameObject obj = Instantiate(Resources.Load(type, typeof(GameObject))) as GameObject;
			obj.SetActive(true);
			EditorSelectorNode node = obj.GetComponent(type) as EditorSelectorNode;
			
			node.nodeId = value.ToInt(NodeIdKey);
			
			if (node.nodeId > NextNodeId) {
				NextNodeId = node.nodeId + 1;// Increment the next node id counter
			}

			try {
				if(node.valueInputField != null) {
					node.valueInputField.text = (string)value[ValueKey];
				} else {
					node.valueText.text = (string)value[ValueKey];
				}
			} catch (NullReferenceException e) {
				Debug.Log ("Unable to set value text. " + e.Message);
			}
			
			// load the basic transform data
			node.gameObject.transform.position = (Vector3)jsTransform.ToJSON("position");
			node.gameObject.transform.localScale = (Vector3)jsTransform.ToJSON("localScale");
			node.gameObject.transform.rotation = (Quaternion)jsTransform.ToJSON("rotation");
			
			// TODO make sure that this will rebuild the line renderers for this node
			int[] children = value.ToArray<int>(NodeConnectionIdsKey);
			foreach(int child in children) {
				node.Connections.Add (child, null);
			}
			
			return node;
		}
	}            

}
