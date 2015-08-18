using UnityEngine;
using System.Collections;

public class EditorNodeSelectionNode : EditorNodeBase {

	protected EditorVariableNode variableNode;
	protected EditorSelectorNode selectorNode;

	new void init(){
		base.init ();
		variableNode = Resources.Load<EditorVariableNode>("EditorVariableNode");
		selectorNode = Resources.Load<EditorSelectorNode>("EditorSelectorNode");
	}

	void Awake() {
		init ();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		base.BaseUpdate();
	}

	public void CreateOperatorNodeClicked() {
		Debug.Log("CreateOperatorNodeClicked");
		EditorSelectorNode node = Instantiate<EditorSelectorNode>(selectorNode);

		node.gameObject.transform.position = this.transform.position;
		node.gameObject.transform.SetParent(this.transform.parent);
		node.gameObject.SetActive(true);

		//node.ChangeState(EditorNodeState.DraggingStarted);
		
		Destroy(this.gameObject);
	}

	public void CreateVariableNodeClicked() {
		Debug.Log("CreateVariableNodeClicked");
		EditorVariableNode node = Instantiate<EditorVariableNode>(variableNode);

		node.gameObject.transform.position = this.transform.position;
		node.gameObject.transform.SetParent(this.transform.parent);
		node.gameObject.SetActive(true);

		//node.ChangeState(EditorNodeState.DraggingStarted);

		Destroy(this.gameObject);
	}
}
