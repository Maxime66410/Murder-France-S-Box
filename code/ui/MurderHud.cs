
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Threading.Tasks;

[Library]
public partial class MurderHud : HudEntity<RootPanel>
{
	public MurderHud()
	{
		if ( !IsClient )
			return;

		RootPanel.StyleSheet.Load( "/styles/hud.scss" );

		RootPanel.AddChild<Vitals>();
		RootPanel.AddChild<Teams>();
		RootPanel.AddChild<Timer>();
		RootPanel.AddChild<WinAndRole>();

		RootPanel.AddChild<NameTags>();
		RootPanel.AddChild<DamageIndicator>();
		RootPanel.AddChild<HitIndicator>();

		RootPanel.AddChild<InventoryBar>();
		RootPanel.AddChild<PickupFeed>();
		
		RootPanel.AddChild<ChatBox>();
		RootPanel.AddChild<Scoreboard>();
		RootPanel.AddChild<VoiceList>();
	}

	[ClientRpc]
	public void OnPlayerDied( string victim, string attacker = null )
	{
		Host.AssertClient();
	}

	[ClientRpc]
	public void ShowDeathScreen( string attackerName )
	{
		Host.AssertClient();
	}
}
