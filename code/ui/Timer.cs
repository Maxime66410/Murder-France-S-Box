
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;


public class Timer : Panel
{
	public Label Timers;

	public Timer()
	{
		Timers = Add.Label("En attente du nombre d'invitees (3 minimum)", "timers");
		Timers.SetClass("IsNotGame", true);
	}

	public override void Tick()
	{
		var player = Local.Pawn;
		if ( player == null ) return;

		if ( !MurderGame.Instance.PreparingGame && !MurderGame.Instance.IsGameIsLaunch && !MurderGame.Instance.InialiseGameEnd )
		{
			Timers.Text = "En attente du nombre d'invitees (" + Client.All.Count + "/3 minimum)";
		}

		if ( MurderGame.Instance.PreparingGame && !MurderGame.Instance.IsGameIsLaunch && !MurderGame.Instance.InialiseGameEnd)
		{
			Timers.Text = "La soiree commence dans " + MurderGame.Instance.RoundDuration + " Secondes";
		}

		if ( !MurderGame.Instance.PreparingGame && MurderGame.Instance.IsGameIsLaunch && !MurderGame.Instance.InialiseGameEnd)
		{
			Timers.SetClass("IsInGame", true);
			Timers.SetClass("IsNotGame", false);

			if ( player.Tags.Has( "invite" ) )
			{
				Timers.SetClass("invitetext", true);
			}
			
			if ( player.Tags.Has( "tueur" ) )
			{
				Timers.SetClass("invitetext", false);
				Timers.SetClass("tueurtext", true);
			}

			if ( player.Tags.Has( "agent" ) )
			{
				Timers.SetClass("invitetext", false);
				Timers.SetClass("agenttext", true);
			}
			
			Timers.Text = "La soiree se termine dans " + MurderGame.Instance.RoundDuration + " Secondes";
		}
		
		if ( !MurderGame.Instance.PreparingGame && !MurderGame.Instance.IsGameIsLaunch && MurderGame.Instance.InialiseGameEnd)
		{
			Timers.SetClass("invitetext", false);
			Timers.SetClass("agenttext", false);
			Timers.SetClass("tueurtext", false);
			Timers.SetClass("IsInGame", false);
			Timers.SetClass("IsNotGame", true);
			Timers.Text = "Planification d'une nouvelle soiree dans " + MurderGame.Instance.RoundDuration + " Secondes";
		}
	}
}
