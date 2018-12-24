using System.Collections.Generic;

namespace ZStart.Common.Helper
{
    public class PagingHelper<T>
    {
        public int countPerPage = 12;

        private int _maxPage = 0;
        public int maxPage
        {
            get { return _maxPage; }
        }

        private int _currentPage = 0;
        public int currentPage
        {
            get { return _currentPage; }
        }

        private int _totalCount = 0;
        public int totalCount
        {
            get { return _totalCount; }
        }
        public List<T> itemList;

        public bool hasNextPage
        {
            get
            {
                if (_currentPage < _maxPage)
                    return true;
                else
                    return false;
            }
        }

        public bool hasPrePage
        {
            get
            {
                if (_currentPage > 1)
                    return true;
                else
                    return false;
            }
        }
        public PagingHelper()
        {
            itemList = new List<T>();
        }

        public void UpdateAllItems(List<T> list, int num)
        {
            //Debug.LogWarning("Change the Props: " + list.Length +" at the time " + Time.time);
            countPerPage = num;
            itemList.Clear();
            itemList.AddRange(list);
            _totalCount = itemList.Count;
            _currentPage = 1;
            if (countPerPage < 1)
                countPerPage = 12;
            if (_totalCount % countPerPage == 0 && _totalCount > 0)
                _maxPage = _totalCount / countPerPage;
            else
                _maxPage = _totalCount / countPerPage + 1;
        }

        public void RemoveItem(T info)
        {
            if (info == null)
                return;
            for (int i = 0; i < itemList.Count; i++)
            {
                if (itemList[i].Equals(info))
                {
                    itemList.RemoveAt(i);
                    break;
                }
            }
        }

        public void SetPage(int page)
        {
            if (page < 0)
                _currentPage = 1;
            else if (page > _maxPage)
                _currentPage = maxPage;
            else
                _currentPage = page;
        }

        public T[] GetItemsByPage(int page)
        {
            if (page < 1 || page > _maxPage)
                return null;
            List<T> list = new List<T>();
            _currentPage = page;
            if (_totalCount <= countPerPage)
                return itemList.ToArray();
            int index = (_currentPage - 1) * countPerPage;
            int leng = countPerPage;
            int number = _totalCount - index;
            if (_currentPage == maxPage && number < leng)
                leng = number;
            for (int i = 0; i < leng; i++)
            {
                list.Add(itemList[i + index]);
            }
            return list.ToArray();
        }

        public T[] CurrentPage()
        {
            if (_currentPage < _maxPage)
            {
                return GetItemsByPage(_currentPage);
            }
            else
            {
                return GetItemsByPage(_currentPage);
            }
        }

        public T[] NextPage()
        {
            if (_currentPage < _maxPage)
            {
                _currentPage += 1;
                return GetItemsByPage(_currentPage);
            }
            else
            {
                return GetItemsByPage(_currentPage);
            }
        }

        public T[] PrePage()
        {
            if (_currentPage > 1)
            {
                _currentPage -= 1;
                return GetItemsByPage(_currentPage);
            }
            else
            {
                return GetItemsByPage(_currentPage);
            }
        }

        public void Clear()
        {
            _currentPage = 0;
            _maxPage = 0;
            _totalCount = 0;
            if (itemList != null)
            {
                itemList.Clear();
            }
        }
    }
}
