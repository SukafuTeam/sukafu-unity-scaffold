using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter)),RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class WindySprite:MonoBehaviour{

	[HideInInspector]
	public  MeshRenderer MeshRenderer;
	private Material _material;
	private MeshFilter _meshFilter;
	private Mesh _mesh;
	private List<Vector3> _vertices=new List<Vector3>(200);
	private List<Vector3> _uvs=new List<Vector3>(200);
	private List<Color> _colors=new List<Color>(200);
	private int[] _triangles;

	public Sprite Sprite;
	private Sprite _sprite;
	private Texture2D _texture;

	public Color Tint=Color.white;
	private Color _tint;

	[Range(0,30)]
	public int Divisions=10;
	private int _divisions;

	[Range(-2,2)]
	public float WindForce=1f;
	private float _windForce;

	[Range(0f,1f)]
	public float ShakeForce=0.03f;
	private float _shakeForce;

	[Range(0f,5f)]
	public float ShakeSpeed=1f;
	private float _shakeSpeed;

	private float _meshWidth=1;
	private float _meshHeight=1;

	[HideInInspector]
	public int SortingLayer;
	private int _sortingLayer;

	[HideInInspector]
	public int OrderInLayer;
	private int _orderInLayer;

	void OnEnable(){
		MeshRenderer=GetComponent<MeshRenderer>();
		_meshFilter=GetComponent<MeshFilter>();
		SetMeshAndMaterial();
		GenerateMesh();
		SortingLayer=MeshRenderer.sortingLayerID;
		OrderInLayer=MeshRenderer.sortingOrder;
        Update();
    }

	void Update(){
		if(
			_sprite!=Sprite || 
			_tint!=Tint ||
			_divisions!=Divisions || 
			_windForce!=WindForce || 
			_shakeForce!=ShakeForce ||
			_shakeSpeed!=ShakeSpeed ||
			_sortingLayer!=SortingLayer ||
			_orderInLayer!=OrderInLayer
		){
			SetMeshAndMaterial();
			_divisions=Divisions;
			_windForce=WindForce;
			_shakeForce=ShakeForce;
			_shakeSpeed=ShakeSpeed;
			_material.SetFloat("_WindForce",WindForce);
			_material.SetFloat("_ShakeForce",ShakeForce);
			_material.SetFloat("_ShakeSpeed",ShakeSpeed);
			if(_sprite!=Sprite){
				_sprite = Sprite;
				_material.SetTexture("_MainTex",Sprite.texture);
				if(Sprite!=null){
					if(Sprite.texture.width>Sprite.texture.height){
						_meshWidth=1f;
						_meshHeight=Sprite.texture.height/(float)Sprite.texture.width;
					}else{
						_meshWidth=Sprite.texture.width/(float)Sprite.texture.height;
						_meshHeight=1f;
					}
				}else{
					_meshWidth=1f;
					_meshHeight=1f;
				}
			}

			_tint=Tint;
			_material.SetColor("_Color",Tint);

			if(_sortingLayer!=SortingLayer || _orderInLayer!=OrderInLayer){
				MeshRenderer.sortingLayerID=SortingLayer;
				MeshRenderer.sortingOrder=OrderInLayer;
				_sortingLayer=SortingLayer;
				_orderInLayer=OrderInLayer;
			}
			GenerateMesh();
		}
	}

	void SetMeshAndMaterial(){
		if(_mesh==null){
			_mesh = new Mesh {name = "WindySpriteMesh"};
			if(_meshFilter.sharedMesh!=null) DestroyImmediate(_meshFilter.sharedMesh);
		}
		if(_meshFilter.sharedMesh==null){
			_meshFilter.sharedMesh=_mesh;
		}
		if(_material==null){
			_material = new Material(Shader.Find("Custom/WindySprite")) {name = "WindySpriteMaterial"};
			if(MeshRenderer.sharedMaterial!=null) DestroyImmediate(MeshRenderer.sharedMaterial);
		}
		if(MeshRenderer.sharedMaterial==null){
			MeshRenderer.sharedMaterial=_material;
		}
	}
	
	private void GenerateMesh(){
		const int pointsX = 3;
		var pointsY=Divisions+2;
		var squareNum=-1;
		_vertices.Clear();
		_uvs.Clear();
		_colors.Clear();
		_triangles=new int[((pointsX*pointsY)*2)*3];
		for(var y=0;y<pointsY;y++){
			for(var x=0;x<pointsX;x++){
				_vertices.Add(new Vector3(
					(x/(float)(pointsX-1)-0.5f)*_meshWidth,
					(float)y/(pointsY-1)*_meshHeight,
					0f
				));
				_uvs.Add(new Vector3(
					(float)x/(pointsX-1),
					(float)y/(pointsY-1),
					0f
				));
				//Add triangles
				if (x <= 0 || y <= 0)
					continue;
				
				var verticeNum=x + y * pointsX;
				squareNum++;
				_triangles[squareNum*6]=verticeNum-pointsX-1;
				_triangles[squareNum*6+1]=verticeNum-1;
				_triangles[squareNum*6+2]=verticeNum;
				_triangles[squareNum*6+3]=verticeNum;
				_triangles[squareNum*6+4]=verticeNum-pointsX;
				_triangles[squareNum*6+5]=verticeNum-pointsX-1;
			}
		}
		_mesh.Clear();
		_mesh.SetVertices(_vertices);
		_mesh.SetUVs(0,_uvs);
		_mesh.SetColors(_colors);
		_mesh.SetTriangles(_triangles,0);
	}
}
