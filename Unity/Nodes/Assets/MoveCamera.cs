using UnityEngine;
using System.Collections;

public class MoveCamera : MonoBehaviour, ISFEditingDelegate {

	public float speed = 40;
	public bool isEditing = false;

	public Vector3 topLeftBounds;
	public Vector3 topRightBounds;
	public Vector3 bottomLeftBounds;
	public Vector3 bottomRightBounds;
	public float maxZoomOut = 10;
	public float maxZoomIn = 1;

	bool isMouseDragging = false;
	bool hasLastMousePos = false;
	Vector3 lastMousePos;
	float lastTimeDown = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update() {
		if (!isEditing) {
			// Process camera move updates
			doInput();
		}
	}

	public void setIsEditingDelegateValue(bool value) {
		Debug.Log ("setEditing:" + value + " message received");
		isEditing = value;
	}

	public GameObject getGameObject() {
		return this.gameObject;
	}
	
	protected void doInput() {
		if(Input.GetKey(KeyCode.RightArrow)) {
			transform.Translate(new Vector3(speed * Time.deltaTime,0,0));
			
		} 
		
		if(Input.GetKey(KeyCode.LeftArrow)) {
			transform.Translate(new Vector3(-speed * Time.deltaTime,0,0));
			
		} 
		
		if(Input.GetKey(KeyCode.DownArrow)) {
			transform.Translate(new Vector3(0,-speed * Time.deltaTime,0));
			
		} 
		
		if(Input.GetKey(KeyCode.UpArrow)) {
			transform.Translate(new Vector3(0,speed * Time.deltaTime,0));
			
		}
		
		if (Input.GetKeyDown (KeyCode.LeftShift)) {
			float now = Time.realtimeSinceStartup;
			float diff = now - lastTimeDown;
			if (diff < .35) {
				Debug.Log("Shift double tapped.");
				lastTimeDown = 0;
			} else {
				lastTimeDown = now;
			}
			
		} 

		if(Input.GetKey(KeyCode.LeftShift)) {
			Vector3 mousePos = Input.mousePosition;//calculateMouse();
			var last = mousePos;
			mousePos.z = 0;

			// TODO
			// This code should be calculating a difference but it doesn't seem to be working correctly
			if (hasLastMousePos) {
				mousePos.x -= lastMousePos.x;
				mousePos.y -= lastMousePos.y;
				mousePos.x *= .05f;
				mousePos.y *= .05f;
				//Debug.Log (mousePos);
				transform.Translate(mousePos);
			}
			lastMousePos = last;
			
			hasLastMousePos = true;
		} else if (Input.GetKeyUp(KeyCode.LeftShift)) {
			hasLastMousePos = false;
			Debug.Log ("Resetting lastMouse");
		}
		
		if (Input.GetAxis("Mouse ScrollWheel") != 0) {
			Vector2 scrollDelta = Input.mouseScrollDelta;
			//Debug.Log (scrollDelta);
			if (transform.position.z + scrollDelta.y >= -15*maxZoomOut && transform.position.z + scrollDelta.y <= -15*maxZoomIn) { 
				transform.Translate(new Vector3(0f, 0f, scrollDelta.y));
			}
		}

//		if (Input.GetMouseButtonDown(0)) {
//			// Perform hit test, if there is no hit then we can go into a dragging mode
//			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
//			RaycastHit hit;
//			if (!Physics.Raycast (ray, out hit, Mathf.Abs (Camera.main.transform.position.z) + 20)) {
//				isMouseDragging = true;
//			}
//		} else if (Input.GetMouseButtonUp(0)) {
//			isMouseDragging = false;
//		}
//
//		if (isMouseDragging) {
//			// Do drag
//			var mousePos = calculateMouse();
//			
//			float x = mousePos.x - startingMousePos.x;
//			float y = mousePos.y - startingMousePos.y;
//			
//			// Calculate diff
//			x = rt.sizeDelta.x + x;
//			y = rt.sizeDelta.y - y;
//			
//			//Apply diff
//			rt.sizeDelta = new Vector2 (x, y);
//			
//			startingMousePos = mousePos;
//		}

	}

	protected Vector3 calculateMouse() {
		var mousePos = Input.mousePosition;
		mousePos.z = Mathf.Abs (Camera.main.transform.position.z);
		mousePos = Camera.main.ScreenToWorldPoint(mousePos);
		return mousePos;
	}

}
