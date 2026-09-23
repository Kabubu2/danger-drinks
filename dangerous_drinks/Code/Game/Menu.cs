using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.Internal;
using Sandbox.Network;
using Sandbox.UI;
using Sandbox.Utility;


public sealed class MenuControl : Component
    {
        [Property] SceneFile NetworkScene {get;set;}
         public void LoadLoby()
        {

        if ( NetworkScene != null )
            {

                Scene.Load(NetworkScene);
    
            }
        }
         public void ExitGame()
        {

        Game.Close();

        }
    }

