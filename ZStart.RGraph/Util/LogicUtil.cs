using System.Text.RegularExpressions;
using UnityEngine;

namespace ZStart.RGraph.Util
{
    public static class LogicUtil
    {
        public static bool IsTouchPhase(TouchPhase phase)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == phase)
            {
                return true;
            }
            if (Input.GetMouseButtonDown(0) && phase == TouchPhase.Began)
            {
                return true;
            }
            if (Input.GetMouseButtonUp(0) && phase == TouchPhase.Ended)
            {
                return true;
            }
            if (Input.GetMouseButton(0) && (phase == TouchPhase.Moved || phase == TouchPhase.Stationary))
            {
                return true;
            }
            return false;
        }

        public static bool IsChinaChar(string character)
        {
            Regex regex = new Regex("^[\u4e00-\u9fa5]{0,}$");
            if (regex.Match(character).Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsLetterOrNumber(string character)
        {
            Regex regex = new Regex("^[A-Za-z0-9]+$");
            if (regex.Match(character).Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
