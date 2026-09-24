using Sandbox;

public sealed class Credit : Component
{
	[Property] string SteamID, Label, Contrib;
	[Property] SpriteRenderer _sprite;
	[Property] TextRenderer Name;
	[Property] TextRenderer Contribution;
	protected override void OnAwake()
	{
		var profilePicture = Texture.LoadAvatar(SteamID, 256);
        _sprite.Sprite = Sprite.FromTexture(profilePicture); 
		Name.Text = Label;
		Contribution.Text = Contrib;
	}
}
