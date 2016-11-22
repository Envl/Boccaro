using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class EnterPage : MonoBehaviour {
	//Private Variables------------------
	//flag
	bool flagMouseDown=false;

	List<GameObject> _lines;//线稿的集合
	List<GameObject> _poems;//诗句的集合
	List<Vector3> _fixedPosLines;//线稿停留时的固定位置
	SwipeManager _swipeManager;
	//当前在front的壶
	int _frontPotIndex=1;
	//壶的类型
	enum PotType{
		Nature,
		Geometry,
		Pattern
	}
	Vector3 PotRightPos;
	Vector3 PotLeftPos;
	Vector3 PotFrontPos;
	float _downTime=-2;
	string[] _sceneNames={"GeometryScene", "NatureScene", "PatternScene"};
	Transform _mainTitle;
	//-##########//

	//Public Variables----------------
	//壶线稿在前面和后面时候的x和z轴坐标
	//调试完毕后这几个要改成 Private //###################
	 float PotBackZ=20;
	 float PotFrontZ=0;
	 float PotBackX=8;
	 float PotBackY=-1;
	 float PotFrontY=-1.28f;
	 float PoemY=-4.9f;
	 float PoemStartProgress=0.75f;//诗句从什么程度的Progress开始播放

	public Vector3 PoemPos=new Vector3(0,-7.38f,0);
	public int AnimDuration=60;//动画持续的帧数
	public GameObject PrefabAnimEleObj;
	public GameObject PrefabPotButton;
	//-------##########//



	///
	////实例化所有诗句和线稿加入list管理
	/// ///
	void PreManageResource(){
		//初始化变量
		_lines=new List<GameObject>();
		_poems=new List<GameObject>();
		//这个位置是一一绑定给每个线稿Obj的
		_fixedPosLines=new List<Vector3>();
		//设定壶 可以放置的固定坐标
		_fixedPosLines.Add(PotLeftPos);
		_fixedPosLines.Add(PotFrontPos);
		_fixedPosLines.Add(PotRightPos);

		////////遍历所有prefab并加入list管理-------------
		Object obj;
		GameObject instance;
		for(int j=0;j<StaticGlobal.potTypes.Length;j++){
			obj=Resources.Load("EnterPage/prefabs/"+StaticGlobal.potTypes[j]+"_line");
			instance=Instantiate(obj)as GameObject;
			_lines.Add(instance);
			//把壶放到设定的位置上\
			instance.transform.position=_fixedPosLines[j];
			//根据对应的壶的位置,放置诗句的位置
			obj=Resources.Load("EnterPage/prefabs/"+StaticGlobal.potTypes[j]+"_poem");
			instance=Instantiate(obj)as GameObject;
			_poems.Add(instance);
			instance.transform.position=new Vector3(_fixedPosLines[j].x,PoemY,_fixedPosLines[j].z);
		}
		//资源用来建立GameObject后释放所有资源
		obj=null;
		Resources.UnloadUnusedAssets();
		//-------------------------///////////////////
	}
	/// <summary>
	/// 更新 left right front三个壶的位置
	/// 更新指向当前壶的索引
	/// 根据当前ProgressLR补足动画
	/// </summary>
	void UpdatePots(int direction,float progressLR){

		if(direction!=0){
			_frontPotIndex-=direction;
			_frontPotIndex=Tool.CirculateIndex(_frontPotIndex,3);
			////////////////补全三个壶和 诗句的动画效果------------------
			//遍历所有Pot,将位置改成 列表中位移方向上的下一个Pot的固定Pos 用动画渐变过去
			for(int i=0;i<_lines.Count;i++){
				//更改线稿位置
				Transform trans=_lines[i].transform;
				GameObject animElementObj=Instantiate(PrefabAnimEleObj);
				Vector3 dest=_fixedPosLines[Tool.CirculateIndex(i+direction,_fixedPosLines.Count)];
				animElementObj.GetComponent<AnimElement>().Init(_fixedPosLines[i],dest,
				trans,progressLR,AnimDuration).Play();
				//更改诗句的位置
				Transform transPoem=_poems[i].transform;
				GameObject animObjPoem=Instantiate(PrefabAnimEleObj);
				Vector3 dest2=new Vector3(dest.x,PoemY,dest.z);
				Vector3 poemStartPos=new Vector3(dest2.x-10*direction,dest2.y,dest2.z);
				animObjPoem.GetComponent<AnimElement>().Init(poemStartPos,dest2,
					transPoem,PoemStartProgress,AnimDuration).Play();
			}
			//更新 _fixedPosLines的数据
			Vector3 reserved=_fixedPosLines[0];
			for(int i=0;i>1-_lines.Count&&i<_lines.Count-1;i+=direction){
				_fixedPosLines[Tool.CirculateIndex(i,_lines.Count)]=_fixedPosLines[Tool.CirculateIndex(i+direction,_lines.Count)];
			}
			_fixedPosLines[Tool.CirculateIndex(-direction,_lines.Count)]=reserved;
			//------------------------------/////////////////////
		}
		//要将各个壶和诗句放回原位
		else{
			for(int i=0;i<_lines.Count;i++){
				Transform trans=_lines[i].transform;
				GameObject animElementObj=Instantiate(PrefabAnimEleObj);
				Vector3 dest=_fixedPosLines[Tool.CirculateIndex(i,_fixedPosLines.Count)];
				animElementObj.GetComponent<AnimElement>().Init(trans.position,dest,
					trans,0.9f-progressLR,AnimDuration).Play();
				//更改诗句的透明度----------deprecated
			}
		}
	}

	/// <summary>
	/// 根据手指当前位置,实时移动壶	
	/// 和 Poem
	/// </summary>
	void MovePotsAndPoem(int direction,float progressLR){
		for(int i=0;i<_lines.Count;i++){
			if(progressLR>0.00001f){
				_lines[i].transform.position=Vector3.Lerp(_fixedPosLines[Tool.CirculateIndex(i,_fixedPosLines.Count)],
					_fixedPosLines[Tool.CirculateIndex(i+direction,_fixedPosLines.Count)],
					progressLR);
			}
		}
	}

	// Use this for initialization
	void Start () {
		_mainTitle=GameObject.Find("main_title").transform;
		StaticGlobal.PotButton= Instantiate(PrefabPotButton)as GameObject;
		StaticGlobal.PotButton.transform.position= Camera.main.ScreenToWorldPoint(StaticGlobal.PotButtonScreenPosition);

		Controller.Instance.currentState=Controller.State.ENTERPAGE;

		_swipeManager=new SwipeManager(0.8f);
		//根据在脚本变量的设定初始化诗句和壶的展示位置 前 左 右 这三个位置
		PotRightPos=new Vector3(PotBackX,PotBackY,PotBackZ);
		PotLeftPos=new Vector3(-PotBackX,PotBackY,PotBackZ);
		PotFrontPos=new Vector3(0,PotFrontY,PotFrontZ);

		//将诗句和线稿实例化.加入list管理
		PreManageResource();
		StaticGlobal.Spin=false;

	}
	
	// Update is called once per frame
	void Update () {
		//_mainTitle.position=new Vector3(Input.acceleration.x/2,Input.acceleration.y/2+3.43f,0);
		_mainTitle.position=Tool.Interpolate(_mainTitle.position,
			new Vector3(-Input.acceleration.x*0.6f,-Input.acceleration.y*0.6f+3.43f,0)
			,0.1f);

		if(Input.GetMouseButtonUp(0)){
			if(!StaticGlobal.DraggingPot){
				//移动小于0.05个Progress  同时按下抬起在半秒内 判定为点击事件
				// 如果点击区域在壶上面 就加载对应场景
				if(_swipeManager.ProgressLR<0.05f&&
					Time.time-_downTime<0.2f
					&&Input.mousePosition.y>Screen.height/4
					&&Input.mousePosition.y<Screen.height/2){
						//将来scene的名字用数字标志  通过_frontIndex来加载对应的Scene
					StaticGlobal.Spin=true;
						SceneManager.LoadSceneAsync(_sceneNames[_frontPotIndex]);

				}

				flagMouseDown=false;
				//根据抬起位置和时间进行手势判断,之后就能用得到的手势结果了
				_swipeManager.mouseUp(Input.mousePosition);
				UpdatePots(_swipeManager.SwipeLR,_swipeManager.ProgressLR);
				//print("fron t  "+_frontPotIndex);
				//print(_swipeManager.SwipeLR);
				StaticGlobal.indicatorProgress=(float)_frontPotIndex/(float)2;
				//print(StaticGlobal.indicatorProgress);
			}
			StaticGlobal.DraggingPot=false;
		}
		if(!StaticGlobal.DraggingPot){
			////////////////////////手势的准备工作------------------------
			if(Input.GetMouseButtonDown(0)){
				_swipeManager.mouseDown(Input.mousePosition);
				flagMouseDown=true;
				_downTime=Time.time;
			}

		
			///---------------------------/////////////////////////////

			//手指未离开屏幕
			if(flagMouseDown){
				_swipeManager.CurrentPos=Input.mousePosition;
				/////////开始根据 progressLR 实时更新壶的位置 和诗句 的透明度
				MovePotsAndPoem(_swipeManager.RawSwipeLR,_swipeManager.ProgressLR);
				//把 front壶 的x坐标保存到全局, 以让indicator进行对应的移动
				float frontPotX=_lines[_frontPotIndex].transform.position.x;
				StaticGlobal.indicatorProgress=1-(float)(frontPotX+11)/(float)22;
			}


			// indicatorProgress范围  0 ~1
	//		StaticGlobal.indicatorProgress=(1+_swipeManager.ProgressLR*_swipeManager.RawSwipeLR)/2;
		}
	}
}
