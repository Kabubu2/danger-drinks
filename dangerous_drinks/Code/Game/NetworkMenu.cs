using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.Internal;
using Sandbox.Network;
using Sandbox.UI;
using Sandbox.Utility;



public sealed class Menu : Component, Component.INetworkListener
{
protected override void OnUpdate()
	{
		
	}
void INetworkListener.OnActive( Connection connection)
	{
		Log.Info( $"{connection.DisplayName} has become active" );
		if (Networking.IsHost)
        {
            string HostID = connection.SteamId.ValueUnsigned.ToString();
            string HostName = connection.DisplayName;
        }
        else if( Networking.IsHost == false )
        {
            string ClientID = connection.SteamId.ValueUnsigned.ToString();
            string ClientName = connection.DisplayName;
        }
	}
    
    public class NetworkControl : Component , Component.INetworkListener
    {
       public bool NewConnectionsAreAllowed{get;set;} = true;
       public bool AcceptConnection( Connection connection, ref string reason )
        {
            if ( !NewConnectionsAreAllowed )
            {
                reason = "Lobby Full";
                return false;
            }

            return true;
        }

        [Property] SceneFile MainScene {get;set;}
        [Property] SceneFile MainMenuScene {get;set;}
        
        [Rpc.Broadcast]
        public void StartGame()
        {
        if ( !Networking.IsHost ) return;

        if ( MainScene != null )
            {

                Scene.Load(MainScene);
    
            }
        }

        public void KickClient()
        {
            if ( !Networking.IsHost ) return;
            foreach(var connection in Connection.All)
            {
                if(connection.IsHost)continue;
                connection.Kick("You have been Kicked");
            }
        }
        public void BacktoMain()
        {
            if ( !Networking.IsHost ) return;
            foreach(var connection in Connection.All)
            {
                if(connection.IsHost)continue;
                connection.Kick("Lobby has been closed");
            }

        if ( MainMenuScene != null )
            {

                Scene.Load(MainMenuScene);

            }
        }
    
    }
}
  
 
