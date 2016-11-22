using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlideImage : MonoBehaviour {

	private UISpriteAnimation spriteAni;
	GameObject MySprite;
	private int num = 0;
	List<TweenAlpha> tweens = new List<TweenAlpha>();
	List<UIButton> buttons = new List<UIButton>();
	List<TweenAlpha> tweenBtn = new List<TweenAlpha>();
	private bool PrepareToBtn=false;
	private Vector3 startpos;
	private Vector3 endpos;

	// Use this for initialization
	void Start () {
		MySprite = GameObject.Find("MySprite");
		spriteAni= MySprite.GetComponent<UISpriteAnimation> ();
		spriteAni.Pause ();

		//获取images中的子物体
		GameObject images = GameObject.Find ("images");
		for(int i=0;i<images.transform.childCount;i++){
			GameObject _text= images.transform.GetChild(i).gameObject;
			TweenAlpha _tween = _text.GetComponent<TweenAlpha> ();
			//将每个text都隐藏起来
			_tween.Play (false);
			tweens.Add (_tween);
		}
		//获取按钮和其透明度
		GameObject btn = GameObject.Find ("buttons");
		for(int i=0;i<btn.transform.childCount;i++){
			GameObject gameobject= btn.transform.GetChild(i).gameObject;
			TweenAlpha _tween = gameobject.GetComponent<TweenAlpha> ();
			UIButton _btn = gameobject.GetComponent<UIButton> ();
			_tween.Play (false);
			buttons.Add (_btn);
			tweenBtn.Add (_tween);
		}

		StaticGlobal.Spin=false;
	}

	// Update is called once per frame
	void Update () {
		if (PrepareToBtn) {
			EventDelegate.Add (buttons [0].onClick, ClickButton0);
			EventDelegate.Add (buttons [1].onClick, ClickButton1);
			EventDelegate.Add (buttons [2].onClick, ClickButton2);
		} else {
			EventDelegate.Remove (buttons [0].onClick, ClickButton0);
			EventDelegate.Remove (buttons [1].onClick, ClickButton1);
			EventDelegate.Remove (buttons [2].onClick, ClickButton2);
		}
	}

	void OnDragStart() {
		startpos = Input.mousePosition;
	}

	void OnDragEnd(){
		endpos = Input.mousePosition;
		float delta = endpos.x - startpos.x;
		//向右滑
		if(delta>0)
			switch (num) {
		case 0:
			//播放第一个text
			tweens [0].Play (true);
			tweens [10].Play (false);
			spriteAni.resetFrame (24);
			spriteAni.RebuildSpriteList();
			spriteAni.PlayTo (7);

			num++;
			break;
		case 1:
			tweens [0].PlayReverse ();
			tweens [1].Play (true);
			spriteAni.PlayTo(13);

			num++;
			break;
		case 2:
			spriteAni.PlayTo (19);
			tweens [1].PlayReverse ();
			tweens [2].Play (true);
			EventDelegate.Add (tweens [2].onFinished, addcircle);

			num++;
			break;
		case 3:
			spriteAni.PlayTo (25);
			tweens [2].Play (false);
			tweens [3].Play (false);
			tweens [4].Play (false);
			tweens [8].Play (true);
			EventDelegate.Add (tweens [8].onFinished, addTweens9);
			//case 2 's delete
			////////////////////////////////////
			EventDelegate.Remove (tweens [2].onFinished, addcircle);
			EventDelegate.Remove (tweens [3].onFinished, addtext);
			PrepareToBtn = false;
			tweenBtn [0].Play (false);
			tweenBtn [1].Play (false);
			tweenBtn [2].Play (false);
			tweens [7].Play (false);
			tweens [5].Play (false);
			tweens [6].Play (false);
			/////////////////////////////////
			num++;
			break;
		case 4:
			EventDelegate.Remove (tweens [8].onFinished, addTweens9);
			tweens [9].PlayReverse ();
			tweens [8].PlayReverse ();
			spriteAni.resetFrame (8);
			spriteAni.PlayTo (35);
			spriteAni.RebuildSpriteList ();
			spriteAni.PlayTo (0);
			tweens [10].Play (true);
			num = 0;
			break;
		}
		//向左滑
		else if (delta < 0)
		{
			for (int i = 0; i < tweens.Count; i++)
				tweens[i].Play(false);


			switch (num)
			{
			case 0:
				tweens[0].Play(true);
				spriteAni.resetFrame(24);
				spriteAni.RebuildSpriteList();
				spriteAni.PlayTo(7);
				num++;
				break;
			case 1:
				spriteAni.PlayToReverse(0);
				num--;
				break;
			case 2:
				spriteAni.PlayToReverse(5);
				tweens[0].Play(true);
				num--;
				break;
			case 3:
				EventDelegate.Remove(tweens[2].onFinished, addcircle);
				EventDelegate.Remove(tweens[3].onFinished, addtext);
				PrepareToBtn = false;
				tweenBtn[0].Play(false);
				tweenBtn[1].Play(false);
				tweenBtn[2].Play(false);

				spriteAni.PlayToReverse(11);
				tweens[1].Play(true);

				num--;
				break;
			case 4:
				spriteAni.PlayToReverse(17);
				EventDelegate.Add(tweens[2].onFinished, addcircle);
				tweens[2].Play(true);
				num--;
				break;
			}
		}
	}


	void addcircle(){
		tweens [3].Play (true);
		EventDelegate.Add (tweens [3].onFinished, addtext);
	}

	void addtext(){
		tweens [4].Play (true);
		PrepareToBtn = true;
		tweenBtn [0].Play (true);
		tweenBtn [1].Play (true);
		tweenBtn [2].Play (true);
	}

	void ClickButton0(){
		tweens [4].Play (false);
		tweens [5].Play (true);
		tweens [6].Play (false);
		tweens [7].Play (false);
	}

	void ClickButton1(){
		tweens [4].Play (false);
		tweens [6].Play (false);
		tweens [5].Play (false);
		tweens [7].Play (true);
	}

	void ClickButton2(){
		tweens [4].Play (false);
		tweens [7].Play (false);
		tweens [5].Play (false);
		tweens [6].Play (true);

	}

	void addTweens9(){
		tweens [9].Play (true);
	}
}
