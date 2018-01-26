using StructureMap;
using Sunnet.Framework.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml;

namespace Sunnet.Framework.Helpers
{
    public class EmailTemplete
    {
        public string Subject { get; set; }
        public string Body { get; set; }
    }

    public static class XmlHelper
    {
        public static string GetEmailContentToXmlByNode(string fileName, string node)
        {
            string path = "";
            if (System.Web.HttpContext.Current != null)
                path = System.Web.HttpContext.Current.Server.MapPath("~/Resource/EmailTemplate/");
            else
                path = AppDomain.CurrentDomain.BaseDirectory + "Resource/EmailTemplate/";
            return GetEmailContentToXmlByNode(path, fileName, node);
        }

        /// <summary>
        /// 用于Assessment站点
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string GetEmailTemp(string fileName, string node)
        {
            string path = "";
            if (System.Web.HttpContext.Current != null)
                path = System.Web.HttpContext.Current.Server.MapPath("~/Resources/EmailTemplate/");
            else
                path = AppDomain.CurrentDomain.BaseDirectory + "Resources/EmailTemplate/";
            return GetEmailContentToXmlByNode(path, fileName, node);
        }

        /// <param name="filePath">物理路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string GetEmailContentToXmlByNode(string filePath, string fileName, string node)
        {
            try
            {
                XmlDocument oXmlDoc = new XmlDocument();
                string path = string.Format("{0}/{1}", filePath, fileName);

                oXmlDoc.Load(path);
                XmlNode oNode = oXmlDoc.SelectSingleNode("/emails/email[position()=1]");
                string body = oNode.SelectSingleNode(node).InnerText;
                return body;
            }
            catch (Exception ex)
            {
                ObjectFactory.GetInstance<ISunnetLog>().Debug(ex);
                return "";
            }
        }

        public static EmailTemplete GetEmailTemplete(string fileName)
        {
            EmailTemplete t = new EmailTemplete();
            t.Subject = GetEmailContentToXmlByNode(fileName, "subject");
            t.Body = GetEmailContentToXmlByNode(fileName, "content");
            return t;
        }

        /// <summary>
        /// 用于Assessment站点
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static EmailTemplete GetEmailTemp(string fileName)
        {
            EmailTemplete t = new EmailTemplete();
            t.Subject = GetEmailTemp(fileName, "subject");
            t.Body = GetEmailTemp(fileName, "content");
            return t;
        }


        /// <param name="filePath">物理路径</param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static EmailTemplete GetEmailTemplete(string filePath, string fileName)
        {
            EmailTemplete t = new EmailTemplete();
            t.Subject = GetEmailContentToXmlByNode(filePath, fileName, "subject");
            t.Body = GetEmailContentToXmlByNode(filePath, fileName, "content");
            return t;
        }

        /// <summary>
        /// 读取指定路径的XML文件(映射为SelectListItem)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="localName"></param>
        /// <param name="inputLang"></param>
        /// <returns></returns>
        public static List<T> ReadXML<T>(string path, string localName, string inputLang,out int sumCount) where T : SelectListItem, new()
        {
            sumCount = 0;
            List<T> list = new List<T>();
            try
            {
                var currentpath = System.Web.HttpContext.Current.Server.MapPath(path);
                if (System.IO.File.Exists(currentpath))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(currentpath);
                    XmlElement xe = (XmlElement)xmlDoc.SelectSingleNode(localName);//获取localName节点
                    if (xe.HasChildNodes)
                    {
                        XmlNodeList nodeList = xe.ChildNodes;//获取localName节点的所有子节点
                        sumCount = nodeList.Count;
                        foreach (XmlElement temp in nodeList)
                        {
                            var lang = temp.GetAttribute("Group");
                            if (!string.IsNullOrEmpty(lang) && lang.Equals(inputLang))
                            {
                                T model = new T();
                                model.Text = temp.GetAttribute("Text");
                                model.Value = temp.GetAttribute("Value");
                                model.Group = new SelectListGroup { Name = temp.GetAttribute("Group") };
                                list.Add(model);
                            }
                        }

                    }
                }
            }
            catch (XmlException ex) {
                ObjectFactory.GetInstance<ISunnetLog>().Debug(ex);
            }
            return list;
        }
        /// <summary>
        /// 新建一个XML文件(可根据SelectListItem)
        /// </summary>
        /// <param name="list"></param>
        /// <param name="localName"></param>
        /// <param name="xmlName"></param>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public static bool InsertXML(List<SelectListItem> list, string localName, string xmlName, string relativePath)
        {
            bool isSuccess = false;
            try
            {
                if (list != null && list.Count > 0)
                {
                    //先创建XML,返回路径
                    XmlDocument xmldoc = new XmlDocument();
                    //加入XML的声明段落,<?xml version="1.0" encoding="gb2312"?>
                    XmlDeclaration xmldecl;
                    xmldecl = xmldoc.CreateXmlDeclaration("1.0", "gb2312", string.Empty);
                    xmldoc.AppendChild(xmldecl);
                    //加入一个根元素
                    XmlNode xmlelem = xmldoc.CreateElement(string.Empty, localName, string.Empty);
                    xmldoc.AppendChild(xmlelem);
                    XmlNode root = xmldoc.SelectSingleNode(localName);//查找<localName> 
                    foreach (var item in list)
                    {
                        XmlElement xesub1 = xmldoc.CreateElement("Item");
                        xesub1.SetAttribute("Text", item.Text);
                        xesub1.SetAttribute("Value", item.Value);
                        xesub1.SetAttribute("Group", item.Group.Name);
                        root.AppendChild(xesub1);//添加到<localName>节点中 
                    }
                    //然后在保存到源位置
                    xmldoc.AppendChild(xmlelem);
                    //保存创建好的XML文档
                    var currentpath = System.Web.HttpContext.Current.Server.MapPath(relativePath);
                    if (!Directory.Exists(currentpath))
                    {
                        Directory.CreateDirectory(currentpath);
                    }
                    string filename = xmlName + ".xml";
                    string path = currentpath + filename;
                    xmldoc.Save(path);
                    return true;
                }
            }
            catch (XmlException ex)
            {
                ObjectFactory.GetInstance<ISunnetLog>().Debug(ex);
                return false;
            }
            return isSuccess;
        }

        #region XML文档创建、删除和节点或属性的添加、修改
        /// <summary>
        /// 创建Xml文档
        /// </summary>
        /// <param name="xmlFileName">xml文档完全文件名（包含物理路径）</param>
        /// <param name="rootNoteName">根节点名称</param>
        /// <param name="version">版本号(必须为:"1.0")</param>
        /// <param name="encoding">编码方式</param>
        /// <param name="standalone">该值必须是"yes"或"no",如果为null,Save方法不在XML声明上写出独立属性</param>
        /// <returns></returns>
        public static bool CreateXmlDocument(string xmlFileName, string rootNoteName, string version, string encoding, string standalone)
        {
            bool isSuccess = false;
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration(version, encoding, standalone);
                XmlNode root = xmlDoc.CreateElement(rootNoteName);
                xmlDoc.AppendChild(xmlDeclaration);
                xmlDoc.AppendChild(root);
                xmlDoc.Save(xmlFileName);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                ObjectFactory.GetInstance<ISunnetLog>().Debug(ex);
                return false;
            }
            return isSuccess;
        }

        /// <summary>
        /// 依据匹配XPath表达式的第一个节点来创建它的子节点(如果此节点已存在则追加一个新的同名节点)
        /// </summary>
        /// <param name="xmlFileName">XML文档完全文件名(包含物理路径)</param>
        /// <param name="xpath">要匹配的XPath表达式(例如:"//节点名//子节点名</param>
        /// <param name="xmlNodeName">要匹配xmlNodeName的节点名称</param>
        /// <param name="innerText">节点文本值</param>
        /// <param name="xmlAttributeName">要匹配xmlAttributeName的属性名称</param>
        /// <param name="value">属性值</param>
        /// <returns></returns>
        public static bool CreateXmlNodeByXPath(string xmlFileName, string xpath,
            string xmlNodeName, string innerText, string xmlAttributeName, string value)
        {
            bool isSuccess = false;
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(xmlFileName);
                XmlNode xmlNode = xmlDoc.SelectSingleNode(xpath);
                if (xmlNode != null)
                {
                    XmlElement subElement = xmlDoc.CreateElement(xmlNodeName);
                    subElement.InnerXml = innerText;

                    if (!string.IsNullOrEmpty(xmlAttributeName) && !string.IsNullOrEmpty(value))
                    {
                        XmlAttribute xmlAttribute = xmlDoc.CreateAttribute(xmlAttributeName);
                        xmlAttribute.Value = value;
                        subElement.Attributes.Append(xmlAttribute);
                    }
                    xmlNode.AppendChild(subElement);
                }
                xmlDoc.Save(xmlFileName);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                ObjectFactory.GetInstance<ISunnetLog>().Debug(ex);
                return false;
            }
            return isSuccess;
        }

        /// <summary>
        /// 依据匹配XPath表达式的第一个节点来创建或更新它的子节点(如果节点存在则更新,不存在则创建)
        /// </summary>
        /// <param name="xmlFileName">XML文档完全文件名(包含物理路径)</param>
        /// <param name="xpath">要匹配的XPath表达式(例如:"//节点名//子节点名</param>
        /// <param name="xmlNodeName">要匹配xmlNodeName的节点名称</param>
        /// <param name="innerText">节点文本值</param>
        /// <returns></returns>
        public static bool CreateOrUpdateXmlNodeByXPath(string xmlFileName, string xpath, string xmlNodeName, string innerText)
        {
            bool isSuccess = false;
            bool isExistsNode = false;//标识节点是否存在
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(xmlFileName); //加载XML文档
                XmlNode xmlNode = xmlDoc.SelectSingleNode(xpath);
                if (xmlNode != null)
                {
                    //遍历xpath节点下的所有子节点
                    foreach (XmlNode node in xmlNode.ChildNodes)
                    {
                        if (node.Name.ToLower() == xmlNodeName.ToLower())
                        {
                            //存在此节点则更新
                            node.InnerXml = innerText;
                            isExistsNode = true;
                            break;
                        }
                    }
                    if (!isExistsNode)
                    {
                        //不存在此节点则创建
                        XmlElement subElement = xmlDoc.CreateElement(xmlNodeName);
                        subElement.InnerXml = innerText;
                        xmlNode.AppendChild(subElement);
                    }
                }
                xmlDoc.Save(xmlFileName);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                ObjectFactory.GetInstance<ISunnetLog>().Debug(ex);
                return false;
            }
            return isSuccess;
        }

        /// <summary>
        /// 依据匹配XPath表达式的第一个节点来创建或更新它的属性(如果属性存在则更新,不存在则创建)
        /// </summary>
        /// <param name="xmlFileName">XML文档完全文件名(包含物理路径)</param>
        /// <param name="xpath">要匹配的XPath表达式(例如:"//节点名//子节点名</param>
        /// <param name="xmlAttributeName">要匹配xmlAttributeName的属性名称</param>
        /// <param name="value">属性值</param>
        /// <returns>成功返回true,失败返回false</returns>
        public static bool CreateOrUpdateXmlAttributeByXPath(string xmlFileName, string xpath, string xmlAttributeName, string value)
        {
            bool isSuccess = false;
            bool isExistsAttribute = false;//标识属性是否存在
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(xmlFileName); //加载XML文档
                XmlNode xmlNode = xmlDoc.SelectSingleNode(xpath);
                if (xmlNode != null)
                {
                    //遍历xpath节点中的所有属性
                    foreach (XmlAttribute attribute in xmlNode.Attributes)
                    {
                        if (attribute.Name.ToLower() == xmlAttributeName.ToLower())
                        {
                            //节点中存在此属性则更新
                            attribute.Value = value;
                            isExistsAttribute = true;
                            break;
                        }
                    }
                    if (!isExistsAttribute)
                    {
                        //节点中不存在此属性则创建
                        XmlAttribute xmlAttribute = xmlDoc.CreateAttribute(xmlAttributeName);
                        xmlAttribute.Value = value;
                        xmlNode.Attributes.Append(xmlAttribute);
                    }
                }
                xmlDoc.Save(xmlFileName); //保存到XML文档
                isSuccess = true;
            }
            catch (Exception ex)
            {
                ObjectFactory.GetInstance<ISunnetLog>().Debug(ex);
                return false;
            }
            return isSuccess;
        }
        ///<summary>
        /// 删除XML
        ///</summary>
        ///<param name="path"></param>
        ///<returns></returns>
        public static bool DeleteXML(string path)
        {
            if (System.IO.File.Exists(path))
            {
                if (!IsUsed(path))
                {
                    System.IO.File.Delete(path);
                    return true;
                }
                else
                    return false;
            }
            return false;
        }
        ///<summary>
        /// 判断文件是否被占用.lunx
        ///</summary>
        ///<param name="fileName"></param>
        ///<returns></returns>
        public static bool IsUsed(String fileName)
        {
            bool result = false;
            try
            {
                System.IO.FileStream fs = System.IO.File.OpenWrite(fileName);
                fs.Close();
            }
            catch
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// 创建XML
        /// </summary>
        /// <param name="localName"></param>
        /// <param name="desc"></param>
        /// <param name="xmlName"></param>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public static string CreateXML(string localName, string xmlName, string relativePath, string desc = "")
        {
            XmlDocument xmldoc = new XmlDocument();
            //加入XML的声明段落,<?xml version="1.0" encoding="gb2312"?>
            XmlDeclaration xmldecl;
            xmldecl = xmldoc.CreateXmlDeclaration("1.0", "gb2312", null);
            xmldoc.AppendChild(xmldecl);
            //加入一个根元素
            XmlNode xmlelem = xmldoc.CreateElement("", localName, "");
            xmldoc.AppendChild(xmlelem);
            //保存创建好的XML文档
            string filename = xmlName + ".xml";//DateTime.Now.ToString("yyMMddHHmm") +
            string currentpath = System.Web.HttpContext.Current.Server.MapPath(relativePath);//"/XMLDB/MeasuresTemp/"
            if (!Directory.Exists(currentpath))
            {
                Directory.CreateDirectory(currentpath);
            }
            string path = currentpath + filename;
            xmldoc.Save(path);
            return path;
        }
        #endregion
    }
}
