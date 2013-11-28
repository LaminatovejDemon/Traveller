using UnityEngine;
using System.Collections;

public class VisualHandler : MonoBehaviour 
{
	public GameObject _FBXTemplate;
	public Vector3 _Offset;
	public float _Scale = 0.65f;
	
	void Start()
	{
		GameObject visual_ = (GameObject)GameObject.Instantiate(_FBXTemplate);
		visual_.transform.parent = transform;
		visual_.transform.localRotation = Quaternion.AngleAxis(180, Vector3.up);
		visual_.transform.localPosition = _Offset;
		visual_.transform.localScale = Vector3.one * _Scale;
	}
	
}
