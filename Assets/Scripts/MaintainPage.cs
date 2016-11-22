using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MaintainPage : MonoBehaviour {

	List<GameObject> _indicators_Small;
	List<GameObject> _indicators_Big;
	List<GameObject> _potDocks;
	List<GameObject> _intros;
	List<Vector3> 		  _indicatorGridPos;
	int[] _fixedDockerAndIntroXPos={-28,-21,-14,-7,0,7,14,21,28};
	float[] _fixedIndicatorXPos={-9.8f,-7.35f,-4.9f,-2.45f,0,2.45f,4.9f,7.35f,9.8f};
	Vector3 indicatorBackMostLPos=new Vector3(-7,5.8f,60);
	Vector3 indicatorFrontMostPos=new Vector3(0,2.5f,20);
	Vector3 _lastFrameMousePos;
	SwipeManager _swipeManager;
	bool _flagMouseDown=false;
	int _frontIndex=0;
	float introY=-1;
	float dockerY=-3.75f;
	int _indicatorStartY;
	Sprite _storedFrontIndicatorSmall;
	Transform _title;

	public Material material;
	public int AnimIndicatorDuration=20;//动画持续的帧数
	public int AnimDockDuration=35;//动画持续的帧数
	public int AnimIntroDuration=50;//动画持续的帧数
	public int distBetweenIntro;
	public GameObject PrefabAnimEleObj;
	public GameObject PrefabPotButton;
	// Use this for initialization
	void Start () {
		_title=GameObject.Find("title").transform;
		StaticGlobal.PotButton= Instantiate(PrefabPotButton)as GameObject;
		StaticGlobal.PotButton.transform.position= Camera.main.ScreenToWorldPoint(StaticGlobal.PotButtonScreenPosition);
	
		Controller.Instance.currentState=Controller.State.MAINTAIN;

		_swipeManager=new SwipeManager(0.8f);
		_indicatorStartY=(int)Camera.main.WorldToScreenPoint(new Vector3(0,3.37f,0)).y;
		//预加载资源
		PreManageResource();
		StaticGlobal.Spin=false;

	}

	// Update is called once per frame
	void Update () {
		_intros[_frontIndex].transform.localPosition=Tool.Interpolate(_intros[_frontIndex].transform.position,
			new Vector3(-Input.acceleration.x*0.2f,-Input.acceleration.y*0.2f+introY,0)
			,0.1f);
		////////////////////////手势的准备工作------------------------
	
		if(Input.GetMouseButtonUp(0)){
			if(!StaticGlobal.DraggingPot){
				_flagMouseDown=false;
				//根据抬起位置和时间进行手势判断,之后就能用得到的手势结果了
				_swipeManager.mouseUp(Input.mousePosition);
				UpdateElements(_swipeManager.SwipeLR,_swipeManager.ProgressLR);
			}
			StaticGlobal.DraggingPot=false;
		}
		if(!StaticGlobal.DraggingPot){
			if(Input.GetMouseButtonDown(0)){
				_swipeManager.mouseDown(Input.mousePosition);
				_flagMouseDown=true;

			}
		}
		///---------------------------/////////////////////////////

		//手指未离开屏幕
		if(_flagMouseDown){
			_swipeManager.CurrentPos=Input.mousePosition;
			/////////开始根据 progressLR 实时更新壶的位置 和诗句 的透明度
			//MovePotsAndPoem(_swipeManager.RawSwipeLR,_swipeManager.ProgressLR);
			MoveElements(_swipeManager.RawSwipeLR,_swipeManager.ProgressLR);
			//使用完当前的mousepos再更新lastFrameMousePos
			_lastFrameMousePos=Input.mousePosition;

		}

	}

	void OnPostRender(){
		material.SetPass(0);//不知道这句干嘛的  但是这句是必需的
		GL.LoadOrtho();//2D绘图
		for(int i=0;i<_indicators_Small.Count;i++){
			int height=(int)(_indicatorStartY-Camera.main.WorldToScreenPoint(_indicators_Small[i].transform.position).y);
			height-=5;
			int glXpos=(int)Camera.main.WorldToScreenPoint(_indicators_Small[i].transform.position).x;
			GLHelper.DrawRect(glXpos-2,_indicatorStartY-height,4,height,new Color(0.504f,0.504f,0.504f,0.9f-Mathf.Abs(_indicators_Small[i].transform.position.x/16)));
		}
		GL.End();

	}


	/// <summary>
	/// 预加载 五个 小标题 和五个 大标题
	///  四组壶+一组空壶 (一共五组)
	/// 五个 intro

	/// </summary>
	void PreManageResource(){
		Object obj;
		GameObject instance;
		Texture2D texture2d;
		SpriteRenderer spr;
		Sprite sp;	
		_intros=new List<GameObject>();
		_potDocks=new List<GameObject>();
		_indicators_Big =new List<GameObject>();
		_indicators_Small = new List<GameObject>();
		_indicatorGridPos=new List<Vector3>();
		for(int i=0;i<5;i++){
			// intro x 5
			obj=Resources.Load("MaintainPage/prefab/intro");
			instance=Instantiate(obj)as GameObject;
			instance.transform.localPosition=new Vector3(i*distBetweenIntro,instance.transform.localPosition.y);
			spr=instance.GetComponent<SpriteRenderer>();
			texture2d=Resources.Load("MaintainPage/intro"+(i+1).ToString()) as Texture2D;
			sp=Sprite.Create(texture2d,new Rect(0,0,texture2d.width,texture2d.height),new Vector2(0.5f,1));
			sp.name="intro"+(i+1);
			spr.sprite=sp;
			_intros.Add(instance);
			//docker x 5
			obj=Resources.Load("MaintainPage/prefab/potDocker"+(i+1));
			instance=Instantiate(obj) as GameObject;
			instance.transform.localPosition=new Vector3(i*distBetweenIntro,instance.transform.localPosition.y);
			_potDocks.Add(instance);
			// bigIndicator x 5
			obj=Resources.Load("MaintainPage/prefab/indicator");
			instance=Instantiate(obj)as GameObject;
			instance.transform.localPosition=GenIndicatorPosFromX(_fixedIndicatorXPos[i+4-_frontIndex]);
			spr=instance.GetComponent<SpriteRenderer>();
			texture2d=Resources.Load("MaintainPage/big"+(i+1).ToString()) as Texture2D;
			sp=Sprite.Create(texture2d,new Rect(0,0,texture2d.width,texture2d.height),new Vector2(0.5f,1));
			sp.name="indicator"+(i+1);
			spr.sprite=sp;
			instance.SetActive(false);
			_indicators_Big.Add(instance);
			// smallIndicator x 5
			obj=Resources.Load("MaintainPage/prefab/indicator");
			instance=Instantiate(obj)as GameObject;
			instance.transform.localPosition=GenIndicatorPosFromX(_fixedIndicatorXPos[i+4-_frontIndex]);
			spr=instance.GetComponent<SpriteRenderer>();
			texture2d=Resources.Load("MaintainPage/small"+(i+1).ToString()) as Texture2D;
			sp=Sprite.Create(texture2d,new Rect(0,0,texture2d.width,texture2d.height),new Vector2(0.5f,1));
			sp.name="indicator"+(i+1);
			spr.sprite=sp;
			//更新透明度
			spr.color=new Color(spr.color.r,spr.color.g,spr.color.b,1-Mathf.Abs(instance.transform.position.x/16));
			_indicators_Small.Add(instance);

			_indicatorGridPos.Add(instance.transform.position);
		}
		//把 front的indicator 换成粗体
		_storedFrontIndicatorSmall=_indicators_Small[_frontIndex].GetComponent<SpriteRenderer>().sprite;
		_indicators_Small[_frontIndex].GetComponent<SpriteRenderer>().sprite=
			_indicators_Big[_frontIndex].GetComponent<SpriteRenderer>().sprite;
		//把special1 自上向下移动到屏幕中显示
		Transform tf=_intros[0].transform.GetChild(0).transform;
		GameObject animObjSpecial1=Instantiate(PrefabAnimEleObj);
		animObjSpecial1.GetComponent<AnimElement>().Init(tf.position,
			new Vector3(_intros[0].transform.position.x,
				_intros[0].transform.position.y+0.7f,
				0)
			,tf,0,75).Play();

		//资源用来建立GameObject后释放所有资源
		obj=null;
		Resources.UnloadUnusedAssets();
	}

	/// <summary>
	/// 根据手指移动的Progress实时更新
	/// 移动标题  docker 和 intro
	/// 更改标题透明度
	/// </summary>
	void MoveElements(int direction,float progressLR){
		//i+4-frontIndex+_swipeManager.ProgressLR  本式用于求出dest的position
		if((_frontIndex-direction)>-1
			&&(_frontIndex-direction)<5){
			for(int i=0;i<_intros.Count;i++){
				//indicator透明度
				SpriteRenderer spr=_indicators_Small[i].GetComponent<SpriteRenderer>();
				spr.color=new Color(spr.color.r,spr.color.g,spr.color.b,1-Mathf.Abs(_indicators_Small[i].transform.position.x/16));

				//indicator位置
				_indicators_Small[i].transform.position=Vector3.Lerp(GenIndicatorPosFromX(_fixedIndicatorXPos[i+4-_frontIndex]),
					GenIndicatorPosFromX(_fixedIndicatorXPos[i+4-_frontIndex+direction]),
					progressLR*60/AnimIndicatorDuration);
				//intro位置
				_intros[i].transform.position=Vector3.Lerp(new Vector3(_fixedDockerAndIntroXPos[i+4-_frontIndex],introY,0),
					new Vector3(_fixedDockerAndIntroXPos[i+4-_frontIndex+direction],introY,0),progressLR*60/AnimIntroDuration);
				//dock位置
				_potDocks[i].transform.position=Vector3.Lerp(new Vector3(_fixedDockerAndIntroXPos[i+4-_frontIndex],dockerY,0),
					new Vector3(_fixedDockerAndIntroXPos[i+4-_frontIndex+direction],dockerY,0),progressLR*60/AnimDockDuration);
			}
		}
	}

	/*void MoveElements(int direction,float progressLR){
			for(int i=0;i<_intros.Count;i++){
				_indicators_Small[i].transform.position=_indicatorGridPos[i]+new Vector3(direction*2*progressLR,0,0);
			}
	}
	void UpdateElements(int direction,float progressLR){
		int variance=(int)(direction*2*progressLR);
		variance=_frontIndex-variance<0?0:variance;
		variance=_frontIndex-variance>4?4:variance;
		//先根据方向移动所有元素
			for(int i=0;i<_indicators_Small.Count;i++){
				//更改indicator位置
				Transform trans=_indicators_Small[i].transform;
				GameObject animElementObj=Instantiate(PrefabAnimEleObj);
				Vector3 dest=GenIndicatorPosFromX(_fixedIndicatorXPos[i+4-_frontIndex+variance]);
				animElementObj.GetComponent<AnimElement>().Init(
					GenIndicatorPosFromX(_fixedIndicatorXPos[i+4-_frontIndex])
					,dest,	trans,progressLR,AnimIndicatorDuration).Play();
			_indicatorGridPos[i]=dest;
			}
		//更新front的index
		_frontIndex=((_frontIndex-variance)<0||((_frontIndex-variance)>4))?_frontIndex:_frontIndex-variance;
	}*/
	/// <summary>
	/// 1--补全 移动动画 
	/// 2--动画完毕后将Front的title换成粗体--------暂不做,感觉没必要
	/// 
	/// </summary>
	void UpdateElements(int direction,float progressLR){
		if((_frontIndex-direction)>-1
			&&(_frontIndex-direction)<5){
			//先根据方向移动所有元素
			for(int i=0;i<_indicators_Small.Count;i++){
				//更改indicator透明度
				Transform transF=_indicators_Small[i].transform;
				GameObject animObjColor=Instantiate(PrefabAnimEleObj);
				Color startColor=new Color(1,1,1,1-Mathf.Abs(_fixedIndicatorXPos[i+4-_frontIndex]/16));
				Color destColor=new Color(1,1,1,1-Mathf.Abs(_fixedIndicatorXPos[i+4-_frontIndex+direction]/16));
				animObjColor.GetComponent<AnimElement>().Init(startColor,destColor
					,transF,progressLR,AnimIndicatorDuration).Play();
				//更改indicator位置
				Transform trans=_indicators_Small[i].transform;
				GameObject animElementObj=Instantiate(PrefabAnimEleObj);
				Vector3 dest=GenIndicatorPosFromX(_fixedIndicatorXPos[i+4-_frontIndex+direction]);
				animElementObj.GetComponent<AnimElement>().Init(
					//GenIndicatorPosFromX(_fixedIndicatorXPos[i+4-_frontIndex])
					trans.position
					,dest,	trans,progressLR,AnimIndicatorDuration).Play();
				//更改dock位置
				Transform transDock=_potDocks[i].transform;
				GameObject animObjDock=Instantiate(PrefabAnimEleObj);
				Vector3 dockDest=new Vector3(_fixedDockerAndIntroXPos[i+4-_frontIndex+direction],dockerY,0);
				animObjDock.GetComponent<AnimElement>().Init(
					//new Vector3(_fixedDockerAndIntroXPos[i+4-_frontIndex],dockerY,0)
					transDock.position
					,dockDest,	transDock,progressLR,AnimDockDuration).Play();
				//更改intro位置
				Transform transIntro=_intros[i].transform;
				GameObject animObjIntro=Instantiate(PrefabAnimEleObj);
				Vector3 introDest=new Vector3(_fixedDockerAndIntroXPos[i+4-_frontIndex+direction],introY,0);
				animObjIntro.GetComponent<AnimElement>().Init(
					//new Vector3(_fixedDockerAndIntroXPos[i+4-_frontIndex],introY,0)
					transIntro.position
					,introDest,	transIntro,progressLR,AnimIntroDuration).Play();
			}
			_indicators_Small[_frontIndex].GetComponent<SpriteRenderer>().sprite=_storedFrontIndicatorSmall;
			//再 更新 front的 index
			//因为只有五个index  故 取值范围 0---4
			_frontIndex=((_frontIndex-direction)<0||((_frontIndex-direction)>4))?_frontIndex:_frontIndex-direction;
			_storedFrontIndicatorSmall=_indicators_Small[_frontIndex].GetComponent<SpriteRenderer>().sprite;
			_indicators_Small[_frontIndex].GetComponent<SpriteRenderer>().sprite=
				_indicators_Big[_frontIndex].GetComponent<SpriteRenderer>().sprite;
		}
		/*//将各个元素放回原位  
		else{
			for(int i=0;i<_indicators_Small.Count;i++){
			}
		}*/
	}

	//根据x坐标 求出标题的 position 在 中间和后方最远处两点之间插值
	Vector3 GenIndicatorPosFromX(float x){
		int direction=(int)(x==0?0:x/Mathf.Abs(x));
		return Vector3.Lerp(indicatorFrontMostPos,new Vector3(direction*(-indicatorBackMostLPos.x),
			indicatorBackMostLPos.y,indicatorBackMostLPos.z
		)
			,Mathf.Abs(x/-indicatorBackMostLPos.x));
	}
}
