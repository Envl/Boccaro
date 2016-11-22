using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlideImageScene2 : MonoBehaviour
{

	private UISpriteAnimation spriteAni;
	GameObject MySprite;
	List<TweenAlpha> tweens = new List<TweenAlpha>();
	private int num = 0;
	private Vector3 startpos;
	private Vector3 endpos;

	// Use this for initialization
	void Start()
	{
		MySprite = GameObject.Find("MySprite");
		spriteAni = MySprite.GetComponent<UISpriteAnimation>();
		spriteAni.Pause();

		List<GameObject> text=new List<GameObject>();
		for (int i = 0; i < 3; i++) {
			text.Add(Instantiate(Resources.Load("hqqScene2/text" + i)) as GameObject);
			text[i].transform.parent = GameObject.Find("Panel").transform;
			text[i].transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		}

		text[0].transform.localPosition = new Vector3(224, 493.0f, 0);
		text[1].transform.localPosition = new Vector3(0, 401.0f, 0);
		text[2].transform.localPosition = new Vector3(0, 513.0f, 0);


		for (int i = 0; i < text.Count; i++) {
			tweens.Add(text[i].GetComponent<TweenAlpha>());
			tweens[i].Play(false);
		}


		StaticGlobal.Spin=false;

	}

	// Update is called once per frame
	void Update()
	{
	}

	void OnDragStart()
	{
		startpos = Input.mousePosition;
	}

	void OnDragEnd()
	{
		endpos = Input.mousePosition;
		float delta = endpos.x - startpos.x;
		if(delta>=0)
			switch (num)
		{
		case 0:
			spriteAni.resetFrame(24);
			spriteAni.PlayTo(7);

			tweens[2].Play(false);
			tweens[0].Play(true);
			num++;
			break;
		case 1:
			spriteAni.PlayTo(13);

			tweens[0].PlayReverse();
			tweens[1].Play(true);
			num++;
			break;
		case 2:
			spriteAni.PlayTo(19);

			tweens[1].PlayReverse();

			num++;
			break;
		case 3:
			spriteAni.PlayTo(25);

			num++;
			break;
		case 4:
			tweens[2].Play(true);


			spriteAni.resetFrame(8);
			spriteAni.PlayTo(35);
			spriteAni.RebuildSpriteList();
			spriteAni.PlayTo(0);
			num = 0;
			break;
		}
		else if(delta<0)
			switch (num)
		{
		case 0:
			spriteAni.resetFrame(24);
			spriteAni.PlayTo(7);

			tweens[2].Play(false);
			tweens[0].Play(true);
			num++;
			break;
		case 1:
			spriteAni.PlayToReverse(0);

			tweens[0].PlayReverse();
			num--;
			break;
		case 2:
			spriteAni.PlayToReverse(5);

			tweens[1].PlayReverse();
			tweens[0].Play(true);
			num--;
			break;
		case 3:
			spriteAni.PlayToReverse(11);
			num--;
			break;
		case 4:
			spriteAni.PlayToReverse(17);
			num--;
			break;
		}
	}



}

