using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class potMenuBtn0 : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnClick(){
		//转入场景
		SceneManager.LoadSceneAsync("PatternScene");
	}
}
