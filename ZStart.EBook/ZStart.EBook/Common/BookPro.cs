using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZStart.EBook.View.Parts;

namespace ZStart.EBook.Common
{
    public enum FlipMode
    {
        RightToLeft,
        LeftToRight
    }

    public class BookPro : MonoBehaviour
    {
        Canvas canvas;
        public RectTransform target;
        public Image clipImage;
        public Image shadowImage;
        public Image shadowLImage;
        public Image shadowRImage;
        public Image shadowLTR;
        public RectTransform LeftPage;
        public RectTransform RightPage;
        public EventTrigger leftHot;
        public EventTrigger rightHot;
        public bool interactable = true;
        public bool enableShadowEffect = true;
        [Tooltip("Uncheck this if the book does not contain transparent pages to improve the overall performance")]
        public bool hasTransparentPages = true;
        [HideInInspector]
        public int currentPaper = 0;
        [HideInInspector]
        public List<Paper> papers = new List<Paper>(50);
        /// <summary>
        /// OnFlip invocation list, called when any page flipped
        /// </summary>
        public UnityEvent OnFlip;

        /// <summary>
        /// The Current Shown paper (the paper its front shown in right part)
        /// </summary>
        public int CurrentPaper
        {
            get { return currentPaper; }
            set
            {
                if (value != currentPaper)
                {
                    if (value < StartFlippingPaper)
                        currentPaper = StartFlippingPaper;
                    else if (value > EndFlippingPaper + 1)
                        currentPaper = EndFlippingPaper + 1;
                    else
                        currentPaper = value;
                    UpdatePages();
                }
            }
        }
        [HideInInspector]
        public int StartFlippingPaper = 0;
        [HideInInspector]
        public int EndFlippingPaper = 1;

        public Vector3 EndBottomLeft
        {
            get { return ebl; }
        }
        public Vector3 EndBottomRight
        {
            get { return ebr; }
        }
        public float Height
        {
            get
            {
                return target.rect.height;
            }
        }

        MaskableGraphic Left;
        MaskableGraphic Right;

        //current flip mode
        FlipMode mode;

        /// <summary>
        /// this value should e true while the user darg the page
        /// </summary>
        bool pageDragging = false;

        /// <summary>
        /// should be true when the page tween forward or backward after release
        /// </summary>
        bool tweening = false;

        // Use this for initialization
        void Start()
        {
            Init();
            Canvas[] c = GetComponentsInParent<Canvas>();
            if (c.Length > 0)
                canvas = c[c.Length - 1];
            else
                Debug.LogError("Book Must be a child to canvas diectly or indirectly");

            UpdatePages();

            CalcCurlCriticalPoints();


            float pageWidth = target.rect.width / 2.0f;
            float pageHeight = target.rect.height;


            clipImage.rectTransform.sizeDelta = new Vector2(pageWidth * 2 + pageHeight, pageHeight + pageHeight * 2);

            //hypotenous (diagonal) page length
            float hyp = Mathf.Sqrt(pageWidth * pageWidth + pageHeight * pageHeight);
            float shadowPageHeight = pageWidth / 2 + hyp;

            shadowImage.rectTransform.sizeDelta = new Vector2(pageWidth, shadowPageHeight);
            shadowImage.rectTransform.pivot = new Vector2(1, (pageWidth / 2) / shadowPageHeight);

            shadowLTR.rectTransform.sizeDelta = new Vector2(pageWidth, shadowPageHeight);
            shadowLTR.rectTransform.pivot = new Vector2(0, (pageWidth / 2) / shadowPageHeight);

            shadowRImage.rectTransform.sizeDelta = new Vector2(pageWidth, shadowPageHeight);
            shadowRImage.rectTransform.pivot = new Vector2(0, (pageWidth / 2) / shadowPageHeight);

            shadowLImage.rectTransform.sizeDelta = new Vector2(pageWidth, shadowPageHeight);
            shadowLImage.rectTransform.pivot = new Vector2(1, (pageWidth / 2) / shadowPageHeight);
        }

        private void Init()
        {
            if (target != null)
                return;
            target = GetComponent<RectTransform>();
            clipImage = target.Find("ClipImage").GetComponent<Image>();
            shadowImage = target.Find("ShadowImage").GetComponent<Image>();
            shadowLImage = target.Find("ShadowLeftImage").GetComponent<Image>();
            shadowLTR = target.Find("ShadowLTR").GetComponent<Image>();
            shadowRImage = target.Find("ShadowRightImage").GetComponent<Image>();
            LeftPage = target.Find("LeftPage").GetComponent<RectTransform>();
            RightPage = target.Find("RightPage").GetComponent<RectTransform>();
            leftHot = target.Find("LeftHot").GetComponent<EventTrigger>();
            rightHot = target.Find("rightHot").GetComponent<EventTrigger>();
            for (int i = 0;i < leftHot.triggers.Count;i += 1)
            {
                if(leftHot.triggers[i].eventID == EventTriggerType.EndDrag)
                    leftHot.triggers[i].callback.AddListener(OnMouseRelease);
                else if (leftHot.triggers[i].eventID == EventTriggerType.BeginDrag)
                {
                    leftHot.triggers[i].callback.AddListener(OnMouseDragLeftPage);
                }
            }
            for (int i = 0; i < rightHot.triggers.Count; i += 1)
            {
                if (rightHot.triggers[i].eventID == EventTriggerType.EndDrag)
                    rightHot.triggers[i].callback.AddListener(OnMouseRelease);
                else if (rightHot.triggers[i].eventID == EventTriggerType.BeginDrag)
                {
                    rightHot.triggers[i].callback.AddListener(OnMouseDragRightPage);
                }
            }
        }

        /// <summary>
        /// transform point from global (world-space) to local space
        /// </summary>
        /// <param name="global">poit iin world space</param>
        /// <returns></returns>
        public Vector3 transformPoint(Vector3 global)
        {
            Vector2 localPos = target.InverseTransformPoint(global);
            return localPos;
        }
        /// <summary>
        /// transform mouse position to local space
        /// </summary>
        /// <param name="mouseScreenPos"></param>
        /// <returns></returns>
        public Vector3 transformPointMousePosition(Vector3 mouseScreenPos)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                Vector3 mouseWorldPos = canvas.worldCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, canvas.planeDistance));
                Vector2 localPos = target.InverseTransformPoint(mouseWorldPos);

                return localPos;
            }
            else if (canvas.renderMode == RenderMode.WorldSpace)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 globalEBR = transform.TransformPoint(ebr);
                Vector3 globalEBL = transform.TransformPoint(ebl);
                Vector3 globalSt = transform.TransformPoint(st);
                Plane p = new Plane(globalEBR, globalEBL, globalSt);
                float distance;
                p.Raycast(ray, out distance);
                Vector2 localPos = target.InverseTransformPoint(ray.GetPoint(distance));
                return localPos;
            }
            else
            {
                //Screen Space Overlay
                Vector2 localPos = target.InverseTransformPoint(mouseScreenPos);
                return localPos;
            }

        }

        /// <summary>
        /// Update page orders
        /// This function should be called whenever the current page changed, the dragging of the page started or the page has been flipped
        /// </summary>
        public void UpdatePages()
        {
            int previousPaper = pageDragging ? currentPaper - 2 : currentPaper - 1;

            //Hide all pages
            for (int i = 0; i < papers.Count; i++)
            {
                BookUtility.HidePage(papers[i].Front);
                papers[i].Front.transform.SetParent(target.transform);
                BookUtility.HidePage(papers[i].Back);
                papers[i].Back.transform.SetParent(target.transform);
            }

            if (hasTransparentPages)
            {
                //Show the back page of all previous papers
                for (int i = 0; i <= previousPaper; i++)
                {
                    BookUtility.ShowPage(papers[i].Back);
                    papers[i].Back.transform.SetParent(target.transform);
                    papers[i].Back.transform.SetSiblingIndex(i);
                    BookUtility.CopyTransform(LeftPage.transform, papers[i].Back.transform);
                }

                //Show the front page of all next papers
                for (int i = papers.Count - 1; i >= currentPaper; i--)
                {
                    BookUtility.ShowPage(papers[i].Front);
                    papers[i].Front.transform.SetSiblingIndex(papers.Count - i + previousPaper);
                    BookUtility.CopyTransform(RightPage.transform, papers[i].Front.transform);
                }

            }
            else
            {
                //show back of previous page only
                if (previousPaper >= 0)
                {
                    BookUtility.ShowPage(papers[previousPaper].Back);
                    //papers[previousPaper].Back.transform.SetParent(BookPanel.transform);
                    //papers[previousPaper].Back.transform.SetSiblingIndex(previousPaper);
                    BookUtility.CopyTransform(LeftPage.transform, papers[previousPaper].Back.transform);
                }
                //show front of current page only
                if (currentPaper <= papers.Count - 1)
                {
                    BookUtility.ShowPage(papers[currentPaper].Front);
                    papers[currentPaper].Front.transform.SetSiblingIndex(papers.Count - currentPaper + previousPaper);
                    BookUtility.CopyTransform(RightPage.transform, papers[currentPaper].Front.transform);

                }
            }
            #region Shadow Effect
            if (enableShadowEffect)
            {
                //the shadow effect enabled
                if (previousPaper >= 0)
                {
                    //has at least one previous page, then left shadow should be active
                    shadowLImage.gameObject.SetActive(true);
                    shadowLImage.transform.SetParent(papers[previousPaper].Back.transform, true);
                    shadowLImage.rectTransform.anchoredPosition = new Vector3();
                    shadowLImage.rectTransform.localRotation = Quaternion.identity;
                }
                else
                {
                    //if no previous pages, the leftShaow should be disabled
                    shadowLImage.gameObject.SetActive(false);
                    shadowLImage.transform.SetParent(target, true);
                }

                if (currentPaper < papers.Count)
                {
                    //has at least one next page, the right shadow should be active
                    shadowRImage.gameObject.SetActive(true);
                    shadowRImage.transform.SetParent(papers[currentPaper].Front.transform, true);
                    shadowRImage.rectTransform.anchoredPosition = new Vector3();
                    shadowRImage.rectTransform.localRotation = Quaternion.identity;
                }
                else
                {
                    //no next page, the right shadow should be diabled
                    shadowRImage.gameObject.SetActive(false);
                    shadowRImage.transform.SetParent(target, true);
                }
            }
            else
            {
                //Enable Shadow Effect is Unchecked, all shadow effects should be disabled
                shadowLImage.gameObject.SetActive(false);
                shadowLImage.transform.SetParent(target, true);

                shadowRImage.gameObject.SetActive(false);
                shadowRImage.transform.SetParent(target, true);

            }
            #endregion
        }


        //mouse interaction events call back
        public void OnMouseDragRightPage(BaseEventData data)
        {
            if (interactable && !tweening)
            {

                DragRightPageToPoint(transformPointMousePosition(Input.mousePosition));
            }

        }
        public void DragRightPageToPoint(Vector3 point)
        {
            if (currentPaper > EndFlippingPaper) return;
            pageDragging = true;
            mode = FlipMode.RightToLeft;
            f = point;

            clipImage.rectTransform.pivot = new Vector2(1, 0.35f);
            currentPaper += 1;

            UpdatePages();

            Left = papers[currentPaper - 1].Front.GetComponent<MaskableGraphic>();
            BookUtility.ShowPage(Left.gameObject);
            Left.rectTransform.pivot = new Vector2(0, 0);
            Left.transform.position = RightPage.transform.position;
            Left.transform.localEulerAngles = new Vector3(0, 0, 0);

            Right = papers[currentPaper - 1].Back.GetComponent<MaskableGraphic>();
            BookUtility.ShowPage(Right.gameObject);
            Right.transform.position = RightPage.transform.position;
            Right.transform.localEulerAngles = new Vector3(0, 0, 0);

            if (enableShadowEffect) shadowImage.gameObject.SetActive(true);
            clipImage.gameObject.SetActive(true);

            UpdateBookRTLToPoint(f);
        }
        public void OnMouseDragLeftPage(BaseEventData data)
        {
            if (interactable && !tweening)
            {
                DragLeftPageToPoint(transformPointMousePosition(Input.mousePosition));
            }

        }
        public void DragLeftPageToPoint(Vector3 point)
        {
            if (currentPaper <= StartFlippingPaper) return;
            pageDragging = true;
            mode = FlipMode.LeftToRight;
            f = point;

            UpdatePages();

            clipImage.rectTransform.pivot = new Vector2(0, 0.35f);

            Right = papers[currentPaper - 1].Back.GetComponent<MaskableGraphic>();
            BookUtility.ShowPage(Right.gameObject);
            Right.transform.position = LeftPage.transform.position;
            Right.transform.localEulerAngles = new Vector3(0, 0, 0);
            Right.transform.SetAsFirstSibling();

            Left = papers[currentPaper - 1].Front.GetComponent<MaskableGraphic>();
            BookUtility.ShowPage(Left.gameObject);
            Left.gameObject.SetActive(true);
            Left.rectTransform.pivot = new Vector2(1, 0);
            Left.transform.position = LeftPage.transform.position;
            Left.transform.localEulerAngles = new Vector3(0, 0, 0);


            if (enableShadowEffect) shadowLTR.gameObject.SetActive(true);
            clipImage.gameObject.SetActive(true);
            UpdateBookLTRToPoint(f);
        }
        public void OnMouseRelease(BaseEventData data)
        {
            if (interactable)
                ReleasePage();
        }
        public void ReleasePage()
        {
            if (pageDragging)
            {
                pageDragging = false;
                float distanceToLeft = Vector2.Distance(c, ebl);
                float distanceToRight = Vector2.Distance(c, ebr);
                if (distanceToRight < distanceToLeft && mode == FlipMode.RightToLeft)
                    TweenBack();
                else if (distanceToRight > distanceToLeft && mode == FlipMode.LeftToRight)
                    TweenBack();
                else
                    TweenForward();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (pageDragging && interactable)
            {
                UpdateBook();
            }
        }
        public void UpdateBook()
        {
            f = Vector3.Lerp(f, transformPointMousePosition(Input.mousePosition), Time.deltaTime * 10);
            if (mode == FlipMode.RightToLeft)
                UpdateBookRTLToPoint(f);
            else
                UpdateBookLTRToPoint(f);
        }

        /// <summary>
        /// This function called when the page dragging point reached its distenation after releasing the mouse
        /// This function will call the OnFlip invocation list
        /// if you need to call any fnction after the page flipped just add it to the OnFlip invocation list
        /// </summary>
        public void Flip()
        {
            pageDragging = false;

            if (mode == FlipMode.LeftToRight)
                currentPaper -= 1;
            //Debug.Log(currentPaper);
            Left.transform.SetParent(target.transform, true);
            Left.rectTransform.pivot = new Vector2(0, 0);
            Right.transform.SetParent(target.transform, true);
            UpdatePages();
            shadowImage.gameObject.SetActive(false);
            shadowLTR.gameObject.SetActive(false);
            clipImage.gameObject.SetActive(false);
            if (OnFlip != null)
                OnFlip.Invoke();
        }

        public void AddPaper(GameObject front, GameObject back)
        {
            Paper paper = new Paper();
            paper.Front = front;
            paper.Back = back;
            papers.Add(paper);
        }

        public void ClearPapers()
        {
            for (int i = 0; i < papers.Count; i += 1)
            {
                papers[i].Front.SetActive(false);
                papers[i].Back.SetActive(false);
            }
            papers.Clear();
        }

        public void TweenForward()
        {
            if (mode == FlipMode.RightToLeft)
            {
                tweening = true;
                Tween.ValueTo(gameObject, f, ebl * 0.98f, 0.3f, TweenUpdate, () =>
                {
                    Flip();
                    tweening = false;
                });
            }
            else
            {
                tweening = true;
                Tween.ValueTo(gameObject, f, ebr * 0.98f, 0.3f, TweenUpdate, () =>
                {
                    Flip();
                    tweening = false;
                });
            }
        }
        void TweenUpdate(Vector3 follow)
        {
            if (mode == FlipMode.RightToLeft)
                UpdateBookRTLToPoint(follow);
            else
                UpdateBookLTRToPoint(follow);
        }

        public void TweenBack()
        {
            if (mode == FlipMode.RightToLeft)
            {
                tweening = true;
                Tween.ValueTo(gameObject, f, ebr * 0.98f, 0.3f, TweenUpdate, () =>
                {
                    currentPaper -= 1;
                    Right.transform.SetParent(target.transform);
                    Left.transform.SetParent(target.transform);
                    //pageDragging = false;
                    tweening = false;
                    shadowImage.gameObject.SetActive(false);
                    shadowLTR.gameObject.SetActive(false);
                    UpdatePages();
                });
            }
            else
            {
                tweening = true;
                Tween.ValueTo(gameObject, f, ebl * 0.98f, 0.3f, TweenUpdate, () =>
                {
                    Left.transform.SetParent(target.transform);
                    Right.transform.SetParent(target.transform);
                    //pageDragging = false;
                    tweening = false;
                    shadowImage.gameObject.SetActive(false);
                    shadowLTR.gameObject.SetActive(false);
                    UpdatePages();
                });
            }
        }

        #region Page Curl Internal Calculations
        //for more info about this part please check this link : http://rbarraza.com/html5-canvas-pageflip/

        float radius1, radius2;
        //Spine Bottom
        Vector3 sb;
        //Spine Top
        Vector3 st;
        //corner of the page
        Vector3 c;
        //Edge Bottom Right
        Vector3 ebr;
        //Edge Bottom Left
        Vector3 ebl;
        //follow point 
        Vector3 f;

        private void CalcCurlCriticalPoints()
        {
            sb = new Vector3(0, -target.rect.height / 2);
            ebr = new Vector3(target.rect.width / 2, -target.rect.height / 2);
            ebl = new Vector3(-target.rect.width / 2, -target.rect.height / 2);
            st = new Vector3(0, target.rect.height / 2);
            radius1 = Vector2.Distance(sb, ebr);
            float pageWidth = target.rect.width / 2.0f;
            float pageHeight = target.rect.height;
            radius2 = Mathf.Sqrt(pageWidth * pageWidth + pageHeight * pageHeight);
        }
        public void UpdateBookRTLToPoint(Vector3 followLocation)
        {
            mode = FlipMode.RightToLeft;
            f = followLocation;
            if (enableShadowEffect)
            {
                shadowImage.transform.SetParent(clipImage.transform, true);
                shadowImage.transform.localPosition = new Vector3(0, 0, 0);
                shadowImage.transform.localEulerAngles = new Vector3(0, 0, 0);

                shadowLTR.transform.SetParent(Left.transform);
                shadowLTR.rectTransform.anchoredPosition = new Vector3();
                shadowLTR.transform.localEulerAngles = Vector3.zero;
                shadowLTR.gameObject.SetActive(true);
            }
            Right.transform.SetParent(clipImage.transform, true);

            Left.transform.SetParent(target.transform, true);
            c = Calc_C_Position(followLocation);
            Vector3 t1;
            float T0_T1_Angle = Calc_T0_T1_Angle(c, ebr, out t1);
            if (T0_T1_Angle >= -90) T0_T1_Angle -= 180;

            clipImage.rectTransform.pivot = new Vector2(1, 0.35f);
            clipImage.transform.localEulerAngles = new Vector3(0, 0, T0_T1_Angle + 90);
            clipImage.transform.position = target.TransformPoint(t1);


            shadowRImage.transform.localEulerAngles = new Vector3(0, 0, T0_T1_Angle + 90);
            shadowRImage.transform.position = target.TransformPoint(t1);

            //page position and angle
            Right.transform.position = target.TransformPoint(c);
            float C_T1_dy = t1.y - c.y;
            float C_T1_dx = t1.x - c.x;
            float C_T1_Angle = Mathf.Atan2(C_T1_dy, C_T1_dx) * Mathf.Rad2Deg;
            Right.transform.localEulerAngles = new Vector3(0, 0, C_T1_Angle - (T0_T1_Angle + 90));

            Left.transform.SetParent(clipImage.transform, true);
            Left.transform.SetAsFirstSibling();

            shadowImage.rectTransform.SetParent(Right.rectTransform, true);
        }
        public void UpdateBookLTRToPoint(Vector3 followLocation)
        {
            mode = FlipMode.LeftToRight;
            f = followLocation;
            if (enableShadowEffect)
            {
                shadowLTR.transform.SetParent(clipImage.transform, true);
                shadowLTR.transform.localPosition = new Vector3(0, 0, 0);
                shadowLTR.transform.localEulerAngles = new Vector3(0, 0, 0);

                shadowImage.transform.SetParent(Right.transform);
                shadowImage.rectTransform.anchoredPosition = new Vector3(0, 0, 0);
                shadowImage.transform.localEulerAngles = Vector3.zero;
                shadowImage.gameObject.SetActive(true);
            }
            Left.transform.SetParent(clipImage.transform, true);
            Right.transform.SetParent(target.transform, true);

            c = Calc_C_Position(followLocation);
            Vector3 t1;
            float T0_T1_Angle = Calc_T0_T1_Angle(c, ebl, out t1);
            if (T0_T1_Angle < 0) T0_T1_Angle += 180;

            clipImage.transform.localEulerAngles = new Vector3(0, 0, T0_T1_Angle - 90);
            clipImage.transform.position = target.TransformPoint(t1);

            shadowLImage.transform.localEulerAngles = new Vector3(0, 0, T0_T1_Angle - 90);
            shadowLImage.transform.position = target.TransformPoint(t1);

            //page position and angle
            Left.transform.position = target.TransformPoint(c);
            float C_T1_dy = t1.y - c.y;
            float C_T1_dx = t1.x - c.x;
            float C_T1_Angle = Mathf.Atan2(C_T1_dy, C_T1_dx) * Mathf.Rad2Deg;
            Left.transform.localEulerAngles = new Vector3(0, 0, C_T1_Angle - 180 - (T0_T1_Angle - 90));

            Right.transform.SetParent(clipImage.transform, true);
            Right.transform.SetAsFirstSibling();

            shadowLTR.rectTransform.SetParent(Left.rectTransform, true);
        }
        private float Calc_T0_T1_Angle(Vector3 c, Vector3 bookCorner, out Vector3 t1)
        {
            Vector3 t0 = (c + bookCorner) / 2;
            float T0_CORNER_dy = bookCorner.y - t0.y;
            float T0_CORNER_dx = bookCorner.x - t0.x;
            float T0_CORNER_Angle = Mathf.Atan2(T0_CORNER_dy, T0_CORNER_dx);
            float T0_T1_Angle = 90 - T0_CORNER_Angle;

            float T1_X = t0.x - T0_CORNER_dy * Mathf.Tan(T0_CORNER_Angle);
            T1_X = normalizeT1X(T1_X, bookCorner, sb);
            t1 = new Vector3(T1_X, sb.y, 0);
            ////////////////////////////////////////////////
            //clipping plane angle=T0_T1_Angle
            float T0_T1_dy = t1.y - t0.y;
            float T0_T1_dx = t1.x - t0.x;
            T0_T1_Angle = Mathf.Atan2(T0_T1_dy, T0_T1_dx) * Mathf.Rad2Deg;
            return T0_T1_Angle;
        }
        private float normalizeT1X(float t1, Vector3 corner, Vector3 sb)
        {
            if (t1 > sb.x && sb.x > corner.x)
                return sb.x;
            if (t1 < sb.x && sb.x < corner.x)
                return sb.x;
            return t1;
        }
        private Vector3 Calc_C_Position(Vector3 followLocation)
        {
            Vector3 c;
            f = followLocation;
            float F_SB_dy = f.y - sb.y;
            float F_SB_dx = f.x - sb.x;
            float F_SB_Angle = Mathf.Atan2(F_SB_dy, F_SB_dx);
            Vector3 r1 = new Vector3(radius1 * Mathf.Cos(F_SB_Angle), radius1 * Mathf.Sin(F_SB_Angle), 0) + sb;

            float F_SB_distance = Vector2.Distance(f, sb);
            if (F_SB_distance < radius1)
                c = f;
            else
                c = r1;
            float F_ST_dy = c.y - st.y;
            float F_ST_dx = c.x - st.x;
            float F_ST_Angle = Mathf.Atan2(F_ST_dy, F_ST_dx);
            Vector3 r2 = new Vector3(radius2 * Mathf.Cos(F_ST_Angle),
               radius2 * Mathf.Sin(F_ST_Angle), 0) + st;
            float C_ST_distance = Vector2.Distance(c, st);
            if (C_ST_distance > radius2)
                c = r2;
            return c;
        }
        #endregion

    }
    [Serializable]
    public class Paper
    {
        public GameObject Front;
        public GameObject Back;
    }


    public static class BookUtility
    {
        /// <summary>
        /// Call this function to Show a Hidden Page
        /// </summary>
        /// <param name="page">the page to be shown</param>
        public static void ShowPage(GameObject page)
        {
            CanvasGroup cgf = page.GetComponent<CanvasGroup>();
            cgf.alpha = 1;
            cgf.blocksRaycasts = true;
        }

        /// <summary>
        /// Call this function to hide any page
        /// </summary>
        /// <param name="page">the page to be hidden</param>
        public static void HidePage(GameObject page)
        {
            CanvasGroup cgf = page.GetComponent<CanvasGroup>();
            cgf.alpha = 0;
            cgf.blocksRaycasts = false;
            page.transform.SetAsFirstSibling();
        }

        public static void CopyTransform(Transform from, Transform to)
        {
            to.position = from.position;
            to.rotation = from.rotation;
            to.localScale = from.localScale;

        }
    }
}
