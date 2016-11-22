using UnityEngine;
using System.Collections;

public class GLHelper 	{
		public static void glVertex2D(Vector2  vert)
		{
			GL.Vertex3((float)vert.x/(float)Screen.width,(float)vert.y/(float)Screen.height,0);
		}

		public static void glVertex2D(double x, double y)
		{
			GL.Vertex3((float)x/(float)Screen.width,(float)y/(float)Screen.height,0);
		}
	public static void DrawCenterRect(int x,int y,int width,int height,Color c){
		GL.Begin(GL.QUADS);
		GL.Color(c);

		glVertex2D(x-width/2,y-height/2);
		glVertex2D(x+width/2,y-height/2);
		glVertex2D(x+width/2,y+height/2);
		glVertex2D(x-width/2,y+height/2);
		GL.End();
	}

	public static void DrawRect(int x,int y,int width,int height,Color c){
		GL.Begin(GL.QUADS);
		GL.Color(c);

		glVertex2D(x,y);
		glVertex2D(x+width,y);
		glVertex2D(x+width,y+height);
		glVertex2D(x,y+height);
		GL.End();
	}
}
