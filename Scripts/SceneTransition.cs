using System;
using Godot;

namespace MRalozerca2.Scripts
{
    public abstract class SceneTransitionLogic : Node
    {
        public Control GameUi;
        public Node2D Camera;

        public virtual void Start() {}
        public virtual void Update(float percentage) {}
        public virtual void End() {}
    }

    public class SceneTransition : Node2D
    {
        public enum TransitionStates
        {
            Idle,
            TransitionIn, // Before scene change
            TransitionOut // After scene change
        }

        [Export] public NodePath GameUiNodePath;
        [Export] public NodePath CameraPath;

        [Export] public PackedScene TargetScene;
        [Export] public NodePath TransitionLogicNodePath;

        [Export] public float Duration = 1.0f;
        [Export] public bool StartGameAfterTransition = false;
        [Export] public float TransitionCurvePower = 1.5f;


        private const string TransitionGroupName = "TransitionGroup";

        private Control _gameUiNode;
        private Node2D _cameraNode;

        private SceneTransitionLogic _transitionLogic;
        private TransitionStates _transitionState = TransitionStates.Idle;

        private DateTime _transitionStart = DateTime.MinValue;

        public void StartTransition(bool receiver)
        {
            _transitionLogic = GetNode<SceneTransitionLogic>(TransitionLogicNodePath);
            if(_transitionLogic == null)
                throw new Exception("Transition logic is null!");

            _transitionLogic.Camera = _cameraNode;
            _transitionLogic.GameUi = _gameUiNode;
            _transitionStart = DateTime.Now;
            _transitionState = receiver ? TransitionStates.TransitionOut : TransitionStates.TransitionIn;
            _transitionLogic.Start();
        }

        public override void _Ready()
        {
            base._Ready();

            this.AddToGroup(TransitionGroupName);

            if(TargetScene == null)
                throw new Exception("Target scene is null!");

            _gameUiNode = GetNode<Control>(GameUiNodePath);
            _cameraNode = GetNode<Node2D>(CameraPath);
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            if (_transitionState != TransitionStates.Idle)
            {
                var diff = DateTime.Now - _transitionStart;
                var percentage = (float) diff.TotalSeconds / Duration;


                var percentageValue = Mathf.Clamp(percentage, 0.0f, 1.0f);
                if(_transitionState == TransitionStates.TransitionOut)
                    percentageValue = 1.0f - percentageValue;

                percentageValue = Mathf.Pow(percentageValue, TransitionCurvePower);

                _transitionLogic.Update(percentageValue);

                if(percentage >= 1.0f)
                {
                    if (_transitionState == TransitionStates.TransitionIn)
                    {
                        var newScene = TargetScene.Instance();
                        var root = GetTree().Root;
                        foreach(var c in root.GetChildren())
                            (c as Node2D)?.QueueFree();

                        root.AddChild(newScene);
                        foreach (var node in GetTree().GetNodesInGroup(TransitionGroupName))
                        {
                            var sceneTransition = node as SceneTransition;
                            if (sceneTransition == null || sceneTransition == this) continue;
                            sceneTransition.StartTransition(true);
                        }
                    }
                    else
                    {
                        if(StartGameAfterTransition)
                            this.GetManager().EmitSignal(nameof(GameManager.OnGameStart));
                    }

                    _transitionState = TransitionStates.Idle;
                    _transitionLogic.End();
                }
            }
        }
    }
}