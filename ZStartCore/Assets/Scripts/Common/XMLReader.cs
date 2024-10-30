using System.Collections.Generic;

namespace ZStart.Core.Common
{
    public class XMLNode
    {
        public string tagName = "";
        public string textValue = "";
        public XMLNode parentNode = null;
        private List<XMLNode> _children = null;
        public List<XMLNode> children
        {
            get { return _children; }
        }

        private Dictionary<string, string> _attributes;
        public Dictionary<string, string> attributes
        {
            get { return _attributes; }
        }

        public XMLNode()
        {
            tagName = "NONE";
            textValue = "";
            parentNode = null;
            _children = new List<XMLNode>();
            _attributes = new Dictionary<string, string>();
        }

        public void AddChild(XMLNode node)
        {
            _children.Add(node);
        }

        public bool HasAttribute(string key)
        {
            if (_attributes.ContainsKey(key))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddAttribute(string key, string val)
        {
            if (_attributes.ContainsKey(key))
            {
                _attributes[key] = val;
            }
            else
            {
                _attributes.Add(key, val);
            }
        }

        public string GetAttribute(string attName)
        {
            string val = "";
            _attributes.TryGetValue(attName, out val);
            return val;
        }
    }

    public class XMLReader
    {

        private static char TAG_START = "<"[0];
        private static char TAG_END = ">"[0];
        private static char TABLE = "\t"[0];
        private static char SPACE = " "[0];
        private static char QUOTE = "\""[0];
        private static char SLASH = "/"[0];
        private static char EQUALS = "="[0];
        private static char QUESTION = "?"[0];
        private static char NEWLINE = "\n"[0];
        private static string BEGIN_QUOTE = "" + EQUALS + QUOTE;

        XMLNode _rootNode;

        public XMLNode rootNode
        {
            get { return _rootNode; }
        }

        public XMLReader(string text)
        {
            Parse(text);
        }

        public override string ToString()
        {
            string output = "";
            output = ">XML: " + this._rootNode.tagName + NEWLINE;
            output += ToString(this._rootNode, 1);
            return output;
        }

        public string ToString(XMLNode node, int indent)
        {
            string output = "";
            indent++;

            string indentString = "";
            for (int i = 0; i < indent; i++)
            {
                indentString += ">";
            }

            foreach (XMLNode n in node.children)
            {
                output += indentString + " " + n.tagName + " [" + n.textValue + "] ";

                foreach (KeyValuePair<string, string> p in n.attributes)
                {
                    output += "<" + p.Key + ": " + p.Value + "> ";
                }
                output += NEWLINE;
                output += ToString(n, indent);
            }
            return output;
        }

        public void Parse(string xml)
        {
            if (string.IsNullOrEmpty(xml)) return;
            int index = 0;
            int lastIndex = 0;
            XMLNode currentNode = null;

            this._rootNode = new XMLNode();
            currentNode = _rootNode;

            while (true)
            {
                index = xml.IndexOf(TAG_START, lastIndex);
                if (index < 0 || index >= xml.Length) break;
                index++; // skip the tag-char

                lastIndex = xml.IndexOf(TAG_END, index);
                if (lastIndex < 0 || lastIndex >= xml.Length) break;

                int tagLength = lastIndex - index;
                string xmlTag = xml.Substring(index, tagLength);

                // The tag starts with "<?"
                if (xmlTag[0] == QUESTION)
                {
                    continue;
                }

                // The tag starts with "</", it is thus an end tag
                if (xmlTag[0] == SLASH)
                {
                    currentNode = currentNode.parentNode;
                    continue;
                }

                bool openTag = true;

                // The tag ends in "/>", it is thus a closed tag
                if (xmlTag[tagLength - 1] == SLASH)
                {
                    xmlTag = xmlTag.Substring(0, tagLength - 1); // cut away the slash
                    openTag = false;
                }

                XMLNode node = ParseTag(xmlTag);
                node.parentNode = currentNode;

                if (currentNode.parentNode == null)
                {
                    this._rootNode = node;
                }
                else
                {
                    currentNode.AddChild(node);
                }

                if (openTag)
                {
                    int nextNode = 0;
                    nextNode = xml.IndexOf(TAG_START, lastIndex);
                    if (nextNode >= 0 && nextNode < xml.Length)
                    {
                        node.textValue = xml.Substring(lastIndex + 1, nextNode - lastIndex - 1).Trim();
                    }
                    else
                    {
                        node.textValue = "";
                    }
                    currentNode = node;
                }
            }
        }

        private static XMLNode ParseTag(string xmlTag)
        {
            XMLNode node = new XMLNode();

            int nameEnd = xmlTag.IndexOf(SPACE, 0);
            if (nameEnd < 0)
            {
                node.tagName = xmlTag;
                return node;
            }

            node.tagName = xmlTag.Substring(0, nameEnd);

            string attrString = xmlTag.Substring(nameEnd, xmlTag.Length - nameEnd);
            return ParseAttributes(attrString, node);
        }

        private static string PathGetFirstNode(string path)
        {
            int index = path.IndexOf(SLASH);
            if (index >= 0 && index < path.Length)
            {
                return path.Substring(0, index);
            }
            else
            {
                return path.Trim();
            }
        }

        private static XMLNode ParseAttributes(string attrString, XMLNode node)
        {
            int index = 0;
            int attrNameIndex = 0;
            int lastIndex = 0;

            string trim = "";
            string[] array = attrString.Split(TABLE);
            for (int i = 0; i < array.Length; i++)
            {
                if (string.IsNullOrEmpty(array[i]))
                    trim += " ";
                else
                    trim += array[i] + " ";
            }
            attrString = trim;

            while (true)
            {
                index = attrString.IndexOf(BEGIN_QUOTE, lastIndex);
                if (index < 0 || index >= attrString.Length) break;

                attrNameIndex = attrString.LastIndexOf(SPACE, index);
                if (attrNameIndex < 0 || attrNameIndex >= attrString.Length) break;
                attrNameIndex++; // skip space char
                string attrName = attrString.Substring(attrNameIndex, index - attrNameIndex);

                index += 2; // skip the equal and quote chars

                lastIndex = attrString.IndexOf(QUOTE, index);
                if (lastIndex < 0 || lastIndex >= attrString.Length) break;

                int tagLength = lastIndex - index;
                string attrValue = attrString.Substring(index, tagLength);
                node.AddAttribute(attrName, attrValue);
            };

            return node;
        }


        // GetTextValue()
        public string GetTextValue(string path)
        {
            return GetTextValue(this._rootNode, path);
        }

        public string GetTextValue(XMLNode node)
        {
            return GetTextValue(node, "");
        }

        public string GetTextValue(XMLNode node, string path)
        {
            if (path.Trim() == "")
            {
                return node.textValue;
            }
            string nodeName = PathGetFirstNode(path);
            foreach (XMLNode n in node.children)
            {
                if (n.tagName == nodeName)
                {
                    int index = 0;
                    index = path.IndexOf(SLASH);
                    if (index >= 0 && index < path.Length)
                    {
                        return GetTextValue(n, path.Substring(index + 1));
                    }
                    else
                    {
                        return n.textValue;
                    }
                }
            }
            return "";
        }


        // GetAttribute()
        public string GetAttribute(string path, string attributeName)
        {
            return GetAttribute(this._rootNode, path, attributeName);
        }

        public string GetAttribute(string attributeName)
        {
            return GetAttribute(this._rootNode, "", attributeName);
        }

        public string GetAttribute(XMLNode node, string attributeName)
        {
            return GetAttribute(node, "", attributeName);
        }

        public string GetAttribute(XMLNode node, string path, string attributeName)
        {
            if (path.Trim() == "")
            {
                return node.GetAttribute(attributeName);
                //if (node.attributes.ContainsKey(attributeName)) {
                //    return node.attributes[attributeName];
                //}
                //else {
                //    return "";
                //}
            }
            string nodeName = PathGetFirstNode(path);
            foreach (XMLNode n in node.children)
            {
                if (n.tagName == nodeName)
                {
                    int index = 0;
                    index = path.IndexOf(SLASH);
                    if (index >= 0 && index < path.Length)
                    {
                        return GetAttribute(n, path.Substring(index + 1), attributeName);
                    }
                    else
                    {
                        return node.GetAttribute(attributeName);
                        //if (n.attributes.ContainsKey(attributeName)) {
                        //    return n.attributes[attributeName];
                        //}
                        //else {
                        //    return "";
                        //}
                    }
                }
            }
            return null;
        }


        // GetNode()
        public XMLNode GetNode(string path)
        {
            return GetNode(this._rootNode, path);
        }

        public XMLNode GetNode(XMLNode node, string path)
        {
            string nodeName = PathGetFirstNode(path);
            foreach (XMLNode n in node.children)
            {
                if (n.tagName == nodeName)
                {
                    int index = 0;
                    index = path.IndexOf(SLASH);
                    if (index >= 0 && index < path.Length)
                    {
                        return GetNode(n, path.Substring(0, index + 1));
                    }
                    else
                    {
                        return n;
                    }
                }
            }
            return null;
        }


        // GetNodeList()
        public List<XMLNode> GetNodeList(string path)
        {
            return GetNodeList(this._rootNode, path);
        }

        public List<XMLNode> GetNodeList(XMLNode node, string path)
        {
            List<XMLNode> nodeList = null;
            string nodeName = PathGetFirstNode(path);
            int index = path.IndexOf(SLASH);
            bool isLastNode = false;
            bool isFound = false;

            if (index < 0 || index >= path.Length) isLastNode = true;
            if (isLastNode)
            {
                nodeList = new List<XMLNode>();
            }

            foreach (XMLNode n in node.children)
            {
                if (n.tagName == nodeName)
                {
                    isFound = true;

                    if (isLastNode)
                    {
                        nodeList.Add(n);
                    }
                    else
                    {
                        return GetNodeList(n, path.Substring(0, index + 1));
                    }
                }
            }
            if (isLastNode && !isFound)
            {
                return null;
            }
            else
            {
                return nodeList;
            }
        }
    }


}
