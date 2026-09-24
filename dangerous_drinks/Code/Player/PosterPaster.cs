using System.Diagnostics;
using Sandbox;

public sealed class PosterPaster : Component, Component.INetworkListener
{
	[Property, Sync]public int PlayerID{get; set;} = 2;
	[Property, Sync]private Sprite _default{get; set;}
	[Property, Sync]public SpriteRenderer ProfComp{get; set;}
	[Property, Sync]public TextRenderer NameComp{get; set;}
	[Property, Sync]private string Name {get; set;}
	[Property, Sync]private Sprite ProfilePicture {get; set;}
	[Property, Sync]private DDNetworkManager NetworkScene {get; set;}


	protected override void OnUpdate()
	{
		if(Connection.All.Count() == 2)
		{
			
		}
		if (Connection.All.Count() < 2)
		{
			if( PlayerID == 1)
			{
				NameAssociate();
				ImageAssociate();
			}
		}
		if(NetworkScene.Players.Count() > 0)
		{
			if(Networking.IsHost && PlayerID == 0)
			{
				NameAssociate(0);
				ImageAssociate(0);
			}
			if(Networking.IsClient && PlayerID == 1)
			{
				NameAssociate(1);
				ImageAssociate(1);
			}
		}
	}

	[Rpc.Broadcast]
	private void ImageAssociate(int ID)
	{
		var ThisPlayer = NetworkScene.Players[ID];
		if(ThisPlayer != null)
		{
			ProfilePicture = ThisPlayer.HostProfile;
			if(ProfComp.Sprite != ProfilePicture) ProfComp.Sprite = ProfilePicture;
		}
		else ProfComp.Sprite = new Sprite();
	}
	[Rpc.Broadcast]
	private void NameAssociate(int ID)
	{
		var ThisPlayer = NetworkScene.Players[ID];
		if(ThisPlayer != null)
		{
			Name = ThisPlayer.HostName;
			if(NameComp.Text != Name) NameComp.Text = Name;
			if(Name == "") NameComp.Text = "No Player";
		}
		else NameComp.Text = "No Player";
		var compScale = NameComp.TextScope.Measure();
		compScale *= NameComp.Scale;
		if (compScale.x > 25*2) NameComp.Scale -= 0.002f; //Text Scale Reduced On Large Size
		if(compScale.x < 28*2) NameComp.Scale += 0.002f; //Text Scale Reduced On Large Size
		NameComp.Scale = MathX.Clamp(NameComp.Scale,0,0.122f);
	}
	[Rpc.Broadcast]
	private void NameAssociate()
	{
		NameComp.Text = "No Player";
		var compScale = NameComp.TextScope.Measure();
		compScale *= NameComp.Scale;
		if (compScale.x > 25*2) NameComp.Scale -= 0.002f; //Text Scale Reduced On Large Size
		if(compScale.x < 28*2) NameComp.Scale += 0.002f; //Text Scale Reduced On Large Size
		NameComp.Scale = MathX.Clamp(NameComp.Scale,0,0.122f);
	}
	[Rpc.Broadcast]
	private void ImageAssociate()
	{
		ProfilePicture = new Sprite();
		if(ProfComp.Sprite != ProfilePicture) ProfComp.Sprite = _default;
	}
}
