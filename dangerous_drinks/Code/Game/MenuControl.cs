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
        private int TargetMenu = 0;
        [Property] MenuOptions[] _menu {get; set;}
        [Property] SkinnedModelRenderer Emblem {get; set;}
        [Property] GameObject[] HostSeeOnly{ get; set;}
        TimeSince GenericWait;
        
	    protected override void OnUpdate()
	    {
            if(Networking.IsClient && HostSeeOnly != null)
            {
                foreach (var item in HostSeeOnly)
                {
                    item.Destroy();
                }
            }
            if(Input.Pressed("Attack1"))
            {
		        var camera = Scene.Camera;
        		var ray = camera.ScreenPixelToRay(Mouse.Position);
		        var trace = Scene.Trace.Ray( ray.Position, ray.Position + ray.Forward * 999999f).Run();
                
		        if(trace.Hit && trace.GameObject.IsValid)
                {
                    if(trace.GameObject.Tags.Has("Clickable")) trace.GameObject.GetComponent<Clickable>().OnClick();
                }
                
            
        }
            if (TargetMenu == 0)
            {
                TransitionToTarget();
                if(GenericWait > 1f)
                {
                    if (Input.Pressed("jump") || Input.Pressed("attack1"))
                    {
                        TargetMenu = 1;
                        GenericWait = 0;
                    }
                }
            }

            if (TargetMenu == 1)
            {
                if(Mouse.Visibility != MouseVisibility.Visible)Mouse.Visibility = MouseVisibility.Visible;
                if(!Emblem.Parameters.GetBool("explode")) Emblem.Parameters.Set("explode", true);
                if(GenericWait > 0.5f) TransitionToTarget();
            }
	    }

        private void TransitionToTarget()
        {
            Scene.Camera.LocalPosition = Vector3.Lerp(Scene.Camera.LocalPosition, _menu[TargetMenu].DynamicPosition, Time.Delta*3);
            Scene.Camera.LocalRotation = Rotation.Lerp(Scene.Camera.LocalRotation, _menu[TargetMenu].DynamicRotation, Time.Delta*3);
        }

        public void ExitGame()
        {
            Game.Close();
        }
    }
public class MenuOptions
{
    public String MenuName;
    public Vector3 DynamicPosition {get; set;} = Vector3.Zero;
    public Rotation DynamicRotation {get; set;} = Rotation.Identity;
}

