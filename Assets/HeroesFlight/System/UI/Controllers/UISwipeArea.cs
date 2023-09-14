using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UISwipeArea : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum SwipeDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    public Action<SwipeDirection> OnSwipe;

    [SerializeField] private float swipeDistanceThreshold = 50f;
    private Vector2 touchStartPos;
    private Vector2 touchEndPos;


    public void OnPointerDown(PointerEventData eventData)
    {
        touchStartPos = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        touchEndPos = eventData.position;
        CheckSwipe();
    }

    private void CheckSwipe()
    {
        float swipeDistance = Vector2.Distance(touchStartPos, touchEndPos);

        if (swipeDistance > swipeDistanceThreshold)
        {
            // Calculate the direction of the swipe
            Vector2 swipeDirection = touchEndPos - touchStartPos;

            // Check if it's a horizontal swipe
            if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
            {
                if (swipeDirection.x > 0)
                {
                    Debug.Log("Right swipe detected");
                    OnSwipe?.Invoke(SwipeDirection.Right);
                }
                else
                {
                    Debug.Log("Left swipe detected");
                    // Handle left swipe action here
                    OnSwipe?.Invoke(SwipeDirection.Left);
                }
            }
            else // It's a vertical swipe
            {
                if (swipeDirection.y > 0)
                {
                    Debug.Log("Up swipe detected");
                    // Handle up swipe action here
                    OnSwipe?.Invoke(SwipeDirection.Up);
                }
                else
                {
                    Debug.Log("Down swipe detected");
                    // Handle down swipe action here
                    OnSwipe?.Invoke(SwipeDirection.Down);
                }
            }
        }
    }
}
