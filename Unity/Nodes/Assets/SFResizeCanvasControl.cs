using UnityEngine;
using System.Collections;

public class SFResizeCanvasControl : MonoBehaviour {

	public Canvas parentCanvas;

	public float timeDown;

	protected bool isDragging;
	protected bool isResizing;
	protected RectTransform startingRectTransform;
	protected Vector3 startingMousePos;
	protected Vector3 draggingOffset;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (this.isResizing) {
			doDragResize();
		}

		if (isDragging) {
			doDragging ();
		}

	}

	protected Vector3 calculateMouse() {
		var mousePos = Input.mousePosition;
		//mousePos.z = Mathf.Abs (Camera.main.transform.position.z);
		//mousePos = Camera.main.ScreenToWorldPoint(mousePos);
		return mousePos;
	}

	protected Vector3 calculateMouseWorld() {
		var mousePos = Input.mousePosition;
		mousePos.z = Mathf.Abs (Camera.main.transform.position.z);
		mousePos = Camera.main.ScreenToWorldPoint(mousePos);
		return mousePos;
	}

	protected void doDragResize() {
		timeDown += Time.deltaTime;
		
		// Calculate the new transform based on the delta from the original location
		
		RectTransform rt = parentCanvas.GetComponent (typeof (RectTransform)) as RectTransform;
		
		var mousePos = calculateMouseWorld();

		float x = mousePos.x - startingMousePos.x;
		float y = mousePos.y - startingMousePos.y;

		// TODO should use 1 / scale factor of game object to get the correct speed factor
		// but the scale is set to .03 right now
		x *= 33.333f;
		y *= 33.333f;
		Debug.Log("dX: " + x + " dY: " + y);

		// Calculate diff
		x = rt.sizeDelta.x + x;
		y = rt.sizeDelta.y - y;

		//Apply diff
		rt.sizeDelta = new Vector2 (x, y);
		
		startingMousePos = mousePos;
	}

	public void resizingDragDidStart() {
		Debug.Log ("Drag Resize Started");
		this.startingRectTransform = parentCanvas.GetComponent (typeof (RectTransform)) as RectTransform;
		this.startingMousePos = calculateMouseWorld();
		isResizing = true;

	}

	public void resizingDragDidEnd() {
		isResizing = false;
		Debug.Log("Drag Resize Ended after: " + timeDown);
		timeDown = 0;
	}

	public void doDragging() {
		Vector3 mousePos = calculateMouseWorld();
		gameObject.transform.position = mousePos + draggingOffset;
	}

	public void draggingDidStart() {
		isDragging = true;
		Debug.Log("Dragging did start");

		Vector3 originalMousePos = Input.mousePosition;
		originalMousePos.z = Mathf.Abs (Camera.main.transform.position.z);
		draggingOffset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(originalMousePos);
	}

	public void draggingDidEnd() {
		isDragging = false;
		Debug.Log("Dragging did end");
	}

}
