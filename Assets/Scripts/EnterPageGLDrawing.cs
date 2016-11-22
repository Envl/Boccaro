using UnityEngine;
using System.Collections;

public class EnterPageGLDrawing : MonoBehaviour {
	public Material material;
	int IndicatorGLY=(int)(Screen.height*4.8f/23f);

	int _indicatorLeft=Screen.width/2-(int)(Screen.width*3f/90f);
	int _indicatorRight=Screen.width/2+(int)(Screen.width*3f/90f);
	// Use this for initialization
	void Start () {
		//print(IndicatorGLY);
		//print(Screens.height);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnPostRender(){
		material.SetPass(0);//不知道这句干嘛的
		GL.LoadOrtho();//2D绘图
		//  3/27== 120/1080    Indicator's background
		DrawCenterRect(Screen.width/2,IndicatorGLY,(int)(Screen.width*3f/27f),4,Tool.IntColor(222));
		//  Indicator
		DrawCenterRect(Tool.Interpolate(_indicatorLeft,_indicatorRight,StaticGlobal.indicatorProgress),
		IndicatorGLY,(int)(Screen.width*3f/81f),4,Tool.IntColor(158));
		GL.End();

	}

	void DrawCenterRect(int x,int y,int width,int height,Color c){
		GL.Begin(GL.QUADS);
		GL.Color(c);
	
		GLHelper.glVertex2D(x-width/2,y-height/2);
		GLHelper.glVertex2D(x+width/2,y-height/2);
		GLHelper.glVertex2D(x+width/2,y+height/2);
		GLHelper.glVertex2D(x-width/2,y+height/2);
		GL.End();
	}

}
