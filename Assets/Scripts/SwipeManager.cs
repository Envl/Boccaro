using UnityEngine;
using System.Collections;

/// <summary>
/// 这个类负责管理 
/// 翻页滑动手势的状态和进度管理
/// </summary>
public class SwipeManager {
	//---Private---Variables--------
	float _map=1;//手指滑动映射效果.如果设定为0.8 就是滑动0.8个屏幕就映射到 滑动了整个屏幕的效果
	float _progressLR=0;//swipe动作进度
	public float ProgressLR{
		get{
			return _progressLR/_map;
		}
		set{//做这个1 的截断  是防止 Progress超过1 使得过分滑动, 也就是一次只允许滑动一个屏幕
			//float tmp=Mathf.Abs(value);
			//tmp=tmp>1?1:tmp;
			_progressLR= value;
		}
	}
	float _swipeThreshold=0.4f;
	public float SwipeThreshold{
		get{
			return _swipeThreshold;
		}
		set{
			_swipeThreshold=Mathf.Lerp(0,1,value);
		}
	}
	float _progressUD=0;//swipe动作进度
	public float ProgressUD{
		get{
			return _progressUD/_map;
		}
		set{
			//float tmp=Mathf.Abs(value);
			//tmp=tmp>1?1:tmp;
			_progressUD=value;
		}
	}

	float _downTime=0,_upTime=0;
	//int _swipeLR=0;
	Vector3 _currrentPos;//手指实时坐标,(在手指没离开屏幕的时候)
	public Vector3 CurrentPos{
		set{
			_currrentPos=value;
			//根据currentPos计算各种Progress
			ProgressLR=Mathf.Abs((_currrentPos.x-_downPos.x)/_screenWidth);
			ProgressUD=Mathf.Abs((_currrentPos.y-_downPos.y)/_screenHeight);
		}
		get{
			return _currrentPos;
		}
	}
	Vector3 _downPos;//手指下落的坐标
	Vector3 _upPos;//手指离开的坐标
	/// <summary>
	///  0-----无滑动
	/// 正数---向右滑动
	/// 负数---向左滑动
	/// </summary>
	public int SwipeLR{
		get{
			float xSpeed=(_upPos.x-_downPos.x)/(_upTime-_downTime);
			float xProgress=(_upPos.x-_downPos.x)/Screen.width;
			if(Mathf.Abs(xProgress)>SwipeThreshold||Mathf.Abs(xSpeed)>1000){//滑动超过0.4或者速度超过一定值
				return (int)(xSpeed/Mathf.Abs(xSpeed));//转化成 int 的 1 -1 
			}
			return 0 ;
		}
	}
	//不做阀值限制的绝对的 左右
	public int RawSwipeLR{
		get{
			float direction=_currrentPos.x-_downPos.x;
			return (int)(direction/Mathf.Abs(direction));//转化成 int 的 1 -1 0
			//return 0;
		}
	}
	public int MovedPix{
		get{
			return (int)(_currrentPos.x-_downPos.x);
		}
	}
	//--Public---Variables---------

	//-----Functions-------------
	int _screenWidth=1080;
	int _screenHeight=1920;
	public SwipeManager(float map){
		_map=map;
		_screenWidth=Screen.width;
		_screenHeight=Screen.height;
	}

	public void mouseUp(Vector3 upPos){
		_upPos=upPos;
		_upTime=Time.time;
	}
	public void mouseDown(Vector3 downPos){
		_downPos=downPos;
		_downTime=Time.time;
	}
}
