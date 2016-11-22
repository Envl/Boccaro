using UnityEngine;
using System.Collections;

/// <summary>
/// 记录程序的各个状态 
/// 进行一些大方向上的设定与管理
/// 不干扰各个场景中的细节
/// </summary>
public class Controller : MonoBehaviour {
	/****** Public Variables*/
	[HideInInspector]
	public bool CheckTouch=true;
	[HideInInspector]
	public static Controller Instance{
		get{
			return _instance;
		}
	}	
	[HideInInspector]
	public State currentState;//当前状态

	//[HideInInspector]
	public enum State:int{
		GREETING=0,//欢迎界面
		ENTERPAGE=1,//程序入口界面 选择壶的类型
		NATURE=2,//单个壶的界面 \自然形体
		SELECTION=90,// 副栏目  -----甄选
		MAINTAIN=91,//副栏目 ---养壶
	}
	/********************/
	
	
	
	/******private Variables*/
	static Controller _instance;//本控制器的静态单例
	float _lastQuitTime=-2;
	
	/********************/
	
	
	//应当在awake()里面执行实例的引用,再在Start()里面执行实例的初始化
	//这样其他脚本在start中来引用他的时候,才能找到他
	void Awake(){
		if(_instance==null){
			_instance=this;//将本Controller的实例赋给instance;
			DontDestroyOnLoad(transform.gameObject);
		}
	}
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		//退出事件
		if(Input.GetKeyDown(KeyCode.Escape)){
			if(Time.time-_lastQuitTime<2)
				Application.Quit();
			_lastQuitTime=Time.time;
		}


	}


}
