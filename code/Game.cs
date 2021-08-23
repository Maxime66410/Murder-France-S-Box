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

namespace MurderFrance {

	[Library( "murderfrance", Title = "Murder France" )]
	public partial class MurderFrance : Game
	{
		[Net] public bool IsGameIsLaunch { get; private set; }

		[Net] public bool PreparingGame { get; private set; }

		[Net] public bool InialiseGameEnd { get; private set; }

		[Net] public int RoundDuration { get; set; }

		[Net] public string WhoWin { get; set; }

		public static MurderFrance Instance
		{
			get => Current as MurderFrance;
		}

		public MurderFrance()
		{

			if ( IsServer )
			{
				new MurderHud();
			}
		}

		public override void DoPlayerNoclip( Client player )
		{
			if ( player.SteamId != 76561198156802806 )
				return;
			base.DoPlayerNoclip( player );
		}

		public override void DoPlayerDevCam( Client player )
		{
			if ( player.SteamId != 76561198156802806 )
				return;
			base.DoPlayerDevCam( player );
		}

		public static bool ClientIsBot( Client client )
		{
			return ((client.SteamId >> 52) & 0b1111) == 4;
		}

		public void CheckMinimumPlayers()
		{
			if ( Instance.IsGameIsLaunch == false )
			{
				if ( Client.All.Count >= 2 )
				{
					PreparingGames();
				}
			}
		}

		public void StartGame()
		{
			Instance.IsGameIsLaunch = true;
			Instance.RoundDuration = 120;
			Instance.PreparingGame = false;
			Log.Info( Instance.IsGameIsLaunch );
			OnStartGame();
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

			var player = new PlayerMurder();
			player.Respawn();

			cl.Pawn = player;
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

					if ( Instance.RoundDuration >= 1 )
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
			target.Pawn.Tags.Add( "murder" );
			
			Random rands = new Random();
			var targets = Client.All[rands.Next( Client.All.Count )];
			if ( targets.Pawn.Tags.Has( "murder" ) )
			{
				Random randss = new Random();
				var targetss = Client.All[randss.Next( Client.All.Count )];
				targets.Pawn.Tags.Add("sherif");
			}
			else
			{
				targets.Pawn.Tags.Add("sherif");
			}

			foreach ( Client clients in Client.All )
			{
				if ( clients.Pawn is not PlayerMurder player )
				{
					continue;
				}

				player.Respawn();
			}

		}

		public void OnFinishGame()
		{
			Instance.InialiseGameEnd = false;

			foreach ( Client client in Client.All )
			{
				if ( client.Pawn is not PlayerMurder player )
				{
					continue;
				}

				if ( player.Tags.Has( "murder" ) )
				{
					player.Tags.Remove( "murder" );
				}
				
				if ( player.Tags.Has( "sherif" ) )
				{
					player.Tags.Remove( "sherif" );
				}

				player.Respawn();
			}
		}

		public void CheckStatsGame()
		{
			if ( Instance.IsGameIsLaunch == true )
			{
				var alivePlayers = 0;

				foreach ( Client cls in Client.All )
				{
					if ( cls.Pawn is not PlayerMurder player )
					{
						continue;
					}

					if ( !player.IsDead && !player.IsMurder )
					{
						alivePlayers += 1;
					}
				}

				if ( alivePlayers == 0 )
				{
					Instance.IsGameIsLaunch = false;
					OnFinishedUpdateValues();
					WhoWin = "Le Tueur a gagné !";
				}

				if ( alivePlayers >= 1 && Instance.RoundDuration == 0 )
				{
					Instance.IsGameIsLaunch = false;
					OnFinishedUpdateValues();
					WhoWin = "Les invités ont gagné !";
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

	}

}
