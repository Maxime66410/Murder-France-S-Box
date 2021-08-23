
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class Vitals : Panel
{
	public Label Health;
	public Label Timer;
	public Label Team;

	public Vitals()
	{
		Health = Add.Label( "100", "health" );
		Team = Add.Label( "", "team" );
		Team.SetClass( "team", true );
	}

	public override void Tick()
	{
		var player = Local.Pawn;
		if ( player == null ) return;

		Health.Text = $"{player.Health.CeilToInt()}";
		Health.SetClass( "danger", player.Health < 40.0f );

		if ( player.Tags.Has( "murder" ) )
		{
			Team.Text = "Tueur";
			Team.SetClass( "teammurder", true );
			Team.SetClass( "teamsherif", false );
			Team.SetClass( "team", false );
			return;
		}

		if ( player.Tags.Has( "sherif" ) )
		{
			Team.SetClass( "teammurder", false );
			Team.SetClass( "teamsherif", true );
			Team.SetClass( "team", false );
			Team.Text = "Inspecteur";
		}
		
		Team.SetClass( "team", true );
		Team.Text = "Invité";
	}
}
