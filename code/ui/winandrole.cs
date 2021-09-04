using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class WinAndRole : Panel
{
	public Label WhoWinAndRole;
	private int Countreset = 1;
	private bool AlreadyViewRole = false;
	private bool AlreadyViewWhoWin = false;

	public WinAndRole()
	{
		WhoWinAndRole = Add.Label( "Role", "winandrole" );
		WhoWinAndRole.SetClass("IsNotLaunch", true);
	}

	public override void Tick()
	{
		var player = Local.Pawn;
		if ( player == null ) return;

		if ( MurderGame.Instance.IsGameIsLaunch )
		{
			if ( !AlreadyViewRole )
			{
				WhoWinAndRole.SetClass("IsNotLaunch", false);
				WhoWinAndRole.SetClass("IsLaunch", true);

				if ( player.Tags.Has( "tueur" ) )
				{
					WhoWinAndRole.SetClass("invitetexts", false);
					WhoWinAndRole.SetClass("agenttexts", false);
					WhoWinAndRole.SetClass("neutral", false);
					WhoWinAndRole.SetClass("tueurtexts", true);
					WhoWinAndRole.Text = "Vous etes un Tueur, faite un massacre !";
				}
				
				if ( player.Tags.Has( "agent" ) )
				{
					WhoWinAndRole.SetClass("invitetexts", false);
					WhoWinAndRole.SetClass("tueurtexts", false);
					WhoWinAndRole.SetClass("neutral", false);
					WhoWinAndRole.SetClass("agenttexts", true);
					WhoWinAndRole.Text = "Vous etes un Agent, securiser cette fete !";
				}

				if ( !player.Tags.Has( "agent" ) )
				{
					if ( !player.Tags.Has("tueur") )
					{
						if ( player.Tags.Has( "invite" ) )
						{
							WhoWinAndRole.SetClass("tueurtexts", false);
							WhoWinAndRole.SetClass("agenttexts", false);
							WhoWinAndRole.SetClass("neutral", false);
							WhoWinAndRole.SetClass("invitetexts", true);
							WhoWinAndRole.Text = "Vous etes un Inviter, bonne fete !";
						}
					}
				}
				
				if ( Countreset == 1 )
				{
					Countreset = 300;
				}

				if ( Countreset >= 2 )
				{
					Countreset--;
				}
				

				if ( Countreset <= 2 )
				{
					Countreset = 1;
					AlreadyViewRole = true;
					WhoWinAndRole.SetClass("IsNotLaunch", true);
					WhoWinAndRole.SetClass("IsLaunch", false);
				}

			}
		}
		else
		{
			AlreadyViewRole = false;
		}

		if ( MurderGame.Instance.InialiseGameEnd )
		{

			if ( !AlreadyViewWhoWin )
			{

				WhoWinAndRole.Text = MurderGame.Instance.WhoWin;

				if ( MurderGame.Instance.WhoWin == "Le tueur a massacrer tout le monde !" )
				{
					WhoWinAndRole.SetClass("IsNotLaunch", false);
					WhoWinAndRole.SetClass("agenttexts", false);
					WhoWinAndRole.SetClass("invitetexts", false);
					WhoWinAndRole.SetClass("tueurtexts", true);
					WhoWinAndRole.SetClass("IsLaunch", true);
				}

				if ( MurderGame.Instance.WhoWin == "Les inviters on survecu a la soiree !" )
				{
					WhoWinAndRole.SetClass("IsNotLaunch", false);
					WhoWinAndRole.SetClass("agenttexts", false);
					WhoWinAndRole.SetClass("tueurtexts", false);
					WhoWinAndRole.SetClass("invitetexts", true);
					WhoWinAndRole.SetClass("IsLaunch", true);
				}
				
				if ( Countreset == 1 )
				{
					Countreset = 500;
				}

				if ( Countreset >= 2 )
				{
					Countreset--;
				}
				

				if ( Countreset <= 2 )
				{
					Countreset = 1;
					AlreadyViewWhoWin = true;
					WhoWinAndRole.SetClass("IsNotLaunch", true);
					WhoWinAndRole.SetClass("IsLaunch", false);
				}
			}
			
			
		}
		
		if ( MurderGame.Instance.PreparingGame )
		{
			AlreadyViewRole = false;
			AlreadyViewWhoWin = false;
		}
	}
}
