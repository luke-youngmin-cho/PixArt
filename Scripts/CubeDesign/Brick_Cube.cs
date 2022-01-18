﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick_Cube : Brick
{
	// mesh
	Vector3 vertLeftTopFront;
	Vector3 vertRightTopFront;
	Vector3 vertRightTopBack;
	Vector3 vertLeftTopBack;
	float osl = 0.5f;// mesh scale factor

	private void Awake()
	{
		ChangeMeshToCube();
	}
	private void ChangeMeshToCube()
	{
		vertLeftTopFront = new Vector3(-osl, osl, osl);
		vertRightTopFront = new Vector3(osl, osl, osl);
		vertRightTopBack = new Vector3(osl, osl, -osl);
		vertLeftTopBack = new Vector3(-osl, osl, -osl);
		UpdateMeshShape();
	}

	// Use this for initialization
	void UpdateMeshShape()
	{
		MeshFilter mf = this.gameObject.GetComponent<MeshFilter>();
		Mesh mesh = mf.mesh;

		//Vertices//
		Vector3[] vertices = new Vector3[]
		{
			//front face//
			vertLeftTopFront,//left top front, 0
			vertRightTopFront,//right top front, 1
			new Vector3(-osl,-osl,osl),//left bottom front, 2
			new Vector3(osl,-osl,osl),//right bottom front, 3

			//back face//
			vertRightTopBack,//right top back, 4
			vertLeftTopBack,//left top back, 5
			new Vector3(osl,-osl,-osl),//right bottom back, 6
			new Vector3(-osl,-osl,-osl),//left bottom back, 7

			//left face//
			vertLeftTopBack,//left top back, 8
			vertLeftTopFront,//left top front, 9
			new Vector3(-osl,-osl,-osl),//left bottom back, 10
			new Vector3(-osl,-osl,osl),//left bottom front, 11

			//right face//
			vertRightTopFront,//right top front, 12
			vertRightTopBack,//right top back, 13
			new Vector3(osl,-osl,osl),//right bottom front, 14
			new Vector3(osl,-osl,-osl),//right bottom back, 15

			//top face//
			vertLeftTopBack,//left top back, 16
			vertRightTopBack,//right top back, 17
			vertLeftTopFront,//left top front, 18
			vertRightTopFront,//right top front, 19

			//bottom face//
			new Vector3(-osl,-osl,osl),//left bottom front, 20
			new Vector3(osl,-osl,osl),//right bottom front, 21
			new Vector3(-osl,-osl,-osl),//left bottom back, 22
			new Vector3(osl,-osl,-osl)//right bottom back, 23

		};

		//Triangles// 3 points, clockwise determines which side is visible
		int[] triangles = new int[]
		{
			//front face//
			0,2,3,//first triangle
			3,1,0,//second triangle

			//back face//
			4,6,7,//first triangle
			7,5,4,//second triangle

			//left face//
			8,10,11,//first triangle
			11,9,8,//second triangle

			//right face//
			12,14,15,//first triangle
			15,13,12,//second triangle

			//top face//
			16,18,19,//first triangle
			19,17,16,//second triangle

			//bottom face//
			20,22,23,//first triangle
			23,21,20//second triangle
		};

		//UVs//
		Vector2[] uvs = new Vector2[]
		{
			//front face// 0,0 is bottom left, 1,1 is top right//
			new Vector2(0,osl),
			new Vector2(0,0),
			new Vector2(osl,osl),
			new Vector2(osl,0),

			new Vector2(0,osl),
			new Vector2(0,0),
			new Vector2(osl,osl),
			new Vector2(osl,0),

			new Vector2(0,osl),
			new Vector2(0,0),
			new Vector2(osl,osl),
			new Vector2(osl,0),

			new Vector2(0,osl),
			new Vector2(0,0),
			new Vector2(osl,osl),
			new Vector2(osl,0),

			new Vector2(0,osl),
			new Vector2(0,0),
			new Vector2(osl,osl),
			new Vector2(osl,0),

			new Vector2(0,osl),
			new Vector2(0,0),
			new Vector2(osl,osl),
			new Vector2(osl,0)


		};

		mesh.Clear();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		mesh.Optimize();
		mesh.RecalculateNormals();
	}
}
