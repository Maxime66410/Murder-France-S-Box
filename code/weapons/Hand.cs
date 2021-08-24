using Sandbox;


[Library( "mf_hand", Title = "Hand", Spawnable = false )]
[Hammer.EditorModel( "weapons/rust_pistol/rust_pistol.vmdl" )]
partial class Hand : BaseDmWeapon
{ 
	public override string ViewModelPath => "";

	public override float PrimaryRate => 15.0f;
	public override float SecondaryRate => 1.0f;
	public override float ReloadTime => 3.0f;

	public override AmmoType AmmoType => AmmoType.Hand;

	public override int Bucket => 1;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "" );
		AmmoClip = 0;
	}

	public override bool CanPrimaryAttack()
	{
		return base.CanPrimaryAttack() && Input.Pressed( InputButton.Attack1 );
	}

	public override void AttackPrimary()
	{

	}
	
	public override void SimulateAnimator(PawnAnimator anim)
	{
		anim.SetParam("holdtype", 0); // TODO this is shit
		anim.SetParam("aimat_weight", 1.0f);
	}
}
