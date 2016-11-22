using UnityEngine;
using System.Collections;

//静态类 用来存储各种静态全局字段
public static class StaticGlobal{
	//给PotButton使用
	public static bool Spin=true;
	public static GameObject PotButton;
	public static bool DraggingPot=false;
	public static bool ClickSubBtn=false;
	public static Vector3 PotButtonScreenPosition=new Vector3(Screen.width*9/10,Screen.height/4.5f,73.2f);
	//Folder Name
	public readonly static string strEnterPage="EnterPage";
	
	//Resource's type
	public readonly static string strLine="line";
	public readonly static string strPoem="poem";
	public readonly static string[] resTypes={strLine,strPoem};
	//Pot type
	public readonly static string strGeometry="geometry";
	public readonly static string strNature="nature";
	public readonly static string strPattern="pattern";//
	public readonly static string[] potTypes={strGeometry,strNature,strPattern};

	//Variable transfer between Scripts
	public static float indicatorProgress=0.5f;//Enterpage 的页面指示器

}