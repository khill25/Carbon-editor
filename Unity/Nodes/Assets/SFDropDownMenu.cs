using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SFDropDownMenu : MonoBehaviour {

	public List<string> menuItems = new List<string>();
	public string selectedMenuItem;

	protected int selectedMenuItemIndex;
	protected bool menuIsOpen = false;

	public GameObject optionPanel;
	public GameObject listPanel;
	public GameObject menuItemPrefab;
	public Text currentlySelectedMenuItemLabel;

	void init() {

		foreach(string item in menuItems) {
			AddMenuItem(item);
		}

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

	public void AddMenuItem(string menuItemText) {
		GameObject menuItem = Instantiate<GameObject>(menuItemPrefab);
		menuItem.SetActive(true);
		Text t = menuItem.GetComponent<Text>();
		t.text = menuItemText;
		menuItem.transform.SetParent(listPanel.transform, false);
	}

	public void RemoveMenuItem(string menuItemText) {

	}

	public void ToggleSelectionMenu() {
		if (this.menuIsOpen) {
			CloseSelectionMenu();
		} else {
			OpenSelectionMenu();
		}
	}

	public void OpenSelectionMenu() {
		this.menuIsOpen = true;
		Animation openAnimation = this.optionPanel.GetComponent<Animation>();
		openAnimation.playAutomatically = false;
		this.optionPanel.gameObject.SetActive(true);
		openAnimation.Play("OpenDropDownAnimation");
	}

	public void CloseSelectionMenu() {
		this.menuIsOpen = false;
		Animation closeAnimation = this.optionPanel.GetComponent<Animation>();
		closeAnimation.Play("CloseDropDownMenu");

		//yield return WaitForAnimation(closeAnimation);

		//this.optionPanel.gameObject.SetActive(false);
	}

	
	public void SelectMenuItem(GameObject o) {
		// Figure out the index I guess???
		Debug.Log ("Menu item selected");
		Text text = o.GetComponent<Text>();
		this.selectedMenuItem = text.text;
		this.currentlySelectedMenuItemLabel.text = this.selectedMenuItem;

		CloseSelectionMenu();

	}


/* 
 * Helper functions
 */


}
