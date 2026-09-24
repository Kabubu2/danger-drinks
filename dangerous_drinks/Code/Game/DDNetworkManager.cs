public sealed class DDNetworkManager : Component, Component.INetworkListener
{

    [Sync, Property]public PlayerInfo[] Players {get; set;} = new PlayerInfo[2];
    void INetworkListener.OnActive( Connection connection)
	{
        if(connection.IsHost && Players[0].HostID == 0)Players[0] = info(connection);
        if(!connection.IsHost && Players[1].HostID == 0)Players[1] = info(connection);   
	}

	void INetworkListener.OnDisconnected( Connection connection )
	{
		if(Players[1].HostID != 0) Players[1] = new PlayerInfo(); 
	}

    public PlayerInfo info(Connection connection)
    {
        var a = new PlayerInfo();
          
        a.HostID = connection.SteamId;
        a.HostName = connection.DisplayName;
            
        var profilePicture = Texture.LoadAvatar(a.HostID, 256);
        a.HostProfile = Sprite.FromTexture(profilePicture); 
        return a;
    }
    
}
  
 
