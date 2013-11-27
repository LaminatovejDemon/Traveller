using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour 
{
	string mListener = "";
	Transform mCaller = null;
	GameObject mButton = null;
	public Renderer mRenderer {get; private set;}
	TextMesh mTextMesh = null;
	
	private int mFingerID = -1;
	
	public static GameObject Create(string caption, string listener, Transform caller)
	{
		GameObject ret_;
		ret_ = ((GameObject)GameObject.Instantiate(Resources.Load("ButtonContainer")));
		ret_.AddComponent<Button>();
		ret_.GetComponent<Button>().mListener = listener;
		ret_.GetComponent<Button>().mCaller = caller;
		ret_.GetComponent<Button>().mButton = ret_;
		ret_.GetComponent<Button>().mRenderer = ret_.transform.GetComponentInChildren<MeshRenderer>();
		ret_.GetComponent<Button>().mTextMesh = ret_.transform.GetComponentInChildren<TextMesh>();
		
		ret_.GetComponent<Button>().mTextMesh.text = caption;
		
		return ret_;
	}
	
	void OnTouchDown(int fingerID)
	{
		MainManager.GetInstance().AttachListner(gameObject);
		mFingerID = fingerID;
	}
	
	void OnTouchUp(int fingerID)
	{
		if ( fingerID != mFingerID )
		{
			return;
		}
		
	 	mCaller.SendMessage(mListener, SendMessageOptions.RequireReceiver);
		mFingerID = -1;
		MainManager.GetInstance().DetachListener(gameObject);
	}
}
