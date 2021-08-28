
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class Scoreboard : Sandbox.UI.Scoreboard<ScoreboardEntry>
{
	protected override void AddHeader()
	{
		Header = Add.Panel( "header" );
		Header.Add.Label( "Liste des invitees", "name" );
	}
}

public class ScoreboardEntry : Sandbox.UI.ScoreboardEntry
{

	public ScoreboardEntry()
	{
		
	}

	public override void UpdateFrom( PlayerScore.Entry entry )
	{
		base.UpdateFrom( entry );
		
	}
}
