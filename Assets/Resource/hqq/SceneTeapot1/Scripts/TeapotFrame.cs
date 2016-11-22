using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TeapotFrame : MonoBehaviour {

	public GameObject PrefabPotButton;
	// Use this for initialization
	void Start () {
		StaticGlobal.PotButton= Instantiate(PrefabPotButton)as GameObject;
		StaticGlobal.PotButton.transform.parent = GameObject.Find ("PanelPotButton").transform;
		StaticGlobal.PotButton.transform.localScale = new Vector3 (1, 1, 1);
	}

	// Update is called once per frame
	void Update () {
	}


}