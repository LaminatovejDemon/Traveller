using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleVisualHandler : BattleVisualBase 
{
	List<ProjectileVisual> _ProjectileQueue = new List<ProjectileVisual>();
	List<ProjectileVisual> _HitQueue = new List<ProjectileVisual>();

	protected override void ResetTurn()
	{
		base.ResetTurn();
		HitStack_ = 0;
	}
		
	public override void QueueFire(Part source, Part target, PartManager.AbilityType type, BattleComputer.Side side, int index_, Ship targetShip, bool intoShield, float damage)
	{
		GameObject weapon_ = GetWeaponVisual(source);
		GameObject weaponHit_ = GetWeaponVisual(source);
		
		if ( weapon_ == null )
		{
			Debug.Log ("\t\t ! ! ! Weapon is null, not queuing");
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
		_TurnFireDone = false;
	}
	
	int HitStack_ = 0;
	
	void ExecuteHit(ProjectileVisual visual)
	{
		if ( visual._TargetShip == null )
		{
			Debug.Log ("\t\t ! ! ! Visual target ship is null");
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
			anim_.speed = BattleVisualManager.ANIMATION_SPEED;
			//visual._Projectile.transform.GetChild(0).animation.Play("Hit");
			//visual._Projectile.transform.GetChild(0).animation["Hit"].speed = ANIMATION_SPEED;
			
			AnimationCallback missCallback_ = visual.transform.GetChild(0).gameObject.AddComponent<AnimationCallback>();
			missCallback_._TargetObject = gameObject;
			missCallback_._DestroyWhenFinishedObject = visual.gameObject;
			Debug.Log ("Calling MISS callback by " + visual);
			missCallback_._TargetMessage = "MissFinished";
			missCallback_._TargetParameter = visual;
			
			IncreaseHitStack();
			
			return;
		}
		
		Utils.SetLayer(visual.transform, visual._Link.gameObject.layer);
		
		float angle_ = ((int)visual._TargetSide+3) * 90.0f;
		
		visual.transform.rotation = visual._TargetShip.transform.rotation * Quaternion.AngleAxis(angle_, Vector3.up) * Quaternion.AngleAxis(90, Vector3.left);
	
		visual.transform.parent = visual._TargetShip.transform;
		visual.name += "_HIT";

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
			anim_.speed = BattleVisualManager.ANIMATION_SPEED;
			
			AnimationCallback hitCallback_ = visual.transform.GetChild(0).gameObject.AddComponent<AnimationCallback>();
			hitCallback_._DestroyWhenFinishedObject = visual.gameObject;
			hitCallback_._TargetObject = gameObject;
			Debug.Log ("adding HIT SHIELD Callback by " + visual);
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
			anim_.speed = BattleVisualManager.ANIMATION_SPEED;


			AnimationCallback hitCallback_ = visual.transform.GetChild(0).gameObject.GetComponent<AnimationCallback>();
			if ( hitCallback_ == null )
			{
				hitCallback_ = visual.transform.GetChild(0).gameObject.AddComponent<AnimationCallback>();
			}
			hitCallback_.Reset();

			hitCallback_._DestroyWhenFinishedObject = visual.gameObject;
			hitCallback_._TargetObject = gameObject;
			hitCallback_._TargetMessage = "HitFinished";
			hitCallback_._TargetParameter = visual;

		}
		
		IncreaseHitStack();
	}
	
	void ExecuteFire(ProjectileVisual visual)
	{
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
		visual.name += "_FIRE";
		
		AnimationState anim_ = visual.transform.GetChild(0).animation["LaserBasicAout"];
		if ( anim_ == null )
		{
			anim_ = visual.transform.GetChild(0).animation["TorpedoBasicAout"];
		}
		anim_.speed = BattleVisualManager.ANIMATION_SPEED;
		AnimationCallback hitCallback_ = visual.transform.GetChild(0).gameObject.AddComponent<AnimationCallback>();
		hitCallback_._TargetObject = gameObject;
		hitCallback_._DestroyWhenFinishedObject = visual.gameObject;
		hitCallback_._TargetMessage = "FireFinished";
		hitCallback_._EndSleep = 1.5f;
		hitCallback_._TargetParameter = visual.gameObject;
		
		++_FireCount ;
	}
	
	float _LastShotTime = -1;
	bool _TurnFireDone = false;
	
	
	void IncreaseHitStack()
	{
		++HitStack_;
	}
	
	void DecreaseHitStack()
	{
		--HitStack_;
		
		if ( HitStack_ <= 0 )
		{
			EndTurn();
		}
	}

	public void MissFinished(ProjectileVisual projectileVisual)
	{
		Debug.Log ("MISS FINISHED FOR " + projectileVisual);	
		DecreaseHitStack();
	}
	
	public void HitShieldFinished(ProjectileVisual projectileVisual)
	{
		GameObject kaboom_ = (GameObject)GameObject.Instantiate((GameObject)Resources.Load("Visuals/ParticleBeamKaboomShield"));
		Debug.Log ("HIT SHIELD FINISHED FOR " + projectileVisual);	
		kaboom_.transform.position = projectileVisual.transform.position;
		Utils.SetLayer(kaboom_.transform, projectileVisual.gameObject.layer);
		GameObject.Destroy(kaboom_, 3.0f);
		projectileVisual._TargetShip._Shield.ChangeVisualCapacity(projectileVisual._Damage);
		DecreaseHitStack();
	}
	
	public void HitFinished(ProjectileVisual visual)
	{
		DecreaseHitStack();

		// Already dead
		if ( visual._Link == null )
		{
			return;
		}

		int layer_ = visual._Link.gameObject.layer;
		Debug.Log ("HIT FINISHED FOR " + visual);
		if ( visual._Link.transform.parent.GetComponent<Ship>().VisualHitPart(visual._Link) )
		{
			GameObject kaboom_ = (GameObject)GameObject.Instantiate((GameObject)Resources.Load("Visuals/ParticleBeamKaboomContainer"));
			kaboom_.transform.position = visual._Link.GetGunPoint() + Camera.main.transform.rotation * Vector3.back * 1.0f;
			Utils.SetLayer(kaboom_.transform, layer_);
			GameObject.Destroy(kaboom_, 3.0f);
			GameObject.Destroy(visual._Link.gameObject, 1.6f);
		}
		else
		{
			GameObject kaboom_ = (GameObject)GameObject.Instantiate((GameObject)Resources.Load("Visuals/ParticleBeamKaboomSmallContainer" + Random.Range(1,5)));
			kaboom_.transform.position = visual._Link.GetGunPoint() + Camera.main.transform.rotation * Vector3.back * 1.0f;
			Utils.SetLayer(kaboom_.transform, layer_);
			GameObject.Destroy(kaboom_, 3.0f);
		}
	}
	
	
	int _FireCount;
	
	public void FireFinished(GameObject target)
	{
		--_FireCount;
		if ( _FireCount <= 0 )
		{
			_TurnFireDone = true;
		}
	}

	float _CameraRotationAmount;

	public override void InitializeBattle()
	{
		base.InitializeBattle();

		_CameraRotationAmount = Random.Range(-2.0f, 2.0f);
	}

	public override void InitializeTurn()
	{
		base.InitializeTurn();

		_TurnFireDone = false;

	}

	// leave without calling base when we want to wait for visual
	public override void AttackFinished()
	{
		if ( _ProjectileQueue.Count == 0 )
		{
			_TurnFireDone = true;
		}
	}

	public override void ProceedHit()
	{
		base.ProceedHit();

		if ( _HitQueue.Count == 0 )
		{
			EndTurn();
		}
	}
	
	void Update()
	{
		if ( _CameraRotationContainer != null )
		{
			_CameraRotationContainer.Rotate(Vector3.up, Time.deltaTime * _CameraRotationAmount);
		}

		if ( _ProjectileQueue.Count > 0 && Time.time - _LastShotTime > BattleVisualManager.SHOOT_DELTA_TIME )
		{
			ExecuteFire(_ProjectileQueue[0]);
			_ProjectileQueue.RemoveAt(0);
			_LastShotTime = Time.time;
		}

		if ( _TurnHitProceedEnabled )
		{
			if ( _HitQueue.Count > 0 && Time.time - _LastShotTime > BattleVisualManager.SHOOT_DELTA_TIME )
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

		// all fired
		if ( _TurnFireDone )
		{
			BattleVisualManager.GetInstance().HandlerFireEnded();
			_TurnFireDone = false;
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
