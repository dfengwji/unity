using System.Collections.Generic;

namespace Assets.Scenes.Test
{
    public class UserInfo
    {
        public string uid = "";
        public string name = "";
        public string cover = "";
        public int status = 0;
       
        public string URI
        {
            get
            {
                return "";
            }
        }

    }

    public class MessageInfo
    {
        public string uid;
        public int type;
        public string name;
        public string cover;
        public string target;
        public long stamp;
        public int[] states;

    }

    public class DownloadFile
    {
        public string file;
        public string url;
        public string hash;
        public int size;
    }

    public class MessageModel
    {
        public List<MessageInfo> messages;
        public List<UserInfo> users;
        public List<DownloadFile> downloads;
        public bool complete = false;

        public bool hadNew = false;

        private List<DownloadFile> newDownloads;
        public DownloadFile[] NewDownloads
        {
            get
            {
                downloads.AddRange(newDownloads);
                var arr = newDownloads.ToArray();
                newDownloads.Clear();
                return arr;
            }
        }

        private List<MessageInfo> newMsgs;
        public MessageInfo[] NewMessages
        {
            get
            {
                var arr = newMsgs.ToArray();
                newMsgs.Clear();
                return arr;
            }
        }

        public MessageModel()
        {
            messages = new List<MessageInfo>(100);
            users = new List<UserInfo>(100);
            newMsgs = new List<MessageInfo>(50);
            downloads = new List<DownloadFile>(60);
            newDownloads = new List<DownloadFile>(60);
        }
    }
}
