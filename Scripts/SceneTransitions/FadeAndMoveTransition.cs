using Godot;

namespace MRalozerca2.Scripts.SceneTransitions
{
    public class FadeAndMoveTransition : SceneTransitionLogic
    {
        [Export] public float TargetHeight = 1200;

        public override void Update(float percentage)
        {
            var diffPos = new Vector2(0, TargetHeight) * percentage;
            Camera.GlobalPosition = -diffPos;
            GameUi.RectPosition = new Vector2(0, diffPos.y);

            GameUi.Modulate = new Color(1, 1, 1, 1.0f - percentage);
            GameUi.Show();
        }
    }
}