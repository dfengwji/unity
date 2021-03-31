using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ZStart.Core;
using ZStart.RGraph.Enum;
using ZStart.RGraph.Model;

namespace ZStart.RGraph.Util
{
    public static class ParseUtil
    {
        public static string GetTextByPath(string path)
        {
            TextAsset asset = Resources.Load(path) as TextAsset;
            if (asset == null)
            {
                ZLog.Warning("Can not find config file that path = " + path + "!!!!");
                return "";
            }
            return asset.text;
        }

        public static GraphModel ParseGraphJson(string json,string root)
        {
            ZLog.Log("ParseGraphJson..." + json.Length);
            if (string.IsNullOrEmpty(json))
                return null;
            try
            {
                var obj = JsonMapper.ToObject(json);
                GraphModel model = new GraphModel(root)
                {
                    name = (string)obj["name"],
                };
                if(((IDictionary)obj).Contains("parent"))
                {
                    model.Parent = (string)obj["parent"];
                }
                var center = (string)obj["center"];
                model.SetCenter(center);
                var nodes = obj["nodes"];
                if (nodes != null && nodes.IsArray)
                {
                    foreach (JsonData item in nodes)
                    {
                        NodeInfo node = new NodeInfo
                        {
                            UID = (string)item["uid"],
                            name = (string)item["name"],
                            avatar = (string)item["avatar"],
                            type = (string)item["type"],
                            parent = center
                        };
                        if (string.IsNullOrEmpty(node.avatar))
                        {
                            node.avatar = Path.Combine(root, "cover/"+ node.name + ".png");
                        }
                        node.fullPath = Path.Combine(root, "entity/entity_" + node.UID + ".json");
                        model.AddNode(node, null);
                    }
                }
                var edges = obj["edges"];
                if (edges != null && edges.IsArray)
                {
                    foreach (JsonData item in edges)
                    {
                        LinkInfo edge = new LinkInfo
                        {
                            UID = (string)item["uid"],
                            name = (string)item["name"],
                            direction = (DirectionType)(int)item["direction"]
                        };
                        var from = (string)item["from"];
                        edge.from = model.GetNode(from);
                        var to = (string)item["to"];
                        edge.to = model.GetNode(to);
                        if (((IDictionary)item).Contains("type"))
                        {
                            edge.type = (string)item["type"];
                        }
                        model.AddEdge(edge);
                    }
                    model.CheckVirtualNodes();
                }
                return model;
            }
            catch (Exception e)
            {
                ZLog.Warning(json);
                ZLog.Warning(e.Message);
                return null;
            }
        }

        public static EntityInfo ParseEntityJson(string json, string root)
        {
            ZLog.Log("ParseNodeJson..." + json.Length);
            if (string.IsNullOrEmpty(json))
                return null;
            try
            {
                var obj = JsonMapper.ToObject(json);
                EntityInfo model = new EntityInfo
                {
                    name = (string)obj["name"],
                    uid = (string)obj["uid"],
                    type = (string)obj["type"],
                    remark = (string)obj["remark"],
                };
                model.properties = new List<PropertyInfo>(5);
                var remark = new PropertyInfo
                {
                    key = "remark",
                    entities = new PairInfo[1]
                };
                remark.entities[0].value = model.remark;
                model.properties.Add(remark);
                var props = obj["props"];
                if (props != null && props.IsArray)
                {
                    foreach (JsonData item in props)
                    {
                        var prop = new PropertyInfo
                        {
                            key = (string)item["key"],
                        };
                        List<PairInfo> list = new List<PairInfo>();
                        var array = item["array"];
                        foreach (JsonData t in array)
                        { 
                            list.Add(new PairInfo((string)t["key"], (string)t["value"]));
                        }
                        prop.entities = list.ToArray();
                        model.properties.Add(prop);
                    }
                }
                int count = 0;
                model.experiences = new List<AffairInfo>(5);
                var experiences = obj["experiences"];
                if (experiences != null && experiences.IsArray)
                {
                    foreach (JsonData item in experiences)
                    {
                        count += 1;
                        AffairInfo tmp = new AffairInfo
                        {
                            uid = "affair-"+count,
                            time = (string)item["time"],
                            year = (int)item["year"],
                            month = (int)item["month"],
                            name = (string)item["name"],
                            description = (string)item["desc"]
                        };
                        if (tmp.month < 1)
                            tmp.month = 1;
                        tmp.time = tmp.year+"";
                        
                        
                        var images = item["images"];
                        var imageList = new List<string>(4);
                        foreach (string image in images)
                        {
                            imageList.Add(string.IsNullOrEmpty(root) ? image : Path.Combine(root, image));
                        }
                        tmp.images = imageList.ToArray();

                        var keywordList = new List<PairInfo>(4);
                        var keywords = item["keywords"];
                        foreach (JsonData keyword in keywords)
                        {
                            keywordList.Add(new PairInfo((string)keyword["key"], (string)keyword["value"]));
                        }
                        tmp.keywords = keywordList.ToArray();
                        model.experiences.Add(tmp);
                    }
                }
                
                return model;
            }
            catch (Exception e)
            {
                ZLog.Warning(json);
                ZLog.Warning(e.Message);
                return null;
            }
        }

        public static List<ThemeInfo> ParseThemes(string json, string root)
        {
            ZLog.Log("ParseThemes..." + json.Length);
            if (string.IsNullOrEmpty(json))
                return null;
            try
            {
                List<ThemeInfo> themes = new List<ThemeInfo>(20);
                var array = JsonMapper.ToObject(json);
                if (array != null && array.IsArray)
                {
                    foreach (JsonData item in array)
                    {
                        var theme = new ThemeInfo
                        {
                            uid = (string)item["uid"],
                            name = (string)item["name"],
                            cover = Path.Combine(root, (string)item["cover"]),
                            remark = (string)item["remark"],
                        };

                        if (((IDictionary)item).Contains("node"))
                        {
                            theme.node = (string)item["node"];
                        }
                        themes.Add(theme);
                    }
                }
                return themes;
            }
            catch (Exception e)
            {
                ZLog.Warning(json);
                ZLog.Warning(e.Message);
                return null;
            }
        }
    }
}
