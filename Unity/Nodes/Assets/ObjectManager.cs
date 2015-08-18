using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectManager : MonoBehaviour {

	public Dictionary<int, GameObject> allNodes;

	public static ObjectManager instance;

	protected void init() {
		instance = this;
	}

	void Awake() {
		init ();
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void RegisterNewNodeObject(NodePrefab node) {
		if (allNodes == null) {
			allNodes = new Dictionary<int, GameObject>();
			
			GameObject[] allNodeObjects = GameObject.FindGameObjectsWithTag("SFNode");
			foreach(GameObject obj in allNodeObjects) {
				NodePrefab p = obj.GetComponent<NodePrefab>();
				allNodes.Add(p.nodeId, obj);
			}
			
		}

		if (this.allNodes.ContainsKey(node.nodeId)) {
			this.allNodes[node.nodeId] = node.gameObject;
		} else {
			this.allNodes.Add(node.nodeId, node.gameObject);
		}
	}

	public GameObject getGameObjectNodeWithNodeId(int nodeId) {

		if (allNodes == null) {
			allNodes = new Dictionary<int, GameObject>();

			GameObject[] allNodeObjects = GameObject.FindGameObjectsWithTag("SFNode");
			foreach(GameObject obj in allNodeObjects) {
				NodePrefab p = obj.GetComponent<NodePrefab>();
				allNodes.Add(p.nodeId, obj);
			}

		}

		if (allNodes.ContainsKey (nodeId)) {
			return allNodes[nodeId];
		}

		return null;

	}

	public void SetupAllNodes() {

		if (allNodes == null) {
			allNodes = new Dictionary<int, GameObject>();
			
			GameObject[] allNodeObjects = GameObject.FindGameObjectsWithTag("SFNode");
			foreach(GameObject obj in allNodeObjects) {
				NodePrefab p = obj.GetComponent<NodePrefab>();
				allNodes.Add(p.nodeId, obj);
			}
			
		}

		foreach (int nodeId in allNodes.Keys) {
			INodeBase node = allNodes[nodeId].GetComponent<INodeBase>();
			node.SetupConnections();
		}
	}

	public void saveAll() {

		List<ISaveable> items = new List<ISaveable> ();

		// This isn't really fast but it will get all the crap we want
		MonoBehaviour[] allNodeTypes = GameObject.FindObjectsOfType<MonoBehaviour>();
		foreach (MonoBehaviour m in allNodeTypes) {
			if (m is ISaveable) {
				ISaveable b = m.gameObject.GetComponent<ISaveable>();
				items.Add(b);
			}
		}

		SaveUtility.SaveAll(items);
	}

	public void LoadAll() {
		SaveUtility.LoadAll ();
	}


}
