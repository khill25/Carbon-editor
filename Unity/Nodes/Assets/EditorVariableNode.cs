using System;
using UnityEngine;
using System.Collections;
using WyrmTale;
using UnityEngine.UI;

public class EditorVariableNode : EditorNodeBase {

	protected override void init() {
		Debug.Log ("EditorVariableNode init");
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

		// TODO disable the editor text field
//		if (state == EditorNodeBase.EditorNodeState.Dragging) {
//
//		}

		// TODO if we click/drag on the panel here we want to be able to
		// drag this node around

	}

	public override object ConvertFromJson(JSON json) {
		return (EditorVariableNode)json;
	}
	
	public static explicit operator EditorVariableNode(JSON value) {
		checked {
			JSON jsTransform = value.ToJSON("transform");
			string type = (string)value[NodeTypeKey];
			
			GameObject obj = Instantiate(Resources.Load(type, typeof(GameObject))) as GameObject;
			obj.SetActive(true);
			EditorVariableNode node = obj.GetComponent(type) as EditorVariableNode;
			
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
