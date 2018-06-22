﻿using System.Collections.Generic;
using UnityEngine;
using FourDimensionalSpace;
using ShapeMetaData;

public class TestVisualization : MonoBehaviour {
    public float Angle = 30f;

    public GameObject VertexPrefab;
    public GameObject EdgePrefab;

    public List<GameObject> Vertices;
    public List<LineRenderer> Edges;

    private GameObject _parent;


    //Если поставить w = 1 все сломается =(
    public Vector4 PointOfView = new Vector4(0, 0, 1, 10);
    private Shape _shape;

	void Start () 
	{
        _shape = ShapeFactory.CreateShape(ShapeType.Hexacosichoron);
        _parent = new GameObject("ShapeParent");

		foreach (var vertex in _shape.Vertices)
        {
            var go = Instantiate(VertexPrefab, Vertex.ToThridDimensionalSpace(vertex, PointOfView), Quaternion.identity);
            go.SetActive(true);
            go.transform.SetParent(_parent.transform);
            Vertices.Add(go);
        }

	    foreach (var pair in _shape.AdjacencyList)
        {
            var go = Instantiate(EdgePrefab);
            go.SetActive(true);
            go.transform.SetParent(_parent.transform);
            LineRenderer renderer = go.GetComponent<LineRenderer>();
            Edges.Add(renderer);

            renderer.SetPositions(new[] 
            {
                Vertex.ToThridDimensionalSpace(_shape.Vertices[pair.Item1], PointOfView),
                Vertex.ToThridDimensionalSpace(_shape.Vertices[pair.Item2], PointOfView),
            });
        }
    }
        

    private void Update()
    {
        if (Input.GetAxis("XoY") != 0)
        {
            RotateShape(Input.GetAxis("XoY"), Planes.XoY);
        }
        if (Input.GetAxis("XoZ") != 0)
        {
            RotateShape(Input.GetAxis("XoZ"), Planes.XoZ);
        }
        if (Input.GetAxis("YoZ") != 0)
        {
            RotateShape(Input.GetAxis("YoZ"), Planes.YoZ);
        }
        if (Input.GetAxis("XoW") != 0)
        {
            RotateShape(Input.GetAxis("XoW"), Planes.XoW);
        }
        if (Input.GetAxis("YoW") != 0)
        {
            RotateShape(Input.GetAxis("YoW"), Planes.YoW);
        }
        if (Input.GetAxis("ZoW") != 0)
        {
            RotateShape(Input.GetAxis("ZoW"), Planes.ZoW);
        }
    }

    public void RotateShape(float angle, Planes plane)
    {
        _shape.Rotate(angle * Time.deltaTime, plane);

        for (int i = 0; i < _shape.Vertices.Length; i++)
        {
            Vertices[i].transform.position = Vertex.ToThridDimensionalSpace(_shape.Vertices[i], PointOfView);
        }

        for (int i = 0; i < _shape.AdjacencyList.Count; i++)
        {
            LineRenderer renderer = Edges[i];
    
            renderer.SetPositions(new[]
            {
                Vertex.ToThridDimensionalSpace(_shape.Vertices[_shape.AdjacencyList[i].Item1], PointOfView),
                Vertex.ToThridDimensionalSpace(_shape.Vertices[_shape.AdjacencyList[i].Item2], PointOfView),
            });
        }
    }
}
