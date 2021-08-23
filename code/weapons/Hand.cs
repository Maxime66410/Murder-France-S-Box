using Sandbox;
using System;

[Library("weapon_hand", Title = "Hand", Spawnable = false)]
[Hammer.EditorModel("weapons/rust_boneknife/rust_boneknife.vmdl")]
partial class Hand : BaseDmWeapon
{
	public override string ViewModelPath => "";
	public override int ClipSize => -1;
	public override float PrimaryRate => 1.0f;
	public override float SecondaryRate => 0.5f;
	public override float ReloadTime => 4.0f;
	public override int Bucket => 0;

	public override void Spawn()
	{
		base.Spawn();
		
		SetModel("");
	}
	

	public override void AttackPrimary()
	{

	}

	public override void AttackSecondary()
	{
		
	}

	public override void SimulateAnimator(PawnAnimator anim)
	{
		anim.SetParam("holdtype", 4); // TODO this is shit
		anim.SetParam("aimat_weight", 1.0f);
	}
}
