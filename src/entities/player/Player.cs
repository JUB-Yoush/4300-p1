using System;
using Godot;

// fps movement controller code adapted from https://github.com/majikayogames/SimpleFPSController
public partial class Player : CharacterBody3D
{
    float gravity = -12f;
    float LookSensitivity = 0.006f;
    private Vector3 _targetVelocity = Vector3.Zero;

    const float HEADBOB_APMPLITUDE = 0.06f;
    const float HEADBOB_FREQUENCY = 2.4f;
    float HeadbobTime = 0f;

    const float WALK_SPEED = 10f;
    const float GROUND_ACCEL = 11.0f;
    const float GROUND_DECEL = 7.0f;
    const float GROUND_FRICTION = 3.5f;

    const float JUMP_VEL = 6f;
    float AirCap = 0.85f;
    float AirAccel = 800;
    float AirSpeed = 500;

    Vector3 cameraTargetRotation = Vector3.Zero;
    Vector3 WishDir;
    Camera3D camera;

    public override void _Ready()
    {
        camera = GetNode<Camera3D>("Head/Camera3D");
    }

    public void HeadbobEffect(double delta)
    {
        HeadbobTime += (float)(delta * Velocity.Length());
        var newTransform = camera.Transform;
        newTransform.Origin = new Vector3(
            (float)(Mathf.Cos(HeadbobTime * HEADBOB_FREQUENCY * 0.5) * HEADBOB_APMPLITUDE),
            (float)(Math.Sin(HeadbobTime * HEADBOB_FREQUENCY * 0.5) * HEADBOB_APMPLITUDE),
            0
        );
        camera.Transform = newTransform;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
        if (@event is InputEventMouseButton)
        {
            Input.MouseMode = Input.MouseModeEnum.Captured;
        }
        else if (@event.IsActionPressed("ui_cancel"))
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }

        if (
            Input.MouseMode == Input.MouseModeEnum.Captured
            && @event is InputEventMouseMotion mouseMotionEvent
        )
        {
            RotateY(-mouseMotionEvent.Relative.X * LookSensitivity);
            camera.RotateX(-mouseMotionEvent.Relative.Y * LookSensitivity);

            camera.Rotation = camera.Rotation with
            {
                X = Mathf.Clamp(camera.Rotation.X, Mathf.DegToRad(-90), Mathf.DegToRad(90)),
            };
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        HeadbobEffect(delta);
        if (Input.IsActionJustPressed("jump") && IsOnFloor())
        {
            _targetVelocity.Y = JUMP_VEL;
        }

        Vector2 inputDir = Input.GetVector("left", "right", "up", "down");
        WishDir = GlobalTransform.Basis * new Vector3(inputDir.X, 0, inputDir.Y);
        WishDir = WishDir.Normalized();
        if (!IsOnFloor())
        {
            HandleAirPhyics(delta);
        }
        else
        {
            HandleGroundPhyics(delta);
        }
        Velocity = _targetVelocity;
        MoveAndSlide();
    }

    public void HandleAirPhyics(double delta)
    {
        _targetVelocity.Y += (float)(gravity * delta);
        var curSpeedinWishDir = Velocity.Dot(WishDir);
        var cappedSpeed = Math.Min((AirSpeed * WishDir).Length(), AirCap);
        var addSpeedTillCap = cappedSpeed - curSpeedinWishDir;
        if (addSpeedTillCap > 0)
        {
            var accelSpeed = AirAccel * AirSpeed * delta;
            accelSpeed = Math.Min(accelSpeed, addSpeedTillCap);
            _targetVelocity += Scale(WishDir, accelSpeed);
        }
    }

    public void HandleGroundPhyics(double delta)
    {
        var curSpeedinWishDir = Velocity.Dot(WishDir);
        var addSpeedTillCap = WALK_SPEED - curSpeedinWishDir;

        if (addSpeedTillCap > 0)
        {
            var accelSpeed = GROUND_ACCEL * delta * WALK_SPEED;
            accelSpeed = Math.Min(accelSpeed, addSpeedTillCap);
            _targetVelocity += Scale(WishDir, accelSpeed);
        }

        var control = Math.Max(_targetVelocity.Length(), GROUND_DECEL);
        var drop = control * GROUND_FRICTION * delta;
        var newSpeed = Math.Max(_targetVelocity.Length() - drop, 0);
        if (_targetVelocity.Length() > 0.0)
        {
            newSpeed /= _targetVelocity.Length();
        }
        _targetVelocity = Scale(_targetVelocity, newSpeed);
    }

    public Vector3 Scale(Vector3 v, double f) =>
        new((float)(v.X * f), (float)(v.Y * f), (float)(v.Z * f));
}
