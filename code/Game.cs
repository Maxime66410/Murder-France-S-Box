using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Text;

/// <summary>
/// This is the heart of the gamemode. It's responsible
/// for creating the player and stuff.
/// </summary>
[Library( "murderfrance", Title = "Murder France" )]
public partial class MurderGame : Game
{

	[Net] public bool IsGameIsLaunch { get; private set; }

	[Net] public bool PreparingGame { get; private set; }

	[Net] public bool InialiseGameEnd { get; private set; }

	[Net] public int RoundDuration { get; set; }

	[Net] public string WhoWin { get; set; }
	
	[Net] public bool RespawnEnabled { get; set; } = true;
	

	public static MurderGame Instance
	{
		get => Current as MurderGame;
	}
	
	
	public MurderGame()
	{
		//
		// Create the HUD entity. This is always broadcast to all clients
		// and will create the UI panels clientside. It's accessible 
		// globally via Hud.Current, so we don't need to store it.
		//
		if ( IsServer )
		{
			new MurderHud();
		}

		
	}
	
	public override void DoPlayerNoclip( Client player )
	{
		if(player.SteamId != 76561198156802806)
			return;

		base.DoPlayerNoclip( player );
	}

	public override void DoPlayerDevCam( Client player )
	{
		if(player.SteamId != 76561198156802806)
			return;

		base.DoPlayerDevCam( player );
	}

	public override void PostLevelLoaded()
	{
		base.PostLevelLoaded();

		ItemRespawn.Init();
		
		LoopCheckPlayer();
		WaitToStart();
		OnFinishedGamePreparing();
	}

	public override void ClientJoined( Client cl )
	{
		base.ClientJoined( cl );

		var player = new DeathmatchPlayer();
		player.Respawn();

		cl.Pawn = player;
	}
	
	public void CheckMinimumPlayers()
	{
		if ( Instance.IsGameIsLaunch == false )
		{
			if ( Client.All.Count >= 3 )
			{
				PreparingGames();
			}
		}
	}
	
	public void StartGame()
	{
		Instance.IsGameIsLaunch = true;
		Instance.RoundDuration = 600;
		Instance.PreparingGame = false;
		Log.Info( Instance.IsGameIsLaunch );
		OnStartGame();
		//Sound.FromScreen( "roundready.round" );
	}
	
	public void PreparingGames()
	{
		if ( Instance.PreparingGame == false && Instance.InialiseGameEnd == false )
		{
			WhoWin = "";
			Instance.PreparingGame = true;
			Instance.RoundDuration = 30;
		}
	}
	
	public async Task WaitToStart()
	{
		while ( true )
		{
			await Task.DelaySeconds( 1 );
			if ( Instance.PreparingGame == true )
			{
				if ( Instance.RoundDuration >= 1 )
				{
					Instance.RoundDuration--;
				}

				Log.Info( Instance.RoundDuration );

				if ( Instance.RoundDuration <= 0 )
				{
					Instance.RoundDuration = 0;
					StartGame();
				}

				if ( Client.All.Count <= 1 )
				{
					if ( Instance.PreparingGame )
					{
						Instance.PreparingGame = false;
						OnFinishGame();
					}

					break;
				}
			}
		}
	}
	
	public async Task LoopCheckPlayer()
		{
			while ( true )
			{
				await Task.DelaySeconds( 1 );
				CheckMinimumPlayers();
				if ( Instance.IsGameIsLaunch == true && Instance.InialiseGameEnd == false )
				{
					GameStade();

					if ( Instance.RoundDuration >= 2 )
					{
						Instance.RoundDuration--;
						CheckStatsGame();
					}

					Log.Info( Instance.RoundDuration );

					if ( Instance.RoundDuration <= 0 )
					{
						Instance.RoundDuration = 0;
						CheckStatsGame();
					}
				}
			}
		}

		public async Task GameStade()
		{
			while ( true )
			{
				await Task.DelaySeconds( 1 );

				if ( Client.All.Count <= 1 )
				{
					if ( Instance.IsGameIsLaunch )
					{
						Instance.IsGameIsLaunch = false;
						OnFinishGame();
					}

					break;
				}
			}
		}

		public void OnStartGame()
		{
			Random rand = new Random();
			var target = Client.All[rand.Next( Client.All.Count )];
			target.Pawn.Tags.Add( "tueur" );
			
			Random rands = new Random();
			var targets = Client.All[rands.Next( Client.All.Count )];
			targets.Pawn.Tags.Add( "agent" );

			foreach ( Client clients in Client.All )
			{
				if ( clients.Pawn is not DeathmatchPlayer player )
				{
					continue;
				}

				if ( !player.Tags.Has( "tueur" ) || !player.Tags.Has( "agent" ) )
				{
					player.Tags.Add("invite");
				}
				
				player.Respawn();
			}
			
			Instance.RespawnEnabled = false;

		}

		public void OnFinishGame()
		{
			Instance.InialiseGameEnd = false;

			Instance.RespawnEnabled = true;

			foreach ( Client client in Client.All )
			{
				if ( client.Pawn is not DeathmatchPlayer player )
				{
					continue;
				}

				if ( player.Tags.Has( "tueur" ) )
				{
					player.Tags.Remove( "tueur" );
				}
				
				if ( player.Tags.Has( "agent" ) )
				{
					player.Tags.Remove( "agent" );
				}
				
				if ( player.Tags.Has( "invite" ) )
				{
					player.Tags.Remove( "invite" );
				}

				player.Respawn();
			}
		}

		public void CheckStatsGame()
		{
			if ( Instance.IsGameIsLaunch == true )
			{
				var alivePlayers = 0;
				var murderAlive = 0;

				foreach ( Client cls in Client.All )
				{
					if ( cls.Pawn is not DeathmatchPlayer player )
					{
						continue;
					}

					if ( !player.IsDead && !player.IsMurder )
					{
						alivePlayers += 1;
					}

					if ( player.IsMurder && !player.IsDead )
					{
						murderAlive += 1;
					}
				}
				
				

				if ( alivePlayers == 0 )
				{
					Instance.IsGameIsLaunch = false;
					OnFinishedUpdateValues();
					WhoWin = "Le tueur a massacrer tout le monde !";
					SoundZombieWin();
				}

				if ( alivePlayers >= 1 && Instance.RoundDuration == 0 || murderAlive == 0 )
				{
					Instance.IsGameIsLaunch = false;
					OnFinishedUpdateValues();
					WhoWin = "Les inviters on survecu a la soiree !";
					SoundHumanWin();
				}
			}
		}

		public void OnFinishedUpdateValues()
		{
			Instance.RoundDuration = 10;
			Instance.InialiseGameEnd = true;
		}

		public async Task OnFinishedGamePreparing()
		{
			while ( true )
			{
				await Task.DelaySeconds( 1 );
				if ( Instance.InialiseGameEnd == true )
				{
					if ( Instance.RoundDuration >= 1 )
					{
						Instance.RoundDuration--;
					}

					Log.Info( Instance.RoundDuration );

					if ( Instance.RoundDuration <= 0 )
					{
						Instance.RoundDuration = 0;
						OnFinishGame();
					}
				}
			}
		}
		
		public void SoundHumanWin()
		{
			//Sound.FromScreen( "humanend.round" );
		}

		public void SoundZombieWin()
		{
			//Sound.FromScreen( "zombieend.round" );
		}
}
