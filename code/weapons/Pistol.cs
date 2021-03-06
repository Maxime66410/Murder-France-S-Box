using Sandbox;


[Library( "mf_pistol", Title = "Pistol" )]
[Hammer.EditorModel( "weapons/pistol/pistolbarreta.vmdl" )]
partial class Pistol : BaseDmWeapon
{ 
	public override string ViewModelPath => "weapons/pistol/v_pistolbarreta.vmdl";

	public override float PrimaryRate => 15.0f;
	public override float SecondaryRate => 1.0f;
	public override float ReloadTime => 5.0f;
	
	public override int ClipSize => 1;

	public override int Bucket => 1;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "weapons/pistol/pistolbarreta.vmdl" );
		AmmoClip = 1;
	}

	public override bool CanPrimaryAttack()
	{
		return base.CanPrimaryAttack() && Input.Pressed( InputButton.Attack1 );
	}

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		if ( !TakeAmmo( 1 ) )
		{
			DryFire();
			return;
		}


		//
		// Tell the clients to play the shoot effects
		//
		ShootEffects();
		PlaySound( "rust_pistol.shoot" );

		//
		// Shoot the bullets
		//
		//Rand.SetSeed( Time.Tick );
		ShootBullet( 0.2f, 3.5f, 10000.0f, 3.0f );

	}
}
