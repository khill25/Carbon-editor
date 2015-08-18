using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WyrmTale;
using System.IO;
using System.Reflection;
using System.ComponentModel;

public interface ISaveable {
	JSON getJson();
}

public interface ILoadable<T> {
	//T ConvertFromJson(JSON json);
}

public class SaveUtility {


	public const string ObjectIdKey = "objectIdKey";
	public const string ObjectNameKey = "objectNameKey";
	public const string ObjectPositionKey = "objectPositionKey";
	public const string ObjectScaleKey = "objectScaleKey";

	public static JSON saveJson = new JSON ();

	/*
	 * TODO
	 * we can either save each node in it's own file (this may speed up load times)
	 * or save everything in one json file as objects in a json array (this may be slow)
	 * Ease of storage, vs loading speed
	 * 
	 * FOR NOW, just do the easier of the two.
	 */

	public static void Save<T>(T item) where T : MonoBehaviour, ISaveable {
	
		// Write json to file
		// or whatever

		string fileName = "test.json";

		JSON json = item.getJson ();
		string jsonString = json.serialized;

		if (File.Exists(fileName)) {
			Debug.Log(fileName+" already exists.");
			return;
		}
		var sr = File.CreateText(fileName);
		sr.Write (jsonString);
		sr.Close();
	}

	public static void SaveAll(List<ISaveable> items) {
		string fileName = "itemList.json";

		if (File.Exists (fileName)) {
			Debug.Log (fileName + " already exists.");
			return;
		}

		var sr = File.CreateText(fileName);

		sr.WriteLine ("{\"nodes\": [");

		int index = 0;
		foreach (ISaveable item in items) {
			JSON json = item.getJson ();
			string jsonString = json.serialized;
			sr.Write (jsonString);

			// If this would be the last item in the list don't add a comma
			if (index == items.Count-1) {
			} else {
				sr.Write(",");
			}

			sr.WriteLine();

			index++;
		}

		sr.WriteLine ("]}");

		sr.Close();
	}

	public static void LoadAll() {

		// key "classType" is in every saved object
		// and is used when restoring the game object
		// we can Load the resource with that type by passing it in via reflection
		JSON json = new JSON();
		using (StreamReader s = new StreamReader ("itemList.json")) {
			try {
				string rawFile = s.ReadToEnd ();
				json.serialized = rawFile;
			} catch (IOException e) {
			}
		}

		foreach(JSON child in json.ToArray<JSON>("nodes")) {
			JSON node = child["node"] as JSON;
			string assemblyName = node["classType"] as string;
			System.Type type = typeof(SaveUtility).Assembly.GetType (assemblyName);
			System.Collections.ArrayList l = SaveUtility.Load(node, assemblyName, type);
		}

		// Now that all the objects are loaded we can go ahead and set up the connections
		ObjectManager.instance.SetupAllNodes ();

		foreach (int id in ObjectManager.instance.allNodes.Keys) {
			GameObject o = ObjectManager.instance.allNodes[id];
			NodePrefab node = o.GetComponent<NodePrefab>();

			Debug.Log ("NodeId: " + node.nodeId + "\n" + "Children: " + node.childNodes.Count
			           + "/n" + "Parents: " + node.connectedFromNodes.Count);

		}

	}

	public static ArrayList Load(JSON json, string prefabScriptName, System.Type T) {

		// Load in the file
	
		//NodePrefab node = (NodePrefab)json;
		Object o = Resources.Load(prefabScriptName, T);
		INodeBase baseNode = o as INodeBase;
		baseNode.ConvertFromJson(json);

		return null;
	}

}
