using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StructureMap;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Encrypt;
using WebGrease.Css.Extensions;

namespace Sunnet.Cli.MainSite.Models
{
    //public enum LinkType
    //{
    //    /// <summary>
    //    /// 添加SchoolSpecialist 到School
    //    /// </summary>
    //    SpecToSchool = 1
    //}

    public class LinkModel
    {
        private Dictionary<string, object> _others;
        private IEncrypt encrypter;
        public LinkModel()
        {
            CreatedOn = DateTime.Now;
            encrypter = ObjectFactory.GetInstance<IEncrypt>();
        }
        public Role RoleType { get; set; }

        public string Host { get; set; }

        public string Path { get; set; }

        public int Sender { get; set; }

        public int Recipient { get; set; }

        public Dictionary<string, object> Others
        {
            get { return _others ?? (_others = new Dictionary<string, object>()); }
            set { _others = value; }
        }

        public DateTime CreatedOn { get; private set; }

        private static int Properties = 4;

        public override string ToString()
        {
            var result = (int)(this.RoleType) + "|" + Sender + "|" + Recipient + "|" + this.CreatedOn.Ticks;
            Others.ForEach(x => result += "|" + x.Key + ":" + x.Value.ToString());
            result = this.Host + this.Path + encrypter.Encrypt(result);
            return result;
        }

        public static LinkModel Parse(string source)
        {
            if (string.IsNullOrEmpty(source)) return null;
            var props = source.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (props.Length < 3)
                return null;
            var link = new LinkModel();
            link.RoleType = (Role)int.Parse(props[0]);
            link.Sender = int.Parse(props[1]);
            link.Recipient = int.Parse(props[2]);
            link.CreatedOn = new DateTime(long.Parse(props[3]));
            if (props.Length > Properties)
            {
                for (int i = Properties; i < props.Length; i++)
                {
                    var keyValues = props[i].Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (keyValues.Length == 2)
                    {
                        var key = keyValues[0];
                        var value = keyValues[1];
                        if (!link.Others.ContainsKey(key)) link.Others.Add(keyValues[0], 0);
                        int intVal;
                        if (int.TryParse(value, out intVal))
                            link.Others[key] = intVal;
                        else
                            link.Others[key] = value;
                    }
                }
            }
            return link;
        }
    }
}