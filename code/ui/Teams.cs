using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class Teams : Panel
{
	public Label Team;

	public Teams()
	{
		Team = Add.Label( "Invite", "teams");
	}

	public override void Tick()
	{
		var player = Local.Pawn;
		if ( player == null ) return;

		if ( player.Tags.Has( "invite" ) )
		{
			Team.Text = "Invite";
			Team.SetClass("teambystander", true);
			Team.SetClass("teammurder", false);
			Team.SetClass("teamsherif", false);
		}
		
		if ( player.Tags.Has( "tueur" ) )
		{
			Team.Text = "Tueur";
			Team.SetClass("teambystander", false);
			Team.SetClass("teammurder", true);
			Team.SetClass("teamsherif", false);
		}
		
		if ( player.Tags.Has( "agent" ) )
		{
			Team.Text = "Agent Secret";
			Team.SetClass("teambystander", false);
			Team.SetClass("teammurder", false);
			Team.SetClass("teamsherif", true);
		}

	}
}
