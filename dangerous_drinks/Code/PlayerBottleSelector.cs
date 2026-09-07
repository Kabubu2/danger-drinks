using Sandbox;

public sealed class PlayerBottleSelector : Component
{
	[Property]
	private GameObject bottleContainer;
	[Property]
	private GameObject[] bottles;
	private void bottleAssignment()
	{
		bottles = new GameObject[bottleContainer.Children.Count];
		for (int i = 0; i < bottleContainer.Children.Count; i++)
		{
			bottles[i] = bottleContainer.Children[i];
		}
	}
	private int _selected = 0;
	protected override void OnAwake()
	{
		bottleAssignment();
	}
	protected override void OnUpdate()
	{
		selection();
	}

	private void selection()
	{
		if(Input.Pressed("Left"))
		{
			_selected--;
			if(_selected > bottles.Length-1)_selected = 0;
			else if (_selected < 0) _selected = bottles.Length-1;
			PlayerHandController();
		}
		if(Input.Pressed("Right"))
		{
			_selected++;
			if(_selected > bottles.Length-1)_selected = 0;
			else if (_selected < 0) _selected = bottles.Length-1;
			PlayerHandController();
		}
	}

	[Property]
	private PlayerHand2Target PlayerHandCon;
	private void PlayerHandController()
	{
		PlayerHandCon.EnableIKHand = true;
		PlayerHandCon.targetPref = bottleContainer.Children[_selected];
	}

}
