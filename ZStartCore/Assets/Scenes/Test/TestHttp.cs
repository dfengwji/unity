using Assets.Scenes.Test;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using ZStart.Core.Controller;
using ZStart.Core.Model;

public class TestHttp : MonoBehaviour
{
    public class RequestParam
    {
        public int page;
        public string sn;

        public RequestParam(string s, int p)
        {
            page = p;
            sn = s;
        }
    }

    public class ResponseInfo
    {
        public ResponseStatus status;
        public string data;
    }

    public class ResponseStatus
    {
        public int code;
        public string error;
    }

    // Start is called before the first frame update
    void Start()
    {
        RequestParam param = new RequestParam("YM20240914167967", 1);
        string data = JsonConvert.SerializeObject(param);
        ZHttpController.Post("test", "http://192.168.1.10/organization/app/dynamic", data, HttpCompleteHandler, false, false, true);
    }

    // Update is called once per frame
    private void HttpCompleteHandler(ZHttpController.Response info)
    {
        if (info.state != ZHttpController.Status.Normal)
        {
            UnityEngine.Debug.LogError("HttpCompleteHandler....net error = " + info.error);

            return;
        }

        var resp = JsonConvert.DeserializeObject<ResponseInfo>(info.data);
        if (resp.status.code != 0)
        {
            UnityEngine.Debug.LogError("HttpCompleteHandler ....state error : uid = " + info.uid + " ;result = " + info.data);
            return;
        }
        var msg = DecodeBase64(resp.data);
        if (string.IsNullOrEmpty(msg))
        {
            Debug.LogError("HttpCompleteHandler ....decode base64 error!!!");
            return;
        }
        Debug.LogError(msg);
        var model = JsonConvert.DeserializeObject<MessageModel>(msg);

    }

    public string DecodeBase64(string msg)
    {
        try
        {
            byte[] bytes = Convert.FromBase64String(msg);
            return Encoding.UTF8.GetString(bytes);
        }
        catch (Exception e)
        {
            Debug.LogError("DecodeBase64 error that reason = " + e.Message);
            return default;
        }
    }
}
