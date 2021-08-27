using Sandbox;
using System;


public class MurderSpectate : Sandbox.Camera
{
	Angles LookThisAngles;

	Vector3 MovesInputs;
	Vector3 TargetsPos;

	Rotation TargetsRot;

	bool PivotEnabled;

	float MoveSpeeds;
	float FovOverrides = 0;
	float LerpsModes = 0;

	public override void Activated()
	{
		base.Activated();

		TargetsPos = CurrentView.Position;
		TargetsRot = CurrentView.Rotation;

		Pos = TargetsPos;
		Rot = TargetsRot;
		LookThisAngles = Rot.Angles();
		FovOverrides = 80;

		DoFPoint = 0.0f;
		DoFBlurSize = 0.0f;
	}

	public override void Deactivated()
	{
		base.Deactivated();
	}

	public override void Update()
	{
		var player = Local.Client;
		if (player == null) 
			return;

		var tr = Trace.Ray(Pos, Pos + Rot.Forward * 4096).UseHitboxes().Run();

		FieldOfView = FovOverrides;

		Viewer = null;
		{
			var lerpTarget = tr.EndPos.Distance(Pos);
			DoFPoint = lerpTarget;
		}

		FreeMove();
	}

	public override void BuildInput(InputBuilder input)
	{
		MovesInputs = input.AnalogMove;

		MoveSpeeds = 1;
		if (input.Down(InputButton.Run)) 
			MoveSpeeds = 5;

		LookThisAngles += input.AnalogLook * (FovOverrides / 80.0f);
		LookThisAngles.roll = 0;

		PivotEnabled = input.Down(InputButton.Walk);

		input.Clear();
		input.StopProcessing = true;
	}

	void FreeMove()
	{
		var mv = MovesInputs.Normal * 300 * RealTime.Delta * Rot * MoveSpeeds;

		TargetsRot = Rotation.From(LookThisAngles);
		TargetsPos += mv;

		Pos = Vector3.Lerp(Pos, TargetsPos, 10 * RealTime.Delta * (1 - LerpsModes));
		Rot = Rotation.Slerp(Rot, TargetsRot, 10 * RealTime.Delta * (1 - LerpsModes));
	}
}
