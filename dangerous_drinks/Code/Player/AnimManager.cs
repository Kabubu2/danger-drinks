using System;
using System.Diagnostics;
using Sandbox;

public enum PlayerState {start, idle, select, drink, poisoned, dud}
public sealed class AnimManager : Component
{
	///
	/// ANIMATION CONTROLLERS AND VARIABLES
	/// 
	[Property]
	private PlayerRelay PlayerRenderer {get; set;}
	[Property]
	private DD_PlayerController playerController{get; set;}

	public bool IsTurn {get; set;}
	[Property]
	public PlayerState playerState {get; set;}
	[Property]
	public GameObject bottleSelected {get; set;}
	protected override void OnStart()
	{
		PlayerRenderer.Renderer.OnGenericEvent += OnAnimationEvent;

	}
	
	[Rpc.Broadcast]
	protected override void OnUpdate()
	{
		LogicAssociate();
		AnimSwitcher();
	}
	
	[Rpc.Broadcast]
	private void OnAnimationEvent( SceneModel.GenericEvent evt )
	{
		Log.Info("AnimEvent!");
		if(evt.String == "GrabBottle")
		{
			var a = evt.Int;
			if(a == 0) held.Enabled = false;
			else held.Enabled = true;
		}
		
	// if ( eventName == "GrabBottle" )
	// {
	// 	if(eventInt == 0) held.Enabled = false;
	// 	else if(eventInt == 1) held.Enabled = true;
		
	// }
	} 
	private bool isThisPlayer()
	{
		
		GameManager gameManager = playerController.gameManager;
		int currentPlayerInt = gameManager.CurrentPlayer;
		if(currentPlayerInt < gameManager.PlayerAsset.Count())
		{
			GameObject currentPlayerAsset = gameManager.PlayerAsset[currentPlayerInt];
			return currentPlayerAsset == this.GameObject;
		}
		return false;
	}
	
	[Rpc.Broadcast]
	private void LogicAssociate()
	{
		
		GameManager gameManager = playerController.gameManager;
		bottleSelected = gameManager.bottleSelected;
		if(gameManager != null)
		{
			switch(gameManager.CurrentGameState)
			{
				case GameState.PreGame:
				playerState = PlayerState.start;
				break;
				case GameState.TurnHold:
				if(isThisPlayer() && bottleSelected != null) playerState = PlayerState.select;
				break;
				case GameState.TurnEnd:
				if(isThisPlayer()) 
				{
					Log.Info("Drank!");
					if(playerState != PlayerState.poisoned && playerState != PlayerState.dud)playerState = PlayerState.drink;
					
				}
				break;
				case GameState.PostTurn:
				if(playerState == PlayerState.drink && gameManager.DrankPoison)playerState = PlayerState.poisoned;
				if(playerState == PlayerState.drink && !gameManager.DrankPoison)playerState = PlayerState.dud;
				break;
				default:
				playerState = PlayerState.idle;
				break;
			}
		}
	}
	private void AnimSwitcher()
	{

		switch (playerState)
			{
				case PlayerState.start:
				anim_Sit();
				break;
				case PlayerState.idle:
				break;
				case PlayerState.select:
				if(isThisPlayer())anim_Select(bottleSelected);
				break;
				case PlayerState.drink:
				if(isThisPlayer())anim_Drink();
				break;
				case PlayerState.dud:
				if(isThisPlayer())anim_DrinkNormal();
				break;
				case PlayerState.poisoned:
				if(isThisPlayer())anim_DrinkHurt();
				break;
			
		}
		if(!isThisPlayer())
		{
			var a = PlayerRenderer.Renderer.Parameters;
			a.Set("dud", false);
			a.Set("drink", false);
			a.Set("poison", false);
			a.Set("select", false);
		}
	}

	[Property]
	public GameObject held{ get; set; }

	public void BottleState(bool value)
	{
		held.Enabled = value;
	}
	public void anim_Drink()	
	{
		var a = PlayerRenderer.Renderer.Parameters;
		if(!a.GetBool("drink"))a.Set("drink", true);
		a.Set("select", false);
	}
	private bool fired_Sit{get; set;}
	public void anim_Sit()	
	{
		if(!fired_Sit)
		{
			
			Log.Info("sit");
			var a = PlayerRenderer.Renderer.Parameters;
			a.Set("playerid", playerController.playerID);
			if(!a.GetBool("begin"))a.Set("begin", true);
			fired_Sit = true;
		}
		
	}
	public void anim_DrinkHurt()	
	{
		var a = PlayerRenderer.Renderer.Parameters;
		if(!a.GetBool("poison"))a.Set("poison", true);
	}
	public void anim_DrinkNormal()	
	{
		var a = PlayerRenderer.Renderer.Parameters;
		if(!a.GetBool("dud"))a.Set("dud", true);
	}

	public float animVar_Stretch(float distance)
	{
		var a = (distance-35f)/5f;
		a = Math.Clamp(a,0f,1f);
		return a;
	}

   	public void anim_Select(GameObject anim_bottle_target)
	{
		if(anim_bottle_target != null)
		{
			var target = anim_bottle_target.Children.FirstOrDefault( x => x.Name == "IK Target");
			if (target != null)
			{
				var a = PlayerRenderer.Renderer.Parameters;
				a.Set("select", true);
				Vector3 interpVec = Vector3.Lerp(PlayerRenderer.Renderer.Parameters.GetVector("ik.hand_left.position") ,target.WorldPosition, Time.Delta*5);
				float interpBlend = MathX.Lerp(PlayerRenderer.Renderer.Parameters.GetFloat("ik.lean.blend"),animVar_Stretch(Vector3.DistanceBetween(PlayerRenderer.PlayerCore.WorldPosition, interpVec)), Time.Delta * 5);
				PlayerRenderer.Renderer.Parameters.Set("ik.hand_left.enabled", true);
				PlayerRenderer.Renderer.Parameters.Set("ik.hand_left.position", interpVec);
				PlayerRenderer.Renderer.Parameters.Set("ik.hand_left.rotation", target.WorldRotation);
				PlayerRenderer.Renderer.Parameters.Set("ik.lean.position", interpVec);
				PlayerRenderer.Renderer.Parameters.Set("ik.lean.blend", interpBlend);
			}
		}
	}
}
