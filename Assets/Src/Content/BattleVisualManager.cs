using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleVisualManager : MonoBehaviour 
{
	private static BattleVisualManager mInstance = null;
	
	struct ProjectileVisual
	{
		public ProjectileVisual(GameObject projectile, Part link, float angle, int  index = -1, Ship targetShip = null)
		{
			_Projectile = projectile;
			_Link = link;
			_Angle = angle;
			_index = index;
			_TargetShip = targetShip;
		}
		
		public float _Angle;
		public GameObject _Projectile;
		public Part _Link;
		public Ship _TargetShip;
		public int _index;
	};
	
	List<ProjectileVisual> _ProjectileQueue = new List<ProjectileVisual>();
	List<ProjectileVisual> _HitQueue = new List<ProjectileVisual>();
	
	public static BattleVisualManager GetInstance()
	{
		if ( mInstance == null )
		{
			GameObject instanceObject_ = GameObject.Find("#BattleVisualManager");
			if ( instanceObject_ != null )
				mInstance = instanceObject_.transform.GetComponent<BattleVisualManager>();
			else
				mInstance =  new GameObject("#BattleVisualManager").AddComponent<BattleVisualManager>();
		}
		return mInstance;
	}
	
	public void QueueFire(Part source, Part target, PartManager.AbilityType type, BattleComputer.Side side, int index_ = -1, Ship targetShip = null)
	{
		GameObject weapon_ = GetWeaponVisual(source);
		GameObject weaponHit_ = GetWeaponVisual(source);
		
		if ( weapon_ == null )
		{
			return;
		}
		float angle_ = ((int)side * 20) + Random.Range(-5.0f, 5.0f);
		weapon_.gameObject.SetActive(false);
		
		_ProjectileQueue.Add(new ProjectileVisual(weapon_, source, angle_));
		_HitQueue.Add(new ProjectileVisual(weaponHit_, target, angle_, index_, targetShip));
		_ProjectileDone = false;
	}
	
	void ExecuteHit(ProjectileVisual visual)
	{
		if ( visual._Link == null )
		{
			Utils.SetLayer(visual._Projectile.transform, visual._TargetShip.gameObject.layer);
			visual._Projectile.transform.rotation = visual._TargetShip.transform.rotation * Quaternion.AngleAxis(visual._Angle, Vector3.forward);
			visual._Projectile.transform.position = visual._TargetShip.transform.position + visual._Projectile.transform.rotation * Vector3.left * 10.0f;
			visual._Projectile.gameObject.SetActive(true);
			Utils.ChangeColor(visual._Projectile.transform, Color.green);
			visual._Projectile.transform.GetChild(0).animation.Play("Hit");
			return;
		}
		
		Utils.SetLayer(visual._Projectile.transform, visual._Link.gameObject.layer);
		visual._Projectile.transform.position = visual._Link.GetGunPoint();
		visual._Projectile.transform.rotation = visual._Link.transform.rotation * Quaternion.AngleAxis(visual._Angle, Vector3.forward);
		visual._Projectile.transform.parent = visual._Link.transform;
		visual._Projectile.gameObject.SetActive(true);
		visual._Projectile.transform.GetChild(0).animation.Play("Hit");
		AnimationCallback hitCallback_ = visual._Projectile.transform.GetChild(0).gameObject.AddComponent<AnimationCallback>();
		hitCallback_._DestroyWhenFinishedObject = visual._Projectile.gameObject;
		hitCallback_._TargetObject = gameObject;
		hitCallback_._TargetMessage = "HitFinished";
		hitCallback_._TargetParameter = visual._Link;
	}
	
	void ExecuteFire(ProjectileVisual visual)
	{
		Utils.SetLayer(visual._Projectile.transform, visual._Link.gameObject.layer);
		visual._Projectile.transform.position = visual._Link.GetGunPoint();
		visual._Projectile.transform.rotation = visual._Link.transform.rotation * Quaternion.AngleAxis(visual._Angle, Vector3.forward);
		visual._Projectile.transform.parent = visual._Link.transform;
		visual._Projectile.gameObject.SetActive(true);
		GameObject.Destroy(visual._Projectile.gameObject, 2.0f);
	}
	
	float _LastShotTime = -1;
	bool _ProjectileDone = false;
	
	public void HitFinished(Part target)
	{
		int layer_ = target.gameObject.layer;
		
		if ( target.transform.parent.GetComponent<Ship>().VisualHitPart(target) )
		{
			GameObject kaboom_ = (GameObject)GameObject.Instantiate((GameObject)Resources.Load("Visuals/ParticleBeamKaboomContainer"));
			kaboom_.transform.position = target.GetGunPoint() + Camera.main.transform.rotation * Vector3.back * 2.0f;
			Utils.SetLayer(kaboom_.transform, layer_);
			GameObject.Destroy(kaboom_, 3.0f);
			GameObject.Destroy(target.gameObject, 1.6f);
		
			// TODO DESTROYED KABOOM
		}
		else
		{
			//TODO HIT KABOOM 
		}
	}
	
	void Update()
	{
		
		if ( _ProjectileQueue.Count > 0 && Time.time - _LastShotTime > 0.3f )
		{
			ExecuteFire(_ProjectileQueue[0]);
			_ProjectileQueue.RemoveAt(0);
			_LastShotTime = Time.time;
			
			if ( _ProjectileQueue.Count == 0 )
			{
				_ProjectileDone = true;
			}
		}
		
		if ( !_ProjectileDone )
		{
			return;
		}
		
		if ( _HitQueue.Count > 0 && Time.time - _LastShotTime > 0.3f )
		{
			ExecuteHit(_HitQueue[0]);
			_HitQueue.RemoveAt(0);
			_LastShotTime = Time.time;
			
			if ( _HitQueue.Count == 0 )
			{
				//TODO END TURN
			}
		}
		
	}
	
	//public GameObject GetWeaponVisual(PartManager.AbilityType type)
	public GameObject GetWeaponVisual(Part source)
	{
		GameObject resource_;
		
		switch (source.mPattern.mID)
		{
		case "LaserGatlingA":
			resource_ = (GameObject)GameObject.Instantiate((GameObject)Resources.Load("Visuals/LaserBasicA"));
			resource_.transform.localScale = new Vector3(2.0f,0.1f,0.1f);
			Utils.ChangeColor(resource_.transform, Color.red);
			break;
		case "LaserBasicA":
			resource_ = (GameObject)GameObject.Instantiate((GameObject)Resources.Load("Visuals/LaserBasicA"));
			Utils.ChangeColor(resource_.transform, new Color(221.0f/255.0f, 45.0f/255.0f, 0f/255.0f, 1));
			break;
		default:
			Debug.Log ("Don't know " + source.mPattern.mID);
			return null;
			break;
		}
		
		return resource_;
	}
}
