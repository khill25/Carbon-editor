using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class OverlayUI : MonoBehaviour {

	public NodePrefab prefab;
	public List<NodePrefab> nodes = new List<NodePrefab>();
	public Text currentNodeCountText;

	int lastCount = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		// Only update the text if it changed
		if (lastCount != nodes.Count) {
			currentNodeCountText.text = "Current node count: " + nodes.Count;
			lastCount = nodes.Count;
		}
	
	}

	public void addNewNode() {

		Debug.Log ("added new node from menu");
		// Create a new node prefab and attach it to the mouse cursor
		NodePrefab instance = Instantiate<NodePrefab>(prefab);
		instance.tag = "SFNode";
		nodes.Add (instance);
		instance.updateState (NodePrefab.NodeState.DraggingNode);

	}

	public void AddNewEditorNode() {
		Debug.Log ("added new node from menu");
		// Create a new node prefab and attach it to the mouse cursor

		EditorNodeSelectionNode p = Resources.Load<EditorNodeSelectionNode>("EditorNodeSelectionNode");
		EditorNodeSelectionNode instance = Instantiate<EditorNodeSelectionNode>(p);
		instance.gameObject.SetActive(true);
		instance.ChangeState(EditorNodeBase.EditorNodeState.DraggingStarted);
	}
}
