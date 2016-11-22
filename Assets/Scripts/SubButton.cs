using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SubButton : MonoBehaviour {
	public Vector3 destPosAtL,destPosAtR;//按钮的目标位置 当按钮停靠在屏幕两边时
	public string SceneName;//要加载的场景名字

	public bool _mouseDown=false;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnMouseDown(){
		_mouseDown=true;
		StaticGlobal.ClickSubBtn=true;
		StaticGlobal.DraggingPot=true;
	}
	void OnMouseUp(){
		StaticGlobal.ClickSubBtn=false;
		if(_mouseDown){
			_mouseDown=false;
			AudioSource.PlayClipAtPoint(transform.parent.
				GetComponent<PotButton>().AudiClick,Camera.main.transform.position);
			StaticGlobal.Spin=true;
			SceneManager.LoadSceneAsync(SceneName);
		}
	}

	/*IEnumerator LoadSceneAsync(string name){
		yield return new WaitForSeconds(0.2f);
		var asyncTask=SceneManager.LoadSceneAsync(name);
		asyncTask.allowSceneActivation=false;
		print(asyncTask.isDone);
		while(!asyncTask.isDone){
			print("d");
			yield return null;
		}
		asyncTask.allowSceneActivation=true;
		yield return null;
	}*/
}
