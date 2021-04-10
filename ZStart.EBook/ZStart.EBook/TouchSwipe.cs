using UnityEngine;
using UnityEngine.Events;

namespace ZStart.EBook
{
    public class TouchSwipe
    {
        public enum Direction
        {
            None,
            Left,
            Right,
            Top,
            Down
        }
        public float distance = 10;
        private bool touchMove = false;
        private Vector2 finalPos, startPos, endPos, oldPos;
        private float length, startTime;
        private UnityAction<Direction, bool> callFun;

        public TouchSwipe(UnityAction<Direction, bool> action)
        {
            callFun = action;
        }

        public void CheckTouchSwipe()
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    startTime = Time.time;
                    finalPos = Vector3.zero;
                    length = 0;
                    touchMove = false;
                    startPos = Input.GetTouch(0).position;
                    oldPos = startPos;
                    if (callFun != null)
                        callFun.Invoke(Direction.None, true);
                }

                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    touchMove = true;

                    oldPos = Input.GetTouch(0).position;
                }

                if (Input.GetTouch(0).phase == TouchPhase.Canceled)
                {
                    touchMove = false;
                }

                if (Input.GetTouch(0).phase == TouchPhase.Stationary)
                {
                    touchMove = false;
                }

                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    if (touchMove)
                    {
                        endPos = Input.GetTouch(0).position;
                        finalPos = endPos - startPos;
                        //length = finalPosition.x < 0 ? -(finalPosition.magnitude * Time.deltaTime) : (finalPosition.magnitude * Time.deltaTime);
                        //length *= .35f;

                        //var force = length / (Time.time - startTime);
                        //force = Mathf.Clamp(force, -maxForce, maxForce);
                        var dir = CheckDirection(endPos, startPos);
                        if (callFun != null)
                            callFun.Invoke(dir, false);
                    }
                }
            }
        }

        public void CheckMouseSwipe()
        {
            if (Input.GetMouseButtonDown(0))
            {
                startTime = Time.time;
                finalPos = Vector3.zero;
                length = 0;
                startPos = Input.mousePosition;
                if (callFun != null)
                    callFun.Invoke(Direction.None, true);
            }

            if (Input.GetMouseButtonUp(0))
            {
                endPos = Input.mousePosition;
                finalPos = endPos - startPos;
                //length = finalPosition.x < 0 ? (finalPosition.magnitude * Time.deltaTime) : -(finalPosition.magnitude * Time.deltaTime);
                //length *= .5f;

                //force = length / (Time.time - startTime);
                //force = Mathf.Clamp(force, -maxForce, maxForce);
                var dir = CheckDirection(endPos, startPos);
                if (callFun != null)
                    callFun.Invoke(dir, false);
            }
        }

        private Direction CheckDirection(Vector2 start, Vector2 end)
        {
            var dis = Vector2.Distance(start, end);
            if (dis < distance)
                return Direction.None;
            var dir = Direction.None;
            var hdis = endPos.x - startPos.x;
            var vdis = endPos.y - startPos.y;
            if (Mathf.Abs(hdis) >= Mathf.Abs(vdis))
            {
                if (hdis > 0)
                {
                    dir = Direction.Left;
                }
                else
                {
                    dir = Direction.Right;
                }
            }
            else
            {
                if (vdis > 0)
                {
                    dir = Direction.Top;
                }
                else
                {
                    dir = Direction.Down;
                }
            }
            return dir;
        }
    }
}
