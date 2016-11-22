using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class NewMaintainPage : MonoBehaviour {
	bool flagMouseDown=false;

	Transform _title,_pot,_intro,_introStamp,_stamp;
	int _frontToolIndex=0;
	bool isExploded=false;// back Tool是否分散
	bool clickedTool=false;

	Transform _world;
	SwipeManager _swipeManager;
	public GameObject PrefabAnimEleObj;
	public Transform[] Tools;
	public Transform[] ToolNames;
	public Transform[] Intros;
	Vector3[] _fixedToolPos;
	Vector3 _potOriginPos=new Vector3(0,-0.2f,0);
	Vector3[] _firstToolDestPos={
		//第一个工具在前台的目标位置
		new Vector3(0,-0.6f,-45),
		//2
		new Vector3(0.75f,0.67f,-5),
		//3
		new Vector3(0,-0.74f,-45),
		//4
		new Vector3(0.85f,0.21f,-15),
		//5
		new Vector3(1,0.5f,-16)
	};
	Vector3[] _potDestPos={
		//第一个工具时壶的目标位置
		new Vector3(0.13f,-0.97f,-5),
		//2
		new Vector3(0.13f,-0.97f,-5),
		new Vector3(0,-0.25f,-5),
		new Vector3(0.13f,-1.95f,-5),
		new Vector3(0,-2.3f,-5),
	};
	Vector3[,] _backPoses={
		//第一个工具在前台时的backPos
		{	new Vector3(-3.78f,-4.58f,5),
			new Vector3(-3.68f,1.42f,5),							
			new Vector3(2.64f,2.75f,5),
			new Vector3(3.5f,-4.87f,5)},
		//22222222
		{	new Vector3(-3.78f,-4.58f,5),
			new Vector3(-3.68f,1.42f,5),							
			new Vector3(3.64f,1.75f,5),
			new Vector3(3.5f,-4.87f,5)},
		//3333333333333
		{	new Vector3(-2.85f,-5.24f,5),
			new Vector3(-3.68f,1.42f,5),							
			new Vector3(2.26f,1.55f,5),
			new Vector3(3.5f,-4.87f,5)},
		//44444444
		{	new Vector3(-3.78f,-4.58f,5),
			new Vector3(-2.58f,1.22f,5),							
			new Vector3(3.64f,1.75f,5),
			new Vector3(3.5f,-4.87f,5)},
		//5555555
		{	new Vector3(-3.83f,-5.38f,5),
			new Vector3(-3.38f,0.42f,5),							
			new Vector3(4.14f,-0.15f,5),
			new Vector3(3.4f,-5.13f,5)}
		};//顺序是 左下角 左上角 右上角 右下角
	int AnimDuration=40;
	public GameObject PrefabPotButton;
	// Use this for initialization
	void Start () {
		_world=GameObject.Find("world").transform;
		_title=GameObject.Find("title").transform;
		_pot=GameObject.Find("pot").transform;
		_stamp=_title.GetChild(0);

		StaticGlobal.PotButton= Instantiate(PrefabPotButton)as GameObject;
		StaticGlobal.PotButton.transform.position=Camera.main.ScreenToWorldPoint(StaticGlobal.PotButtonScreenPosition);
		Controller.Instance.currentState=Controller.State.MAINTAIN;

		_swipeManager=new SwipeManager(0.7f);
		PreManagement();
		StaticGlobal.Spin=false;

		///----------Pot点击事件-----
		_pot.GetComponent<MaintainTool>().OnClick+=(toolIndex) => {
			if(isExploded){
				OnToolClick();

			}
		};
		////-----------注册tool的点击事件----------
		foreach(var trans in Tools){
			trans.GetComponent<MaintainTool>().OnClick+=(toolIndex)=>{
				//前台Tool被点击 
				if(toolIndex==_frontToolIndex){
					OnToolClick();
				}	
			};
		}
	}

	void OnToolClick(){

				clickedTool=true;
				Vector3 toAngle;
				if(_frontToolIndex==4||_frontToolIndex==3){
					toAngle=new Vector3(0,0,35);
				}
				//淡出title的stamp
				Instantiate(PrefabAnimEleObj).GetComponent<AnimElement>().Init(
					new Color(1,1,1,1),
					new Color(1,1,1,0),
					_stamp,
					0
				).SetReverseAnimation(isExploded).Play();
				//淡出title
				Instantiate(PrefabAnimEleObj).GetComponent<AnimElement>().Init(
					new Color(1,1,1,1),
					new Color(1,1,1,0),
					_title,
					0
				).SetReverseAnimation(isExploded).Play();
				//淡入Intro
				Instantiate(PrefabAnimEleObj).GetComponent<AnimElement>().Init(
					new Color(1,1,1,1),
					new Color(1,1,1,0),
					Intros[_frontToolIndex],
					0
				).SetReverseAnimation(!isExploded).Play();

				//淡入intro的stamp
				Instantiate(PrefabAnimEleObj).GetComponent<AnimElement>().Init(
					new Color(1,1,1,1),
					new Color(1,1,1,0),
					Intros[_frontToolIndex].GetChild(0),
					0
				).SetReverseAnimation(!isExploded).Play();
				//紫砂壶移动到目标位置
				Instantiate(PrefabAnimEleObj).GetComponent<AnimElement>().Init(
					_potOriginPos,
					_potDestPos[_frontToolIndex],
					_pot,
					0
				).SetReverseAnimation(isExploded).Play();
				//front tool移动到dest
				Instantiate(PrefabAnimEleObj).GetComponent<AnimElement>().Init(
					_fixedToolPos[_frontToolIndex],
					_firstToolDestPos[_frontToolIndex],
					Tools[_frontToolIndex],
					0).AddLocalRotateAnimation(Vector3.zero,toAngle)
					.SetReverseAnimation(isExploded).Play();
				for(int i=0;i<4;i++){

					//backTool分散开
					Instantiate(PrefabAnimEleObj).GetComponent<AnimElement>().Init(
						_fixedToolPos[Tool.CirculateIndex(_frontToolIndex+i+1,5)],
						_backPoses[_frontToolIndex,i],
						Tools[Tool.CirculateIndex(_frontToolIndex+i+1,5)],
						0
					).SetReverseAnimation(isExploded).Play();

				}
				isExploded=!isExploded;
			
	}

	// Update is called once per frame
	void Update () {
		
		if(Input.GetMouseButtonUp(0)){
			if(!StaticGlobal.DraggingPot){
				flagMouseDown=false;
				//根据抬起位置和时间进行手势判断,之后就能用得到的手势结果了
				_swipeManager.mouseUp(Input.mousePosition);
				//炸开状态 不响应滑动操作
				if(!clickedTool && !isExploded){
					UpdateTools(-_swipeManager.SwipeLR,_swipeManager.ProgressLR);
					}
			}
			StaticGlobal.DraggingPot=false;
			clickedTool=false;
		}
		if(!StaticGlobal.DraggingPot){
			////////////////////////手势的准备工作------------------------
			if(Input.GetMouseButtonDown(0)){
				_swipeManager.mouseDown(Input.mousePosition);
				flagMouseDown=true;
			}


			///---------------------------/////////////////////////////

			//手指未离开屏幕  炸开状态 不响应滑动操作
			if(! clickedTool&&flagMouseDown&&!isExploded){
				_swipeManager.CurrentPos=Input.mousePosition;
				/////////开始根据 progressLR 实时更新壶的位置 和诗句 的透明度
				MoveTools(-_swipeManager.RawSwipeLR,_swipeManager.ProgressLR);
				//把 front壶 的x坐标保存到全局, 以让indicator进行对应的移动
				float frontPotX=Tools[_frontToolIndex].transform.position.x;
			}


			// indicatorProgress范围  0 ~1
			//		StaticGlobal.indicatorProgress=(1+_swipeManager.ProgressLR*_swipeManager.RawSwipeLR)/2;
		}
	}

	/// 更新 left right front三个壶的位置
	/// 更新指向当前Tool的索引
	/// 根据当前ProgressLR补足动画
	void PreManagement(){
		
		//获取到固定位置
		_fixedToolPos=new Vector3[5];
		int tmpI=0;
		foreach(var trans in Tools){
			_fixedToolPos[tmpI]=new Vector3(trans.position.x,trans.position.y,trans.position.z);
			tmpI++;
		}
			
	}

	void MoveTools(int direction,float progressLR){
		for(int i=0;i<Tools.Length;i++){
			if(progressLR>0.00001f){
				Tools[i].position=Vector3.Lerp(_fixedToolPos[Tool.CirculateIndex(i,_fixedToolPos.Length)],
					_fixedToolPos[Tool.CirculateIndex(i+direction,_fixedToolPos.Length)],
					progressLR);
			}
		}
	}
	void UpdateTools(int direction,float progressLR){
		if(direction!=0){
			_frontToolIndex-=direction;
			_frontToolIndex=Tool.CirculateIndex(_frontToolIndex,5);
			////////////////补全三个壶和 诗句的动画效果------------------
			//遍历所有Pot,将位置改成 列表中位移方向上的下一个Pot的固定Pos 用动画渐变过去
			for(int i=0;i<Tools.Length;i++){
				//更改线稿位置
				Transform trans=Tools[i].transform;
				GameObject animElementObj=Instantiate(PrefabAnimEleObj);
				Vector3 dest=_fixedToolPos[Tool.CirculateIndex(i+direction,_fixedToolPos.Length)];
				animElementObj.GetComponent<AnimElement>().Init(_fixedToolPos[i],dest,
					trans,progressLR,AnimDuration).Play();
				//更改ToolName的位置
				Transform transPoem=ToolNames[i];
				GameObject animObjPoem=Instantiate(PrefabAnimEleObj);
				Vector3 dest2=new Vector3(dest.x,-3.8f,dest.z);
				Vector3 poemStartPos=new Vector3(dest2.x-10*direction,dest2.y,dest2.z);
				animObjPoem.GetComponent<AnimElement>().Init(poemStartPos,dest2,
					transPoem,0.7f,AnimDuration).Play();
			}
			//更新 _fixedToolPos的数据
			Vector3 reserved=_fixedToolPos[0];
			for(int i=0;i>1-Tools.Length&&i<Tools.Length-1;i+=direction){
				_fixedToolPos[Tool.CirculateIndex(i,Tools.Length)]=_fixedToolPos[Tool.CirculateIndex(i+direction,Tools.Length)];
			}
			_fixedToolPos[Tool.CirculateIndex(-direction,Tools.Length)]=reserved;
			//------------------------------/////////////////////
		}
		//要将各个壶和诗句放回原位
		else{
			for(int i=0;i<Tools.Length;i++){
				Transform trans=Tools[i].transform;
				GameObject animElementObj=Instantiate(PrefabAnimEleObj);
				Vector3 dest=_fixedToolPos[Tool.CirculateIndex(i,_fixedToolPos.Length)];
				animElementObj.GetComponent<AnimElement>().Init(trans.position,dest,
					trans,0.9f-progressLR,AnimDuration).Play();
				//更改诗句的透明度----------deprecated
			}
		}
	}
}
