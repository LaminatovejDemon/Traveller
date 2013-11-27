using UnityEngine;
using System.Collections;

public class TextManager : MonoBehaviour 
{
	private static TextManager mInstance = null;
	
	public static TextManager GetInstance()
	{
		if ( mInstance == null )
		{
			GameObject instanceObject_ = GameObject.Find("#TextManager");
			if ( instanceObject_ != null )
				mInstance = instanceObject_.transform.GetComponent<TextManager>();
			else
				mInstance = new GameObject("#TextManager").AddComponent<TextManager>();
		}
		return mInstance;
	}
	
	
	public GameObject GetText(string caption, float scale = 1.0f)
	{
		GameObject mTextPrefab = ((GameObject)Resources.Load("TextPrefab"));
	
		GameObject text_ = (GameObject)GameObject.Instantiate(mTextPrefab);
		text_.GetComponent<TextMesh>().characterSize *= scale;
		text_.GetComponent<TextMesh>().text = caption;
		
		return text_;
	}
}
