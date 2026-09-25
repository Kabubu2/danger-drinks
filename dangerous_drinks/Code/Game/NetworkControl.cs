public class NetworkControl : Component , Component.INetworkListener
    {
      // public bool NewConnectionsAreAllowed{get;set;} = true;
       public bool AcceptConnection( Connection connection, ref string reason )
        {
            if ( !CanConnect )
            {
                reason = "Lobby Full";
                return false;
            }

            return true;
        }
        private bool CanConnect
        {
            get
            {
                return Connection.All.Count < 2;
            }
        }
        [Property,Sync] SceneFile MainScene {get;set;}
        [Property,Sync] SceneFile MainMenuScene {get;set;}
        
        [Rpc.Broadcast]
        public void StartGame()
        {
        if ( MainScene != null )
        {
            var scene = new SceneLoadOptions();
            scene.SetScene(MainScene);
            Game.ChangeScene(scene);
        }
        }

        public void KickClient()
        {
            if ( !Networking.IsHost ) return;
            foreach(var connection in Connection.All)
            {
                if(connection.IsHost)continue;
                GetComponent<DDNetworkManager>().Players[0] = new PlayerInfo();
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

            
        }
    
    }
        public class PlayerInfo
    {
        public long HostID{get;set;}
        public string HostName{get;set;}
        public Sprite HostProfile{get;set;}
    }