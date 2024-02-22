using Godot;

namespace MRalozerca2.Scripts.SceneTransitions
{
    public class FadeAndMoveTransition : SceneTransitionLogic
    {
        [Export] public float TargetHeight = 1200;

        public float GetDownTarget() {
            const float defaultAspect = 9.0f / 16.0f;
            const float defaultHeight = 1000 * defaultAspect;

            float aspect = GetViewport().Size.y / GetViewport().Size.x;
            float height = 1000 * aspect;
            float diff = height - defaultHeight;

            if(diff > 0) {
                return diff;
            } else {
                return 0;
            }
        }

        public override void Update(float percentage)
        {
            var downLimit = GetDownTarget();

            var diffPos = new Vector2(0, (TargetHeight + downLimit) * percentage - downLimit);
            Camera.GlobalPosition = -diffPos;
            GameUi.RectPosition = new Vector2(0, diffPos.y + downLimit);

            GameUi.Modulate = new Color(1, 1, 1, 1.0f - percentage);
            GameUi.Show();
        }
    }
}