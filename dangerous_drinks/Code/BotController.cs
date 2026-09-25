using System;
using Sandbox;

public sealed class BotController : Component
{
	[Property]private GameObject bottleFolder;
	[Property]private GameManagerBot gameManagerBot;
	private void CheckUp()
	{
		if(gameManagerBot != null){if(bottleFolder == null)bottleFolder = gameManagerBot.BottlesContainer;}
		if(gameManagerBot == null){if(Scene.GetAll<GameManagerBot>() != null) gameManagerBot = Scene.GetAll<GameManagerBot>().FirstOrDefault();}
	}
	private TimeSince Timer;
	protected override void OnUpdate()
	{
		
		CheckUp();
		if(gameManagerBot != null && gameManagerBot.CurrentPlayer == 1 && gameManagerBot.CurrentGameState == GameState.TurnHold)
		{
			if(BottleSelected == null && bottleFolder != null && bottleFolder.Children.Count()>0) SelectFilter();
			else
			{
				if(Timer > 4) //Forced Delay
				{
					int random = new Random().Next(100);
					if( random < 10) BottleSelected = null;
					else 
					{
						SendChoice(this.GameObject, BottleSelected);
						Log.Info("Bot Sending");
					}
				}
			}	
		}
		
	}
	private void SelectFilter()
	{
		Log.Info("Bot Selecting");
		if(gameManagerBot != null)
		{
				BottleSelected = bottleZone();
				if (BottleSelected != null && gameManagerBot.CurrentPlayer == 1)
				{	
					SendChoice(this.GameObject, BottleSelected);
					Timer = 0f;
				}
		}
	}
	private void SendChoice(GameObject player, GameObject item)
	{
		//gameManager.PlayerDrink(player, item);
		if(player != null && item != null && gameManagerBot.CurrentPlayer == 1)
		{
			if(item.GetComponent<Bottle>() != null)
			{
				gameManagerBot.SelectDrink(item.GetComponent<Bottle>().poisoned, player, item);
			}
		}
	}
	private GameObject BottleSelected{ get; set; }
	private GameObject bottleZone()
	{
		int random = new Random().Next(bottleFolder.Children.Count());
		return bottleFolder.Children[random];
	}

}
