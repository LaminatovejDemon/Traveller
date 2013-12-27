using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleVisualManager : MonoBehaviour 
{
	
	public const float ANIMATION_SPEED = 4.0f;
	public const float SHOOT_DELTA_TIME = 0.10f;
	
	private static BattleVisualManager mInstance = null;
	

	
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
		
	public void QueueFire(Part source, Part target, PartManager.AbilityType type, BattleComputer.Side side, int index_, Ship targetShip, bool intoShield, float damage)
	{
		GameObject weapon_ = GetWeaponVisual(source);
		GameObject weaponHit_ = GetWeaponVisual(source);
		
		if ( weapon_ == null )
		{
			return;
		}
		float angle_ = ((int)side * 20) + Random.Range(-5.0f, 5.0f);
		weapon_.gameObject.SetActive(false);
		weaponHit_.gameObject.SetActive(false);

		ProjectileVisual projectileVisual_ = weapon_.AddComponent<ProjectileVisual>();
		projectileVisual_.Initialize(source, angle_, side, index_, targetShip, intoShield, damage);

		_ProjectileQueue.Add(projectileVisual_);

		ProjectileVisual hitVisual_ = weaponHit_.AddComponent<ProjectileVisual>();
		hitVisual_.Initialize( target, angle_, side, index_, targetShip, intoShield, damage);

		_HitQueue.Add(hitVisual_);
		_ProjectileDone = false;
	}
	
	int HitStack_ = 0;
	
	void ExecuteHit(ProjectileVisual visual)
	{
		if ( visual._TargetShip == null )
		{
			return;
		}

		if ( visual._Link == null )
		{
			Utils.SetLayer(visual.transform, visual._TargetShip.gameObject.layer);
			visual.transform.rotation = visual._TargetShip.transform.rotation * Quaternion.AngleAxis( ((int)visual._TargetSide+3) * 90.0f, Vector3.up) * Quaternion.AngleAxis(90, Vector3.left);;
			visual.transform.position = visual._TargetShip.transform.position + (visual.transform.rotation * (Vector3.left * 10.0f + Vector3.up * visual._index));
			visual.gameObject.SetActive(true);
//			Utils.ChangeColor(visual._Projectile.transform, Color.green);
			
			AnimationState anim_ = visual.transform.GetChild(0).animation["Hit"];
			
			if ( anim_ == null )
			{
				anim_ = visual.transform.GetChild(0).animation["TorpedoHit"];
			}
			
			visual.transform.GetChild(0).animation.Play(anim_.name);
			anim_.speed = ANIMATION_SPEED;
			//visual._Projectile.transform.GetChild(0).animation.Play("Hit");
			//visual._Projectile.transform.GetChild(0).animation["Hit"].speed = ANIMATION_SPEED;
			
			AnimationCallback missCallback_ = visual.transform.GetChild(0).gameObject.AddComponent<AnimationCallback>();
			missCallback_._TargetObject = gameObject;
			missCallback_._DestroyWhenFinishedObject = visual.gameObject;
			missCallback_._TargetMessage = "MissFinished";
			missCallback_._TargetParameter = null;
			
			IncreaseHitStack();
			
			return;
		}
		
		Utils.SetLayer(visual.transform, visual._Link.gameObject.layer);
		
		float angle_ = ((int)visual._TargetSide+3) * 90.0f;
		
		visual.transform.rotation = visual._TargetShip.transform.rotation * Quaternion.AngleAxis(angle_, Vector3.up) * Quaternion.AngleAxis(90, Vector3.left);
	
		visual.transform.parent = visual._Link.transform;
		
		if ( visual._IntoShield )
		{
			visual.transform.position = visual._TargetShip.transform.position + (visual.transform.rotation * (Vector3.forward * 1.5f + Vector3.up * visual._index));
			
			visual.gameObject.SetActive(true);
			
			AnimationState anim_ = visual.transform.GetChild(0).animation["Hit"];
			
			if ( anim_ == null )
			{
				anim_ = visual.transform.GetChild(0).animation["TorpedoHit"];
			}
			visual.transform.GetChild(0).animation.Play(anim_.clip.name);
			anim_.speed = ANIMATION_SPEED;
			
			AnimationCallback hitCallback_ = visual.transform.GetChild(0).gameObject.AddComponent<AnimationCallback>();
			hitCallback_._DestroyWhenFinishedObject = visual.gameObject;
			hitCallback_._TargetObject = gameObject;
			hitCallback_._TargetMessage = "HitShieldFinished";
			hitCallback_._TargetParameter = visual;
		}
		else
		{
			visual.transform.position = visual._Link.GetGunPoint();
			
			visual.gameObject.SetActive(true);
			
			AnimationState anim_ = visual.transform.GetChild(0).animation["Hit"];
			
			if ( anim_ == null )
			{
				anim_ = visual.transform.GetChild(0).animation["TorpedoHit"];
			}
			visual.transform.GetChild(0).animation.Play(anim_.name);
			anim_.speed = ANIMATION_SPEED;
			
			AnimationCallback hitCallback_ = visual.transform.GetChild(0).gameObject.AddComponent<AnimationCallback>();
			hitCallback_._DestroyWhenFinishedObject = visual.gameObject;
			hitCallback_._TargetObject = gameObject;
			hitCallback_._TargetMessage = "HitFinished";
			hitCallback_._TargetParameter = visual._Link;
		}
		
		IncreaseHitStack();
	}
	
	void ExecuteFire(ProjectileVisual visual)
	{
		Debug.Log ("Calling execute fire for visual" + visual);
		Utils.SetLayer(visual.transform, visual._Link.gameObject.layer);
		visual.transform.position = visual._Link.GetGunPoint();
		visual.transform.rotation = visual._Link.transform.rotation * Quaternion.AngleAxis(visual._Angle, Vector3.forward);
		
		
		// Random rotation of torpeodes
		if ( Random.value < 0.5f )
		{	
			Vector3 localScale_ = visual.transform.localScale;
			localScale_.y = -localScale_.y;
			visual.transform.localScale = localScale_;
		}
		
		visual.transform.parent = /*visual._Link.transform*/ null;
		visual.gameObject.SetActive(true);
		
		AnimationState anim_ = visual.transform.GetChild(0).animation["LaserBasicAout"];
		if ( anim_ == null )
		{
			anim_ = visual.transform.GetChild(0).animation["TorpedoBasicAout"];
		}
		anim_.speed = ANIMATION_SPEED;
		
		AnimationCallback hitCallback_ = visual.transform.GetChild(0).gameObject.AddComponent<AnimationCallback>();
		hitCallback_._TargetObject = gameObject;
		hitCallback_._DestroyWhenFinishedObject = null;
		hitCallback_._TargetMessage = "FireFinished";
		hitCallback_._EndSleep = 1.5f;
		hitCallback_._TargetParameter = visual.gameObject;
		
		++_FireCount ;
		//GameObject.Destroy(visual._Projectile.gameObject, anim_.length /*/ ANIMATION_SPEED*/);
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
	
	public void HitShieldFinished(ProjectileVisual projectileVisual)
	{
		GameObject kaboom_ = (GameObject)GameObject.Instantiate((GameObject)Resources.Load("Visuals/ParticleBeamKaboomShield"));
			
		kaboom_.transform.position = projectileVisual.transform.position;
		Utils.SetLayer(kaboom_.transform, projectileVisual.gameObject.layer);
		GameObject.Destroy(kaboom_, 3.0f);
		projectileVisual._TargetShip._Shield.ChangeVisualCapacity(projectileVisual._Damage);
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
	
	
	int _FireCount;
	
	public void FireFinished(GameObject target)
	{
		GameObject.Destroy(target, 1.5f);
		--_FireCount;
		if ( _FireCount <= 0 )
		{
			_ProjectileDone = true;
		}
	}
	
	void Update()
	{
		
		if ( _ProjectileQueue.Count > 0 && Time.time - _LastShotTime > SHOOT_DELTA_TIME )
		{
			ExecuteFire(_ProjectileQueue[0]);
			_ProjectileQueue.RemoveAt(0);
			_LastShotTime = Time.time;
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
			resource_ = (GameObject)GameObject.Instantiate((GameObject)Resources.Load("Visuals/LaserPlaceholder"));
			Debug.Log ("Don't know " + source.mPattern.mID);
			break;
		}
		
		return resource_;
	}
}
