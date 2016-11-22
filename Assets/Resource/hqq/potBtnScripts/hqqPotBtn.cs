using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class hqqPotBtn : MonoBehaviour {
	Camera cam;
	List<GameObject> btns = new List<GameObject>();
	List<Vector3> btnsPosLeft = new List<Vector3>();
	List<Vector3> btnsPosRight = new List<Vector3>();
	Vector3 zero;

	void Start () {
		StaticGlobal.Spin=false;
		cam = GameObject.Find ("Camera").GetComponent<Camera> ();
		zero = new Vector3 (0, 0, 0.0f);

		Vector3 pos = cam.ScreenToWorldPoint (StaticGlobal.PotButtonScreenPosition);
		pos.z = 0;
		transform.position = pos;
		InitMenuBtn ();
	}

	void Update () {
		if(StaticGlobal.Spin){
			transform.Rotate(0,-7,0);
		}
	}

	public void OnClick(){
		//如果按钮没有被激活
		if (!btns[0].activeSelf) 
			playAnim ();
		else
			playAnimBack ();

	}

	void OnDoubleClick(){
		SceneManager.LoadSceneAsync("EnterScene");

	}

	void OnDrag(){
		Vector3 mpos = Input.mousePosition;
		transform.position = cam.ScreenToWorldPoint(mpos);

	}
	void OnDragEnd(){
		float dockedX;
		float dockedY;
		if (Input.mousePosition.y < Screen.height / 8)
			dockedY = Screen.height / 8;
		else if (Input.mousePosition.y > Screen.height * 7 / 8)
			dockedY = Screen.height * 7 / 8;
		else
			dockedY = Input.mousePosition.y;

		if (Input.mousePosition.x >= Screen.width / 2) 
			dockedX = Screen.width * 9 / 10;
		else 
			dockedX = Screen.width * 1 / 10;

		StaticGlobal.PotButtonScreenPosition = Input.mousePosition;
		Vector3 endPos = cam.ScreenToWorldPoint (new Vector3 (dockedX, dockedY, 0));
		(Instantiate (Resources.Load ("AnimElementObj"))as GameObject).GetComponent<AnimElement> ()
			.Init (transform.position, endPos, transform, 0.5f, 30).Play ();

		StaticGlobal.PotButtonScreenPosition = cam.WorldToScreenPoint (endPos);
		StaticGlobal.PotButtonScreenPosition.z = 73.2f;

	}

	void InitMenuBtn(){
		btns.Add(Instantiate (Resources.Load ("potMenuBtn/potMenuBtn0"))as GameObject);
		btns.Add(Instantiate (Resources.Load ("potMenuBtn/potMenuBtn1"))as GameObject);
		btns.Add(Instantiate (Resources.Load ("potMenuBtn/potMenuBtn2"))as GameObject);
		btns.Add(Instantiate (Resources.Load ("potMenuBtn/potMenuBtn3"))as GameObject);
		btns.Add(Instantiate (Resources.Load ("potMenuBtn/potMenuBtn4"))as GameObject);
		for (int i = 0; i < btns.Count; i++) {
			btns [i].transform.parent = transform;
			btns [i].transform.localPosition = zero;
			btns [i].transform.localScale = new Vector3 (0.0f, 0.0f, 0.0f);
			btns [i].SetActive (false);
		}


		btnsPosRight.Add( new Vector3 (0, 113, 0));
		btnsPosRight.Add( new Vector3 (-78, 78, 0));
		btnsPosRight.Add( new Vector3 (-111, 0, 0));
		btnsPosRight.Add( new Vector3 (-80, -80, 0));
		btnsPosRight.Add( new Vector3 (0, -113, 0));

		btnsPosLeft.Add( new Vector3 (-16, 113, 0));
		btnsPosLeft.Add( new Vector3 (62, 78, 0));
		btnsPosLeft.Add( new Vector3 (95, 0, 0));
		btnsPosLeft.Add( new Vector3 (63, -80, 0));
		btnsPosLeft.Add( new Vector3 (-15, -113, 0));

	}

	void playAnimBack(){

		for (int i = 0; i < btns.Count; i++) {
			if(Input.mousePosition.x>=Screen.width/2)
				(Instantiate (Resources.Load ("AnimElementObj")as GameObject)).GetComponent<AnimElement> ()
					.InitLocal (btnsPosRight [i], zero, btns [i].transform, 0.1f, 20)
					.AddScaleAnimation (new Vector3 (1,1,1), new Vector3 (0, 0, 0)).Play ();
			else 
				(Instantiate (Resources.Load ("AnimElementObj")as GameObject)).GetComponent<AnimElement> ()
					.InitLocal (btnsPosLeft [i], zero, btns [i].transform, 0.1f, 20)
					.AddScaleAnimation (new Vector3 (1,1,1), new Vector3 (0, 0, 0)).Play ();

			if (btns [i].transform.position.x == transform.position.x)
				btns [i].SetActive (false);
		}
	}

	void playAnim(){
		for (int i = 0; i < btns.Count; i++) {
			if(Input.mousePosition.x>=Screen.width/2)
				(Instantiate (Resources.Load ("AnimElementObj")as GameObject)).GetComponent<AnimElement> ()
					.InitLocal (zero, btnsPosRight [i], btns [i].transform, 0.1f, 20)
					.AddScaleAnimation (new Vector3 (), new Vector3 (1, 1, 1)).Play ();
			else 
				(Instantiate (Resources.Load ("AnimElementObj")as GameObject)).GetComponent<AnimElement> ()
					.InitLocal (zero, btnsPosLeft [i], btns [i].transform, 0.1f, 20)
					.AddScaleAnimation (new Vector3 (), new Vector3 (1, 1, 1)).Play ();
			btns [i].SetActive (true);
		}
	}
}