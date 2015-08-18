using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SFLineRenderer : MonoBehaviour {

	private enum Mode
	{
		Line, //Draws Line Segments at points
		Bezier, //Interprets points as control points of Bezier curve
		BezierInterpolated, //Interpolates 
		BezierReduced
	}
	
	private Mode mode;
	private NodePrefab originNode;
	private NodePrefab destinationNode;
	private List<Vector3> points;
	private List<Vector3> gizmos;	
	public LineRenderer lineRenderer;
	public LineHandle lineHandle;

	public Color startColor;
	public Color endColor;

	private void init() {
		lineRenderer = GetComponent<LineRenderer>();
		points = new List<Vector3>();
		
		mode = Mode.Line;
	}

	void Awake() {
		Debug.Log ("SFLineRenderer awake");
		init ();
	}

	// Use this for initialization
	void Start () 
	{
		Debug.Log ("SFLineRenderer Start");

	}
	
	// Update is called once per frame
	void Update () 
	{
		//ProcessInput();
		Render();
		lineRenderer.SetColors(new Color(.7f, .3f, .15f, 1.0f), new Color(.15f, .3f, .7f, 1.0f));
	}

	public void addBezierPath(NodePrefab origin, NodePrefab destination) {
		points.Add (origin.transform.position);
		points.Add (destination.transform.position);
		// Add control points to get a nice bezier curve???
		//BezierInterpolate ();
		Render ();
	}

	public void updateBezierPath(NodePrefab origin, NodePrefab destination) {
		if (points.Count > 1) {
			points [0] = origin.transform.position;
			points [points.Count - 1] = destination.transform.position;
			Render ();
		} else {
			Debug.LogError("There are currently no points in the array");
		}
	}

	public void addBeginingBezierPathPoints(Vector3 origin, Vector3 destination) {
		this.mode = Mode.Line;
		points.Add (origin);
		points.Add (destination);
		Render ();
	}

	// Used to
	public void updateBezierPath(Vector3 origin, Vector3 destination) {
		if (points.Count > 1) {
			points [0] = origin;
			points [points.Count - 1] = destination;
			Render ();
		} else {
			Debug.LogError("There are currently no points in the array");
		}
	}

	private void ProcessInput() {
		if (mode == Mode.BezierReduced)
		{
			if (Input.GetMouseButtonDown(0))
			{
				//points.Clear();
			}
			if (Input.GetMouseButton(0))
			{
				Vector2 screenPosition = Input.mousePosition;
				Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, Mathf.Abs (Camera.main.transform.position.z)));
				
				points.Add(worldPosition);
			}
		}
		else
		{
			if (Input.GetMouseButtonDown(0))
			{
				Vector2 screenPosition = Input.mousePosition;
				Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, Mathf.Abs (Camera.main.transform.position.z)));
				
				points.Add(worldPosition);
			}
		}
		
		if(Input.GetKeyDown(KeyCode.F1))
		{
			mode = Mode.Line;
		}
		
		if(Input.GetKeyDown(KeyCode.F2))
		{
			mode = Mode.Bezier;
		}
		
		if(Input.GetKeyDown(KeyCode.F3))
		{
			mode = Mode.BezierInterpolated;
		}
		
		if(Input.GetKeyDown(KeyCode.F4))
		{
			mode = Mode.BezierReduced;
		}
		
		if (Input.GetKeyDown(KeyCode.X))
		{
			points.Clear();
		}
	}
	
	
	///Note: this file merely illustrate the algorithms.
	///Generally, they should NOT be called each frame!
	private void Render()
	{

		switch(mode)
		{
		case Mode.Line:
			RenderLineSegments();
			break;
		case Mode.Bezier:
			RenderBezier();
			break;
		case Mode.BezierInterpolated:
			BezierInterpolate();
			break;
		case Mode.BezierReduced:
			BezierReduce();
			break;
		}

		if (points.Count > 1) {

			Vector3 p = lineRenderer.transform.position;
			p.z = 1.5f;
			lineRenderer.transform.position = p;

			if (lineHandle != null) {
				Vector3 mid = (points [0] + points [1]) * .5f;
				mid.z = 1;
				lineHandle.transform.position = mid;//lineHandle.transform.TransformPoint((points[0] + points[1]) * .5f);
			}
		}

	}
	
	private void RenderLineSegments()
	{
		gizmos = points;
		SetLinePoints(points);
	}
	
	private void RenderBezier()
	{
		BezierPath bezierPath = new BezierPath();
		
		bezierPath.SetControlPoints(points);
		List<Vector3> drawingPoints = bezierPath.GetDrawingPoints2();
		
		gizmos = drawingPoints;
		
		SetLinePoints(drawingPoints);
	}
	
	private void BezierInterpolate()
	{
		BezierPath bezierPath = new BezierPath();
		bezierPath.Interpolate(points, .25f);
		
		List<Vector3> drawingPoints = bezierPath.GetDrawingPoints2();
		
		gizmos = bezierPath.GetControlPoints();  
		
		SetLinePoints(drawingPoints);
	}
	
	private void BezierReduce()
	{
		BezierPath bezierPath = new BezierPath();
		bezierPath.SamplePoints(points, 10, 1000, 0.33f);
		
		List<Vector3> drawingPoints = bezierPath.GetDrawingPoints2();
		Debug.Log(gizmos.Count);
		
		gizmos = bezierPath.GetControlPoints();
		SetLinePoints(drawingPoints);
	}
	
	private void SetLinePoints(List<Vector3> drawingPoints)
	{
		//lineRenderer.SetLinePoints (drawingPoints);
		lineRenderer.SetVertexCount(drawingPoints.Count);


		for (int i = 0; i < drawingPoints.Count; i++)
		{
			lineRenderer.SetPosition(i, drawingPoints[i]);
		}

	}
	
	public void OnDrawGizmos()
	{
		if (gizmos == null)
		{
			return;
		}        
		
		for (int i = 0; i < gizmos.Count; i++)
		{
			Gizmos.DrawWireSphere(gizmos[i], 1f);            
		}
	}

}
