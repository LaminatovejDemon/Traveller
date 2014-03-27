using UnityEngine;
using System.Collections;

public class VisualHandler : MonoBehaviour 
{
	public GameObject _FBXTemplate;
	public Vector3 _Offset;
	public float _Scale = 0.65f;
	public bool _InvertX;
	
	void Start()
	{
		GameObject visual_ = (GameObject)GameObject.Instantiate(_FBXTemplate);
		visual_.transform.parent = transform;
		visual_.layer = visual_.transform.parent.gameObject.layer;
		visual_.transform.localRotation = Quaternion.AngleAxis(180, Vector3.up);

		Vector3 scale = Vector3.one * _Scale;
		if ( _InvertX )
		{
			scale.x = -scale.x;
			_Offset.x = -_Offset.x;
		}

		visual_.transform.localPosition = _Offset;

		visual_.transform.localScale = scale;
	}
	
}
