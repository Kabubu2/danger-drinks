using System;
using System.Diagnostics;
using System.Numerics;
using Sandbox;

public sealed class AnimManagerBot : Component
{
	[Property]
	private PlayerRelay PlayerRenderer {get; set;}
	[Property]
	private DD_PlayerControllerBot playerController{get; set;}

	public bool IsTurn {get; set;}
	[Property]
	public PlayerState playerState {get; set;}
	[Property]
	public GameObject bottleSelected {get; set;}
	protected override void OnStart()
	{
		PlayerRenderer.Renderer.OnGenericEvent += OnAnimationEvent;

	}
	protected override void OnUpdate()
	{
		LogicAssociate();
		AnimSwitcher();
	}

	[Property] GameObject _particle{get; set;}
	private void OnAnimationEvent( SceneModel.GenericEvent evt )
	{
		Log.Info("AnimEvent!");
		if(evt.String == "GrabBottle")
		{
			var a = evt.Int;
			if(a == 0) held.Enabled = false;
			else held.Enabled = true;
		}
		Log.Info("AnimEvent!");
		if(evt.String == "DropBottle")
		{
			var b = held.Clone();
			b.Parent = null;
			
			var a = _particle.Clone();
			a.WorldPosition = held.Parent.WorldPosition;
			b.WorldPosition = held.Parent.WorldPosition;
			b.AddComponent<KillMe>().Connected = a;
			b.WorldRotation = Quaternion.Identity;
			held.Enabled = false;
		}
		
	// if ( eventName == "GrabBottle" )
	// {
	// 	if(eventInt == 0) held.Enabled = false;
	// 	else if(eventInt == 1) held.Enabled = true;
		
	// }
	} 
	private bool isThisPlayer()
	{
		
		GameManagerBot gameManager = playerController.gameManager;
		int currentPlayerInt = gameManager.CurrentPlayer;
		if(currentPlayerInt < gameManager.PlayerAsset.Count())
		{
			GameObject currentPlayerAsset = gameManager.PlayerAsset[currentPlayerInt];
			return currentPlayerAsset == this.GameObject;
		}
		return false;
	}
	
	private void LogicAssociate()
	{
		
		GameManagerBot gameManager = playerController.gameManager;
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
			a.Set("lives", 3);
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
		a.Set("lives", playerController.gameManager.Players[playerController.gameManager.Whois(GameObject)].Lives);
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
				float targetSpec = MathX.Lerp(a.GetFloat("spectrum"),spectrumPoint(target.WorldPosition), Time.Delta * 5f);
				if(a.GetFloat("spectrum") != targetSpec)
				{
					a.Set("spectrum", targetSpec);
					//Log.Info(spectrumPoint(target.WorldPosition));
				}
			}
		}
	}
	[Property]public GameObject spine2 { get; set; }
	private float spectrumPoint(Vector3 localFromSpine)
	{
		var bottles = playerController.gameManager.BottlesContainer.Children;
		var bottleCount = bottles.Count();
		float[] bottleRange = new float[bottleCount];
		for (int i = 0; i < bottleCount; i++)
		{
			bottleRange[i] = spine2.LocalTransform.PointToLocal(bottles[i].WorldPosition).y;
		}
		var min = bottleRange.Min();
		var max = bottleRange.Max();
		var a = spine2.LocalTransform.PointToLocal(localFromSpine);

		var spectrumBalance = MathX.Remap(a.y, min,max,0,0.99f);
		spectrumBalance = MathF.Round(spectrumBalance, 2);
		return spectrumBalance;
	}
}
