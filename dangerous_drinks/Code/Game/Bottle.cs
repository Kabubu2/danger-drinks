using Sandbox;

public sealed class Bottle : Component
{
[Property] 
[Sync]public bool poisoned { get; set; }
[Property] 
[Sync]public bool empty{ get; set; }
}

