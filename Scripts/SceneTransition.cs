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

        // [Export] public PackedScene TargetScene;
        [Export] public string TargetSceneFile;
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

        private bool _isReceiver;

        private void SetupTransition()
        {
            _transitionLogic = GetNode<SceneTransitionLogic>(TransitionLogicNodePath);
            if(_transitionLogic == null)
                throw new Exception("Transition logic is null!");

            _transitionLogic.Camera = _cameraNode;
            _transitionLogic.GameUi = _gameUiNode;
        }

        private void SetTransitionDisabledNodesProcess(bool process)
        {
            var transitionDisabledNodes = GetTree().GetNodesInGroup("TransitionDisabled");
            foreach(var node in transitionDisabledNodes)
                (node as Node)?.SetProcess(process);
        }

        private void SetTransitionHiddenNodes(bool show)
        {
            var transitionHiddenNodes = GetTree().GetNodesInGroup("TransitionHidden");
            foreach (var node in transitionHiddenNodes)
            {
                if (!(node is Control c))
                    continue;

                if (show)
                    c.Show();
                else
                    c.Hide();
            }
        }

        public void StartTransition()
        {
            SetupTransition();

            _transitionStart = DateTime.Now;
            _transitionState = _isReceiver ? TransitionStates.TransitionOut : TransitionStates.TransitionIn;
            _transitionLogic.Start();

            _transitionLogic.Update(_isReceiver ? 1.0f : 0.0f);

            SetTransitionDisabledNodesProcess(false);
            SetTransitionHiddenNodes(false);
        }

        public override void _Ready()
        {
            base._Ready();

            if(TargetSceneFile == null)
                throw new Exception("Target scene is null!");

            AddToGroup(TransitionGroupName);

            _gameUiNode = GetNode<Control>(GameUiNodePath);
            _cameraNode = GetNode<Node2D>(CameraPath);
            if(_transitionState == TransitionStates.Idle && _isReceiver)
                _gameUiNode.Hide();
        }

        private bool SetTransitionReceivers(Node n)
        {
            if (n is SceneTransition st)
            {
                st._isReceiver = true;
                return true;
            }

            foreach (var c in n.GetChildren())
            {
                if (c is Node node)
                {
                    if (SetTransitionReceivers(node))
                        return true;
                }
            }

            return false;
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
                        var newScene = ResourceLoader.Load<PackedScene>(TargetSceneFile);
                        if(newScene == null)
                            throw new Exception($"Can't load scene: {TargetSceneFile}");

                        var root = GetTree().Root;
                        foreach(var c in root.GetChildren())
                            (c as Node2D)?.QueueFree();

                        var instance = newScene.Instance();
                        SetTransitionReceivers(instance);
                        root.AddChild(instance);
                        foreach (var node in GetTree().GetNodesInGroup(TransitionGroupName))
                        {
                            var sceneTransition = node as SceneTransition;
                            if (sceneTransition == null || sceneTransition == this) continue;
                            sceneTransition.StartTransition();
                        }
                    }
                    else
                    {
                        if(StartGameAfterTransition)
                            this.GetManager().EmitSignal(nameof(GameManager.OnGameStart));

                        SetTransitionDisabledNodesProcess(true);
                        SetTransitionHiddenNodes(true);
                    }

                    _transitionState = TransitionStates.Idle;
                    _transitionLogic.End();
                    _isReceiver = false;
                }
            }
        }
    }
}