using Sandbox;

public sealed class NetworkManager : Component, Component.INetworkListener
{
	[Property] public GameObject PlayerPrefab {get; set;}

	[Property] public GameObject SpawnPoint {get; set;}

	public void OnActive( Connection channel)
	{
		//var clothing = new ClothingContainer();
		//clothing.Deserialize( channel.GetUserData("avatar"));

		var player = PlayerPrefab.Clone(SpawnPoint.WorldTransform);
		
		player.NetworkSpawn(channel);
	}
	
}
