using UnityEngine;
using System.Collections;

public class MaintainTool : MonoBehaviour {
	public delegate void OnClickEventHandler(int toolIndex);
	public event OnClickEventHandler OnClick=delegate{};

	public int ToolIndex;

	void OnMouseDown(){
		OnClick(ToolIndex);
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
