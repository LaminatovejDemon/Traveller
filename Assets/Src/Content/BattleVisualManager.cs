using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleVisualManager : MonoBehaviour 
{
	
	public const float ANIMATION_SPEED = 4.0f;
	public const float SHOOT_DELTA_TIME = 0.05f;
	
	private static BattleVisualManager mInstance = null;
	
	struct ProjectileVisual
	{
		public ProjectileVisual(GameObject projectile, Part link, float angle, BattleComputer.Side targetSide, int  index, Ship targetShip, bool intoShield, float damage)
		{
			_Projectile = projectile;
			_Link = link;
			_Angle = angle;
			_index = index;
			_TargetShip = targetShip;
			_TargetSide = targetSide;
			_IntoShield = intoShield;
			_Damage = damage;
		}
		
		public float _Damage;
		public float _Angle;
		public bool _IntoShield;
		public GameObject _Projectile;
		public Part _Link;
		public Ship _TargetShip;
		public int _index;
		public BattleComputer.Side _TargetSide;
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
	
	public void ResetTurn()
	{
		HitStack_ = 0;
	}
		
	public void QueueFire(Part source, Part target, PartManager.AbilityType type, BattleComputer.Side side, int index_, Ship targetShip, bool intoShield, int damage)
	{
		GameObject weapon_ = GetWeaponVisual(source);
		GameObject weaponHit_ = GetWeaponVisual(source);
		
		if ( weapon_ == null )
		{
			return;
		}
		float angle_ = ((int)side * 20) + Random.Range(-5.0f, 5.0f);
		weapon_.gameObject.SetActive(false);
		
		_ProjectileQueue.Add(new ProjectileVisual(weapon_, source, angle_, side, index_, targetShip, intoShield, damage));
		_HitQueue.Add(new ProjectileVisual(weaponHit_, target, angle_, side, index_, targetShip, intoShield, damage));
		_ProjectileDone = false;
	}
	
	int HitStack_ = 0;
	
	void ExecuteHit(ProjectileVisual visual)
	{
		if ( visual._Link == null )
		{
			Utils.SetLayer(visual._Projectile.transform, visual._TargetShip.gameObject.layer);
			visual._Projectile.transform.rotation = visual._TargetShip.transform.rotation * Quaternion.AngleAxis( ((int)visual._TargetSide+3) * 90.0f, Vector3.up) * Quaternion.AngleAxis(90, Vector3.left);;
			visual._Projectile.transform.position = visual._TargetShip.transform.position + (visual._Projectile.transform.rotation * (Vector3.left * 10.0f + Vector3.up * visual._index));
			visual._Projectile.gameObject.SetActive(true);
//			Utils.ChangeColor(visual._Projectile.transform, Color.green);
			
			AnimationState anim_ = visual._Projectile.transform.GetChild(0).animation["Hit"];
			
			if ( anim_ == null )
			{
				anim_ = visual._Projectile.transform.GetChild(0).animation["TorpedoHit"];
			}
			
			visual._Projectile.transform.GetChild(0).animation.Play(anim_.name);
			anim_.speed = ANIMATION_SPEED;
			//visual._Projectile.transform.GetChild(0).animation.Play("Hit");
			//visual._Projectile.transform.GetChild(0).animation["Hit"].speed = ANIMATION_SPEED;
			
			AnimationCallback missCallback_ = visual._Projectile.transform.GetChild(0).gameObject.AddComponent<AnimationCallback>();
			missCallback_._DestroyWhenFinishedObject = visual._Projectile.gameObject;
			missCallback_._TargetObject = gameObject;
			missCallback_._TargetMessage = "MissFinished";
			missCallback_._TargetParameter = null;
			
			IncreaseHitStack();
			
			return;
		}
		
		Utils.SetLayer(visual._Projectile.transform, visual._Link.gameObject.layer);
		
		float angle_ = ((int)visual._TargetSide+3) * 90.0f;
		
		visual._Projectile.transform.rotation = visual._TargetShip.transform.rotation * Quaternion.AngleAxis(angle_, Vector3.up) * Quaternion.AngleAxis(90, Vector3.left);
	
		visual._Projectile.transform.parent = visual._Link.transform;
		
		if ( visual._IntoShield )
		{
			visual._Projectile.transform.position = visual._TargetShip.transform.position + (visual._Projectile.transform.rotation * (Vector3.forward * 1.5f + Vector3.up * visual._index));
			
			visual._Projectile.gameObject.SetActive(true);
			
			AnimationState anim_ = visual._Projectile.transform.GetChild(0).animation["Hit"];
			
			if ( anim_ == null )
			{
				anim_ = visual._Projectile.transform.GetChild(0).animation["TorpedoHit"];
			}
			visual._Projectile.transform.GetChild(0).animation.Play(anim_.clip.name);
			anim_.speed = ANIMATION_SPEED;
			
			AnimationCallback hitCallback_ = visual._Projectile.transform.GetChild(0).gameObject.AddComponent<AnimationCallback>();
			hitCallback_._DestroyWhenFinishedObject = visual._Projectile.gameObject;
			hitCallback_._TargetObject = gameObject;
			hitCallback_._TargetMessage = "HitShieldFinished";
			hitCallback_._TargetParameter = visual._Projectile;
			visual._TargetShip._Shield.ChangeVisualCapacity(visual._Damage);
		}
		else
		{
			visual._Projectile.transform.position = visual._Link.GetGunPoint();
			
			visual._Projectile.gameObject.SetActive(true);
			
			AnimationState anim_ = visual._Projectile.transform.GetChild(0).animation["Hit"];
			
			if ( anim_ == null )
			{
				anim_ = visual._Projectile.transform.GetChild(0).animation["TorpedoHit"];
			}
			visual._Projectile.transform.GetChild(0).animation.Play(anim_.name);
			anim_.speed = ANIMATION_SPEED;
			
			AnimationCallback hitCallback_ = visual._Projectile.transform.GetChild(0).gameObject.AddComponent<AnimationCallback>();
			hitCallback_._DestroyWhenFinishedObject = visual._Projectile.gameObject;
			hitCallback_._TargetObject = gameObject;
			hitCallback_._TargetMessage = "HitFinished";
			hitCallback_._TargetParameter = visual._Link;
			
		}
		
		IncreaseHitStack();
	}
	
	void ExecuteFire(ProjectileVisual visual)
	{
		Utils.SetLayer(visual._Projectile.transform, visual._Link.gameObject.layer);
		visual._Projectile.transform.position = visual._Link.GetGunPoint();
		visual._Projectile.transform.rotation = visual._Link.transform.rotation * Quaternion.AngleAxis(visual._Angle, Vector3.forward);
		visual._Projectile.transform.parent = visual._Link.transform;
		visual._Projectile.gameObject.SetActive(true);
		
		AnimationState anim_ = visual._Projectile.transform.GetChild(0).animation["LaserBasicAout"];
		if ( anim_ == null )
		{
			anim_ = visual._Projectile.transform.GetChild(0).animation["TorpedoBasicAout"];
		}
		anim_.speed = ANIMATION_SPEED;
		
		GameObject.Destroy(visual._Projectile.gameObject, anim_.length / ANIMATION_SPEED);
	}
	
	float _LastShotTime = -1;
	bool _ProjectileDone = false;
	
	
	void IncreaseHitStack()
	{
		++HitStack_;
	}
	
	void DecreaseHitStack()
	{
		--HitStack_;
		
		if ( HitStack_ <= 0 )
		{
			BattleManager.GetInstance().TurnEnded();
		}
	}
	
	public void MissFinished()
	{
		DecreaseHitStack();
	}
	
	public void HitShieldFinished(GameObject projectile)
	{
		GameObject kaboom_ = (GameObject)GameObject.Instantiate((GameObject)Resources.Load("Visuals/ParticleBeamKaboomShield"));
			
		kaboom_.transform.position = projectile.transform.position;
		Utils.SetLayer(kaboom_.transform, projectile.layer);
		GameObject.Destroy(kaboom_, 3.0f);
		
		DecreaseHitStack();
	}
	
	public void HitFinished(Part target)
	{
		int layer_ = target.gameObject.layer;
		
		if ( target.transform.parent.GetComponent<Ship>().VisualHitPart(target) )
		{
			GameObject kaboom_ = (GameObject)GameObject.Instantiate((GameObject)Resources.Load("Visuals/ParticleBeamKaboomContainer"));
			kaboom_.transform.position = target.GetGunPoint() + Camera.main.transform.rotation * Vector3.back * 1.0f;
			Utils.SetLayer(kaboom_.transform, layer_);
			GameObject.Destroy(kaboom_, 3.0f);
			GameObject.Destroy(target.gameObject, 1.6f);
		}
		else
		{
			GameObject kaboom_ = (GameObject)GameObject.Instantiate((GameObject)Resources.Load("Visuals/ParticleBeamKaboomSmallContainer" + Random.Range(1,5)));
			kaboom_.transform.position = target.GetGunPoint() + Camera.main.transform.rotation * Vector3.back * 1.0f;
			Utils.SetLayer(kaboom_.transform, layer_);
			GameObject.Destroy(kaboom_, 3.0f);
		}
		
		DecreaseHitStack();
	}
	
	void Update()
	{
		
		if ( _ProjectileQueue.Count > 0 && Time.time - _LastShotTime > SHOOT_DELTA_TIME )
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
		
		if ( _HitQueue.Count > 0 && Time.time - _LastShotTime > SHOOT_DELTA_TIME )
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
		case "LaserGatlingA1":
			resource_ = (GameObject)GameObject.Instantiate((GameObject)Resources.Load("Visuals/LaserBasicA"));
			resource_.transform.localScale = new Vector3(2.0f,0.1f,0.1f);
			Utils.ChangeColor(resource_.transform, Color.red);
			break;
		case "LaserHeavyA1":
			resource_ = (GameObject)GameObject.Instantiate((GameObject)Resources.Load("Visuals/LaserBasicA"));
			resource_.transform.localScale = new Vector3(1.0f,1.5f,1.5f);
			break;
		case "LaserBasicA1":
			resource_ = (GameObject)GameObject.Instantiate((GameObject)Resources.Load("Visuals/LaserBasicA"));
			Utils.ChangeColor(resource_.transform, new Color(221.0f/255.0f, 45.0f/255.0f, 0f/255.0f, 1));
			break;
		case "TorpedoA1":
			resource_ = (GameObject)GameObject.Instantiate((GameObject)Resources.Load("Visuals/TorpedoBasicA"));
			break;
		default:
			Debug.Log ("Don't know " + source.mPattern.mID);
			return null;
			break;
		}
		
		return resource_;
	}
}
