﻿using Sandbox;
using System;

[Library("weapon_hand", Title = "Hand", Spawnable = false)]
[Hammer.EditorModel("weapons/rust_boneknife/rust_boneknife.vmdl")]
partial class ZombieHand : BaseDmWeapon
{
	public override string ViewModelPath => "";
	public override int ClipSize => -1;
	public override float PrimaryRate => 1.0f;
	public override float SecondaryRate => 0.5f;
	public override float ReloadTime => 4.0f;
	public override int Bucket => 0;
	public virtual int BaseDamage => 35;
	public virtual int MeleeDistance => 80;

	public override void Spawn()
	{
		base.Spawn();
		
		SetModel("");
	}

	public virtual void MeleeStrike(float damage, float force)
	{
		var forward = Owner.EyeRot.Forward;
		forward = forward.Normal;

		foreach (var tr in TraceBullet(Owner.EyePos, Owner.EyePos + forward * MeleeDistance, 10f))
		{
			if (!tr.Entity.IsValid()) continue;

			tr.Surface.DoBulletImpact(tr);

			if (!IsServer) continue;

			using (Prediction.Off())
			{
				var damageInfo = DamageInfo.FromBullet(tr.EndPos, forward * 100 * force, damage)
					.UsingTraceResult(tr)
					.WithAttacker(Owner)
					.WithWeapon(this);

				tr.Entity.TakeDamage(damageInfo);
			}
		}
	}

	public override void AttackPrimary()
	{
		//if (!CanPrimaryAttack()) return;

		
		MeleeStrike(BaseDamage, 1.5f);
		
		(Owner as AnimEntity).SetAnimBool("b_attack", true);
		ShootEffects();
	}

	public override void AttackSecondary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		(Owner as AnimEntity).SetAnimBool("b_attack", true);

		ShootEffects();
		MeleeStrike(BaseDamage * 1.5f, 1.5f);
	}

	[ClientRpc]
	protected override void ShootEffects()
	{
		Host.AssertClient();

		ViewModelEntity?.SetAnimBool("fire", true);
		CrosshairPanel?.CreateEvent("fire");
	}

	public override void SimulateAnimator(PawnAnimator anim)
	{
		anim.SetParam("holdtype", 4); // TODO this is shit
		anim.SetParam("aimat_weight", 1.0f);
	}
}