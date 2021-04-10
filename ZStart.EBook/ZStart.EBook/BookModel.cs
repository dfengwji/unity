using System.Collections.Generic;

namespace ZStart.EBook
{
    public class BookModel 
    {
        public class PageInfo
        {
            public string uid;
            public int page;
            public string title;
            public string image;
            public string audio;
            public string video;
            public string cover;
            public List<MenuInfo> menus = new List<MenuInfo>(10);

            public void AddMenu(MenuInfo info)
            {
                menus.Add(info);
            }
        }

        public struct MenuInfo
        {
            public string name;
            public int page;
        }

        public class PaperModel
        {
            public PageInfo left;
            public PageInfo right;
        }
        public string path;
        public string uid;
        public string frontCover;
        public string backCover;
        private List<PaperModel> pages = new List<PaperModel>(10);
        public List<PaperModel> AllPages
        {
            get
            {
                return pages;
            }
        }

        public BookModel()
        {

        }

        public void AddPage(PaperModel info)
        {
            pages.Add(info);
        }

        public void AddPage(PageInfo front, PageInfo back)
        {
            PaperModel info = new PaperModel
            {
                left = front,
                right = back
            };
            pages.Add(info);
        }
    }
}
