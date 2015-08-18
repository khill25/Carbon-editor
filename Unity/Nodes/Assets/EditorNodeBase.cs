using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using WyrmTale;

public class EditorNodeBase : MonoBehaviour, INodeBase, ISaveable, ILoadable<EditorNodeBase> {

	public float lineZOffset = .5f;
	public static string NodeTypeKey = "classType";
	public static string NodeIdKey = "nodeId";
	public static string ValueKey = "nodeValue";
	public static string NodeConnectionIdsKey = "childrenIds";
	public static string NodeNodeKey = "node";

	public static int NextNodeId = 0;

	public int nodeId;
	public SFLineRenderer linePrefab;
	public string nodeValue;
	// TODO want to be able to figure out what kind of object nodeValue is
	// Maybe this is in the form of GUID of the value it's storing

	protected List<Vector3> anchorPoints = new List<Vector3>();
	protected SFLineRenderer tempLineRenderer;
	protected Dictionary<int, SFLineRenderer> Connections = new Dictionary<int, SFLineRenderer>(); 

	protected EditorNodeState state;
	protected EditorNodeState nextState;
	protected bool transitionToNextState;
	protected Text valueText;
	protected InputField valueInputField;

	public enum EditorNodeState {
		None = 0,
		DraggingStarted,
		Dragging,
		DraggingEnded,
		EditingStarted,
		Editing,
		EditingEnded,
		DraggingConnectionStarted,
		DraggingConnection,
		DraggingConnectionEnded
	}

	protected Vector3 draggingOffset;
	protected bool clickStarted;
	protected float clickBegan;
	
	bool hasInited = false;
	protected virtual void init() {

		if(hasInited) return;

		hasInited = true;

		Debug.Log("EditorNodeBase init");

		/*
		Vector3 current = this.gameObject.transform.localPosition;
		RectTransform rect = this.gameObject.GetComponent<RectTransform>();
		Vector2 size = rect.sizeDelta;
		Vector3 left = Vector3.zero;
		left.x = current.x;
		left.y = current.y + size.y / 2.0f;
		
		Vector3 right = Vector3.zero;
		right.x = current.x + size.x;
		right.y = current.y + size.y / 2.0f;
		
		Vector3 top = Vector3.zero;
		top.x = current.x + size.x / 2.0f;
		top.y = current.y;
		
		Vector3 bottom = Vector3.zero;
		bottom.x = current.x + size.x / 2.0f;
		bottom.y = current.y + size.y;
		
		anchorPoints.Add(left);
		anchorPoints.Add(top);
		anchorPoints.Add(right);
		anchorPoints.Add(bottom);
		*/

		if (Connections == null) Connections = new Dictionary<int, SFLineRenderer> ();
		//linePrefab = gameObject.GetComponent<SFLineRenderer>();
		valueText = gameObject.GetComponentInChildren<Text>();
		valueInputField = gameObject.GetComponentInChildren<InputField>();

		nodeId = NextNodeId++;

	}
	
	void Awake() {
		init ();
	}


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (transitionToNextState) {
			state = nextState;
			transitionToNextState = false;
		}

		switch (state) {

			case EditorNodeState.DraggingStarted: {
				Debug.Log ("DraggingStarted state moving to DraggingState");
				ChangeState(EditorNodeState.Dragging);
				break;
			}

			case EditorNodeState.Dragging: {
				DoDragging();
				break;
			}

			case EditorNodeState.DraggingEnded: {
				ChangeState(EditorNodeState.None);
				break;
			}

			case EditorNodeState.EditingStarted: {
				ChangeState(EditorNodeState.Editing);
				break;
			}

			case EditorNodeState.Editing: {
				// DoEditing()
				break;
			}

			case EditorNodeState.EditingEnded: {
				ChangeState(EditorNodeState.None);
				break;
			}

			case EditorNodeState.DraggingConnectionStarted: {
				ChangeState(EditorNodeState.DraggingConnection);
				break;
			}

			case EditorNodeState.DraggingConnection: {
				DoConnectionDragging();
				break;
			}

			case EditorNodeState.DraggingConnectionEnded: {
				ChangeState(EditorNodeState.None);
				break;
			}

		}

		// Ghetto event handling
		if(Input.GetMouseButtonDown(0)) {
			clickStarted = true;
			clickBegan = Time.realtimeSinceStartup;
			//Debug.Log("Click started");
		} else if (Input.GetMouseButtonUp(0) && clickStarted) {

			float now = Time.realtimeSinceStartup;
			float diff = now - clickBegan;
			//Debug.Log("Mouse up after " + diff + " seconds");
			if (diff > .08) {
				if (state == EditorNodeState.Dragging) {
					Debug.Log("Dragging ending");
					ChangeState(EditorNodeState.DraggingEnded);
				}
			}

			clickBegan = 0;
			clickStarted = false;
		} 

	}

	protected void BaseUpdate() {
		Update ();
	}

	public void AddNewNode() {
		// Create a new selector/variable editor node.
		// TODO use anchor points
		Debug.Log("Adding new EditorNode");
		EditorNodeSelectionNode prefab = Resources.Load<EditorNodeSelectionNode>("EditorNodeSelectionNode");
		EditorNodeSelectionNode node = Instantiate<EditorNodeSelectionNode>(prefab);

		node.gameObject.transform.position = this.transform.position;
		node.gameObject.transform.SetParent(this.transform.parent);
		node.gameObject.SetActive(true);
		node.StartDragging();

		// TODO line renderer to the new node
	}
	
	public void StartDragging() {
		ChangeState (EditorNodeState.DraggingStarted);
		this.draggingOffset = gameObject.transform.position - WorldMousePosition();
	}

	protected void DoDragging() {
		Vector3 current = WorldMousePosition();
		gameObject.transform.position = current + draggingOffset;
	}

	public void EndDragging() {
		ChangeState(EditorNodeState.DraggingEnded);
	}

	public void AddNewConnection() {
		this.tempLineRenderer = Instantiate<SFLineRenderer> (linePrefab);
		this.tempLineRenderer.gameObject.SetActive (true);
		
		Vector3 mousePos = WorldMousePosition();
		mousePos.z = lineZOffset;
		Vector3 position = gameObject.transform.position;
		position.z = lineZOffset;
		this.tempLineRenderer.addBeginingBezierPathPoints (position, mousePos);
		
		ChangeState(EditorNodeState.DraggingConnectionStarted);
	}

	protected void updateCurrentBezierCurve() {
		if (tempLineRenderer != null) {
			Vector3 mousePos = WorldMousePosition();
			mousePos.z = lineZOffset;
			Vector3 position = gameObject.transform.position;
			position.z = lineZOffset;
			tempLineRenderer.updateBezierPath (position, mousePos);
		}
	}

	protected void DoConnectionDragging() {
		updateCurrentBezierCurve ();
		
		if (Input.GetKeyDown (KeyCode.Escape)) {
			// Disable the line renderer
			//currentlyDraggingLine
			tempLineRenderer.gameObject.SetActive (false);
			GameObject.Destroy (tempLineRenderer.gameObject);
			tempLineRenderer = null;
			
			ChangeState(EditorNodeState.DraggingConnectionEnded);
			
		} else if (Input.GetMouseButtonDown (0)) {
			// Raycast and find the node it clicked on
			// if it's a valid node then get it
			// set the destination
			// and add this into it's list of
			// connected nodes
			
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, Mathf.Abs (Camera.main.transform.position.z) + 20)) {
				INodeBase destination = hit.collider.GetComponent<INodeBase> ();
				if (destination is EditorNodeBase) {
					FinishNewConnection(destination as EditorNodeBase);
				} else {
					Debug.Log("Clicked node was not an editor node.");
				}
			}
			
		}
		
	}

	public void FinishNewConnection(EditorNodeBase destination) {
		Vector3 originP = gameObject.transform.position;
		originP.z = lineZOffset;
		Vector3 destinationP = destination.gameObject.transform.position;
		destinationP.z = lineZOffset;
		tempLineRenderer.updateBezierPath(originP, destinationP);
		Connections.Add (destination.nodeId, tempLineRenderer);
		ChangeState (EditorNodeState.DraggingConnectionEnded);
	}

	public void ChangeState(EditorNodeState to) {
		Debug.Log ("Changing state");
		this.nextState = to;
		transitionToNextState = true;
	}

	protected Vector3 WorldMousePosition() {
		var mousePos = Input.mousePosition;
		mousePos.z = Mathf.Abs (Camera.main.transform.position.z);
		mousePos = Camera.main.ScreenToWorldPoint(mousePos);
		return mousePos;
	}

	public void SetupConnections() {

	}

	public static implicit operator JSON(EditorNodeBase editorNode) {
		GameObject g = editorNode.gameObject;
		JSON nodeJson = new JSON ();
		if (g != null) {
			JSON js = new JSON();
			JSON jsTransform = new JSON();

			js[NodeIdKey] = editorNode.nodeId;

			// Only use this text value if we don't have an input field
			string value = "";
			if (editorNode.valueInputField == null && editorNode.valueText != null) {
				value = editorNode.valueText.text;
			} 

			if (editorNode.valueInputField != null) {
				value = editorNode.valueInputField.text;
			}

			// This ensures we don't end up with anything weird
			// Or at least we hope so
			js[ValueKey] = value;

			js[NodeTypeKey] = editorNode.GetType().Name;

			// Get the child lists
			List<int> cIds = new List<int>();
			foreach(int key in editorNode.Connections.Keys) {
				cIds.Add (key);
			}
			js[NodeConnectionIdsKey] = cIds.ToArray();

			// Base object crap
			js["transform"] = jsTransform;
			jsTransform["position"] = (JSON)g.transform.position;
			jsTransform["localScale"] = (JSON)g.transform.localScale;
			jsTransform["rotation"] = (JSON)g.transform.rotation;

			nodeJson[NodeNodeKey] = js;

		}          
		return nodeJson;
	}

//	public EditorNodeBase ConvertFromJson(JSON json) {
//		return (EditorNodeBase)json;
//	}

	public virtual object ConvertFromJson(JSON json) {
		return (EditorNodeBase)json;
	}

	// JSON to class conversion
	public static explicit operator EditorNodeBase(JSON value) {
		checked {
			JSON jsTransform = value.ToJSON("transform");
			string type = (string)value[NodeTypeKey];

			GameObject obj = Instantiate(Resources.Load(type, typeof(GameObject))) as GameObject;
			EditorNodeBase node = obj.GetComponent(type) as EditorNodeBase;
			node.gameObject.SetActive(true);
			//node.init();
			
			node.nodeId = value.ToInt(NodeIdKey);
			
			if (node.nodeId > NextNodeId) {
				NextNodeId = node.nodeId + 1;// Increment the next node id counter
			}

			// TODO load this data
			if(node.valueInputField != null) {
				node.valueInputField.text = (string)value[ValueKey];
			} else {
				node.valueText.text = (string)value[ValueKey];
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

	// convert a JSON array to a NodePrefab Array
	public static EditorNodeBase[] Array(JSON[] array) {
		List<EditorNodeBase> tc = new List<EditorNodeBase>();
		for (int i=0; i<array.Length; i++) {
			tc.Add ((EditorNodeBase)array [i]);
		}
		return tc.ToArray();
	} 

	public JSON getJson() {
		return this;
	}
}
