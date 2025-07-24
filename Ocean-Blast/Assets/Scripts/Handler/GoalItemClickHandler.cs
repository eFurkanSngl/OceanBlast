using UnityEngine;
using UnityEngine.EventSystems;


public class GoalItemClickHandler : MonoBehaviour, IPointerClickHandler
{
    private GoalBox _goalBox;

    public void Set(GoalBox goalBox)
    {
        _goalBox = goalBox;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_goalBox != null)
        {
            _goalBox.OnGoalItemClicked(this.gameObject);
        }
    }
}