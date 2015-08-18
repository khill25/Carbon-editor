using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using WyrmTale;

public class NodePrefab : MonoBehaviour, ISFEditingDelegate, ISaveable, ILoadable<NodePrefab>, INodeBase{

	public enum NodeState {
		None = 0,
		DraggingNode,
		DraggingEnding,
		DraggingConnection,
		DraggingConnectionEnding,
		Editing,
		EditingEnding
	}

	public int nodeId = 0;
	public static int nextNodeId = 1;

	public SFLineRenderer lineRendererPrefab;
	public Renderer nodeMeshRenderer;
	public Color startColor;
	public Color endColor;

	// References for the stuffs
	public Dictionary<int, SFLineRenderer> lineRenderers;
	public List<NodePrefab> childNodes;
	public List<NodePrefab> connectedFromNodes; // Nodes that connected to this, sorta like a parent relationship
	protected List<int> childrenIds;
	protected List<int> parentIds;

	//Temp for line dragging and dragging in general
	private SFLineRenderer currentlyDraggingLine;
	public Vector3 offset;

	// State variables
	public NodeState currentState;
	public bool isSelected;
	private bool shouldTransitionToNextState;
	private NodeState nextState;

	private SFInputField inputField = null;
	private bool shouldEnableInputField = false;
	private bool shouldDisableInputField = false;

	public List<Vector3> connectionAnchorPoints;

	bool hasInited = false;
	void init() {
		if (hasInited) return;

		hasInited = true;
		Debug.Log("NodePrefab init");
		if (nodeId == 0) {
			nodeId = nextNodeId;
			nextNodeId++;
		}
		if (childNodes == null) childNodes = new List<NodePrefab> ();
		if (lineRenderers == null) lineRenderers = new Dictionary<int, SFLineRenderer> ();
		((Renderer)gameObject.GetComponent(typeof(Renderer))).material.color = new Color (.05f,.05f,.05f);

		Canvas canvas = gameObject.GetComponentInChildren<Canvas>();
		SFInputField[] comps = canvas.GetComponentsInChildren<SFInputField>();
		if (comps.Length > 0 && inputField == null) {
			inputField = comps [0];
			inputField.subscribeToEditingUpdates(this);
			inputField.subscribeToEditingUpdates(Camera.main.GetComponent<MoveCamera>());
		}
	}

	// Use this for initialization
	void Start () {
	}

	void Awake() {
		init ();
	}

	public void updateState(NodeState newState) {
		nextState = newState;
		shouldTransitionToNextState = true;
	}
	
	// Update is called once per frame
	void Update () {

		if (shouldTransitionToNextState) {
			currentState = nextState;
			shouldTransitionToNextState = false;
		}

		switch (currentState) {
			case NodeState.DraggingNode: {
				doDragging ();
				break;
			}
			case NodeState.DraggingEnding: {
				nextState = NodeState.None;
				shouldTransitionToNextState = true;
				break;
			}
			case NodeState.DraggingConnection: {
				doConnectionDragging();
				break;
			}
			case NodeState.DraggingConnectionEnding: {
				nextState = NodeState.None;
				shouldTransitionToNextState = true;
				break;
			}

			case NodeState.Editing: {
				break;
			}

			case NodeState.EditingEnding: {
				nextState = NodeState.None;
				shouldTransitionToNextState = true;
				break;
			}


		case NodeState.None:
			checkAndSetSelected();
			break;

		default:
			checkAndSetSelected();
			break;
		}

		if (shouldEnableInputField) {
			// Allow this to be editing now
			if (inputField != null) {
				Debug.Log ("Enabling inputField interactable");
				inputField.enabled = true;
				inputField.interactable = true;
			}
			shouldEnableInputField = false;
		} else if (shouldDisableInputField) {

			if (this.inputField != null) {
				Debug.Log ("Disabling inputField interactable");
				inputField.enabled = false;
				inputField.interactable = false;
			}
			shouldDisableInputField = false;
		}

		renderSelected();
	}

	protected void renderSelected() {
		if (isSelected) {
			// Render some cool shit
			nodeMeshRenderer.material.shader = Shader.Find ("Toon/Lit Outline");

			// Don't allow deletes if we are editing
			if (currentState != NodeState.Editing) {
				if (Input.GetKeyUp(KeyCode.Backspace) || Input.GetKeyUp(KeyCode.Delete)) {
					// Delete this 
					foreach(NodePrefab child in childNodes) {
						child.removeParent(this);
					}
					
					foreach(NodePrefab connected in connectedFromNodes) {
						connected.removeParent(this);
					}
					
					foreach (SFLineRenderer line in lineRenderers.Values) {
						Destroy(line.gameObject);
					}
					
					Destroy(this.gameObject);
				}
			}
			
		} else {
			nodeMeshRenderer.material.shader = Shader.Find ("Standard");
		}
	}

	protected void doDragging() {
		var mousePos = Input.mousePosition;
		mousePos.z = Mathf.Abs (Camera.main.transform.position.z); // select distance = 15 units from the camera
		Vector3 transformPos = Camera.main.ScreenToWorldPoint (mousePos);
		Debug.Log (transformPos + offset);
		gameObject.transform.position = transformPos + offset;
		
		updateConnectionsToChildren ();
		updateConnectedFromNodes ();
	}

	protected void doConnectionDragging() {
		updateCurrentBezierCurve ();
		
		if (Input.GetKeyDown (KeyCode.Escape)) {
			// Disable the line renderer
			//currentlyDraggingLine
			currentlyDraggingLine.gameObject.SetActive (false);
			GameObject.Destroy (currentlyDraggingLine.gameObject);
			currentlyDraggingLine = null;

			nextState = NodeState.DraggingConnectionEnding;
			shouldTransitionToNextState = true;
			
		} else if (Input.GetMouseButtonDown (0)) {
			// Raycast and find the node it clicked on
			// if it's a valid node then get it
			// set the destination
			// and add this into it's list of
			// connected nodes
			
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, Mathf.Abs (Camera.main.transform.position.z) + 20)) {
				NodePrefab destination = hit.collider.GetComponent<NodePrefab> ();
				connectedToDestinationNode(destination);
			}
			
		}
	
	}

	protected void connectedToDestinationNode(NodePrefab destination) {
		
		childNodes.Add (destination);
		destination.connectedFromNodes.Add (this);
		currentlyDraggingLine.updateBezierPath (this, destination);
		destination.lineRenderers.Add (this.nodeId, currentlyDraggingLine);
		lineRenderers.Add (destination.nodeId, currentlyDraggingLine);
		
		nextState = NodeState.DraggingConnectionEnding;
		shouldTransitionToNextState = true;
		
	}

	protected bool checkAndSetSelected() {
		bool wasSelected = isSelected;
		if (Input.GetMouseButtonUp(0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, Mathf.Abs (Camera.main.transform.position.z) + 20)) {
				NodePrefab nodeHit = hit.collider.GetComponent<NodePrefab> ();
				if (nodeHit == this) {
					isSelected = true;
				} else {
					isSelected = false;
				}
			} else {
				isSelected = false;
			}
		}

		if (wasSelected && !isSelected) {
			shouldDisableInputField = true;
		}

		if (!wasSelected && isSelected) {
			shouldEnableInputField = true;
		}

		return isSelected;
	}

	public void removeParent(NodePrefab parent) {
		if (this.lineRenderers.ContainsKey (parent.nodeId)) {
			Destroy (this.lineRenderers [parent.nodeId].gameObject);
			this.lineRenderers.Remove(parent.nodeId);
		}

		bool removedFromChildNodes = this.childNodes.Remove (parent);
		bool removedFromConnectedFromNodes = this.connectedFromNodes.Remove (parent);
	}

	public void bodyTextClicked() {

		// Make the text editable

	}

	public void headerTextClicked() {

	}

	public void updateConnectedFromNodes() {
		// For each connected parent type node
		// loop through and update the bezier paths
		// so the lines stay up to date
		foreach(NodePrefab parent in connectedFromNodes){
			parent.updateConnectionOfChild(this);

		}
	}

	public void updateConnectionOfChild(NodePrefab child) {
		lineRenderers [child.nodeId].updateBezierPath (this, child);
	}


	public void updateCurrentBezierCurve() {
		if (currentlyDraggingLine != null) {
			Vector3 mousePos = Input.mousePosition;
			mousePos.z = Mathf.Abs (Camera.main.transform.position.z);
			mousePos = Camera.main.ScreenToWorldPoint (mousePos);
			//Debug.Log (mousePos);
			currentlyDraggingLine.updateBezierPath (gameObject.transform.position, mousePos);
		}
	}

	public void updateConnectionsToChildren() {
		foreach(NodePrefab child in childNodes){
			updateConnectionOfChild(child);
			
		}
	}
	
	public void addNewConnectionButtonPressed() {
		// Select destination node
		this.currentlyDraggingLine = Instantiate<SFLineRenderer> (lineRendererPrefab);
		this.currentlyDraggingLine.gameObject.SetActive (true);

		Vector3 mousePos = Input.mousePosition;
		mousePos.z = Mathf.Abs (Camera.main.transform.position.z);
		mousePos = Camera.main.ScreenToWorldPoint(mousePos);
		this.currentlyDraggingLine.addBeginingBezierPathPoints (gameObject.transform.position, mousePos);

		nextState = NodeState.DraggingConnection;
		shouldTransitionToNextState = true;
	}

	public void didStartDragging() {
		Debug.Log ("didStartDragging");

		Vector3 originalMousePos = Input.mousePosition;
		originalMousePos.z = Mathf.Abs (Camera.main.transform.position.z);;
		offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(originalMousePos);
		shouldTransitionToNextState = true;
		nextState = NodeState.DraggingNode;
	}

	public void clickEndsDragging() {
		if (currentState == NodeState.DraggingNode) {
			Debug.Log ("draggingEnded");

			offset = new Vector3 ();
			nextState = NodeState.DraggingEnding;
			shouldTransitionToNextState = true;
		}
	}

	public void setIsEditingDelegateValue(bool value) {
		if (value) {
			nextState = NodeState.Editing;
		} else {
			nextState = NodeState.EditingEnding;
		}

		shouldTransitionToNextState = true;
	}

	public void SetupConnections() {

		foreach (int childId in childrenIds) {

			// Find the child nodes
			GameObject childObject = ObjectManager.instance.getGameObjectNodeWithNodeId(childId);
			NodePrefab child = childObject.GetComponent<NodePrefab>();
			this.childNodes.Add (child);

			SFLineRenderer line = Instantiate<SFLineRenderer> (lineRendererPrefab);
			line.gameObject.SetActive(true);
		
			child.lineRenderers.Add (this.nodeId, line);
			lineRenderers.Add (child.nodeId, line);

			line.addBezierPath(this, child);
		}

		foreach (int parentId in parentIds) {
			
			// Find the child nodes
			GameObject parentObject = ObjectManager.instance.getGameObjectNodeWithNodeId(parentId);
			NodePrefab parent = parentObject.GetComponent<NodePrefab>();
			this.connectedFromNodes.Add (parent);
		}

	}

	public GameObject getGameObject() {
		return this.gameObject;
	}

	public JSON getJson() {
		return this;
	}

	public static implicit operator JSON(NodePrefab value) {
		GameObject g = value.gameObject;
		JSON nodeJson = new JSON ();
        if (g != null) {
			JSON js = new JSON();
            JSON jsTransform = new JSON();
            js["transform"] = jsTransform;
            jsTransform["position"] = (JSON)g.transform.position;
            jsTransform["localScale"] = (JSON)g.transform.localScale;
            jsTransform["rotation"] = (JSON)g.transform.rotation;
			js["nodeId"] = value.nodeId;
			js["nodeValue"] = value.inputField.text;

			js["classType"] = "NodePrefab";

			// TODO make sure that this is actually gonna work
			List<int> cIds = new List<int>();
			foreach(NodePrefab child in value.childNodes) {
				cIds.Add (child.nodeId);
			}
			js["childrenIds"] = cIds.ToArray();


			List<int> ids = new List<int>();
			foreach(NodePrefab parent in value.connectedFromNodes) {
				ids.Add(parent.nodeId);
			}
			js["parentIds"] = ids.ToArray();

			nodeJson["node"] = js;

        }          
        return nodeJson;
	}

	//public NodePrefab ConvertFromJson(JSON json) {
	//	return (NodePrefab)json;
	//}

	public object ConvertFromJson(JSON json) {
		return (NodePrefab)json;
	}

	// JSON to class conversion
	public static explicit operator NodePrefab(JSON value) {
		checked {
			JSON jsTransform = value.ToJSON("transform");
			GameObject o = Resources.Load("NodePrefab", typeof(GameObject)) as GameObject;
			GameObject obj = Instantiate(o) as GameObject;
			NodePrefab node = obj.GetComponent("NodePrefab") as NodePrefab;
			node.init();

			node.nodeId = value.ToInt("nodeId");
			
			if (node.nodeId > nextNodeId) {
				nextNodeId = node.nodeId + 1;// Increment the next node id counter
			}

			node.inputField.text = (string)value["nodeValue"];
			node.gameObject.transform.position = (Vector3)jsTransform.ToJSON("position");
			node.gameObject.transform.localScale = (Vector3)jsTransform.ToJSON("localScale");
			node.gameObject.transform.rotation = (Quaternion)jsTransform.ToJSON("rotation");

			// TODO make sure that this will rebuild the line renderers for this node
			int[] children = value.ToArray<int>("childrenIds");
			node.childrenIds = new List<int>(children);

			// TODO create the parents connection manually??
			int[] parents = value.ToArray<int>("parentIds");
			node.parentIds = new List<int>(parents);

			ObjectManager.instance.RegisterNewNodeObject(node);

			return node;
		}
	}               
	
	// convert a JSON array to a NodePrefab Array
	public static NodePrefab[] Array(JSON[] array) {
		List<NodePrefab> tc = new List<NodePrefab>();
		for (int i=0; i<array.Length; i++) {
			tc.Add ((NodePrefab)array [i]);
		}
		return tc.ToArray();
	}       

}
