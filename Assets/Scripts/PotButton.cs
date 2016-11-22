using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PotButton : MonoBehaviour {
	
	bool _subBtnShown=false;
	float _downTime=-2;
	Vector3 _downPos;
	Transform[] _subButtons;
	Vector3[] _destPosAtLs,_destPosAtRs;


	public GameObject AnimObj;
	public AudioClip AudiShow,AudiHide,AudiClick,AudiDocked;
	// Use this for initialization
	void Start () {
		_subButtons=new Transform[5];
		_destPosAtLs=new Vector3[5];
		_destPosAtRs=new Vector3[5];
		for(int i=0;i<5;i++){
			_subButtons[i]=transform.GetChild(4-i);
			float subBtnRadius=1.1f;
			_destPosAtLs[i]=new Vector3(Mathf.Cos(Mathf.PI*i/4-Mathf.PI/2)*subBtnRadius,Mathf.Sin(Mathf.PI*i/4-Mathf.PI/2)*subBtnRadius,0);
				//transform.GetComponent<SubButton>().destPosAtL;
			_destPosAtRs[i]=new Vector3(-Mathf.Cos(Mathf.PI*i/4-Mathf.PI/2)*subBtnRadius,Mathf.Sin(Mathf.PI*i/4-Mathf.PI/2)*subBtnRadius,0);
				//transform.GetComponent<SubButton>().destPosAtR;
		}
	
	}
	
	// Update is called once per frame
	void Update () {
		if(StaticGlobal.Spin){
			transform.Rotate(0,-7,0);
		}
		Dragging();

	}

	void  OnMouseDown(){
		//双击回到首页
		if(Time.time-_downTime<0.3){
			StaticGlobal.DraggingPot=false;
			if(Controller.Instance.currentState!=Controller.State.ENTERPAGE){
				StaticGlobal.Spin=true;
				SceneManager.LoadSceneAsync("EnterScene");
			}
		}
		StaticGlobal.DraggingPot=true;
		_downTime=Time.time;
		_downPos=Input.mousePosition;
	}
	void DeactiveGO(GameObject  go){
		go.SetActive(false);
	}
	public void SwitchSubButtons(){
		//已显示子按钮 隐藏之
		if(_subBtnShown){
			_subBtnShown=false;
			for(int i=0;i<5;i++){
				GameObject target=_subButtons[i].gameObject;
				target.GetComponent<SubButton>()._mouseDown=false;
				(Instantiate(AnimObj)as GameObject)
					.GetComponent<AnimElement>()
					.InitLocal(target.transform.localPosition,
						new Vector3(0,0,0),
						_subButtons[i],0.1f,20)
					.OnFinish(()=>target.SetActive(false))
					.AddScaleAnimation(new Vector3(1,1,1),new Vector3())
					.Play();
			}
			AudioSource.PlayClipAtPoint(AudiHide,Camera.main.transform.position,0.3f);
		}
		//未显示子按钮 显示之
		else{
			_subBtnShown=true;
			AudioSource.PlayClipAtPoint(AudiShow,Camera.main.transform.position,0.3f);
			for(int i=0;i<5;i++){
				Vector3 dest=transform.position.x>0?_destPosAtRs[i]:_destPosAtLs[i];
				GameObject target=_subButtons[i].gameObject;
				target.SetActive(true);
				target.GetComponent<SubButton>()._mouseDown=false;
				(Instantiate(AnimObj)as GameObject)
					.GetComponent<AnimElement>()
					.InitLocal(target.transform.localPosition,
						dest,
						_subButtons[i],0.1f,30)
					.AddScaleAnimation(new Vector3(),new Vector3(1,1,1))
					.Play();
			}
		}
	}

	public void OnMouseUp(){
		if(Time.time-_downTime<0.3f
			&&(_downPos-Input.mousePosition).sqrMagnitude<200){
			//打开导航面板
			//暂时加载甄选栏目
			SwitchSubButtons();
		}
		//自动停靠
		float dockedX=transform.position.x>0?Screen.width*9/10:Screen.width/10;
		float dockedY=Input.mousePosition.y<Screen.height/8?Screen.height/8:Input.mousePosition.y;
		dockedY=Input.mousePosition.y>Screen.height*7/8?Screen.height*7/8:dockedY;
		StaticGlobal.PotButtonScreenPosition=new Vector3(dockedX, dockedY, -Camera.main.transform.position.z);
		(Instantiate(AnimObj)as GameObject)
			.GetComponent<AnimElement>()
			.Init(StaticGlobal.PotButton.transform.position,
				Camera.main.ScreenToWorldPoint(StaticGlobal.PotButtonScreenPosition),
			StaticGlobal.PotButton.transform,0.5f,30)
			.Play();
	}

	void Dragging(){
		if(StaticGlobal.DraggingPot
			&&!StaticGlobal.ClickSubBtn
			&&(_downPos-Input.mousePosition).sqrMagnitude>1000
		){
			if(_subBtnShown){
				SwitchSubButtons();
			}
			transform.position= Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
		}
	}
}
