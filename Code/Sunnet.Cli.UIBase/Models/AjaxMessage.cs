using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sunnet.Cli.UIBase.Models
{
    public class AjaxMessage
    {
        public AjaxMessage(string name, string title, string message, string @class)
        {
            Name = name;
            Class = @class;
            Message = message;
            Title = title;
        }

        public string Name { get; private set; }
        public string Title { get; private set; }
        public string Field { get; set; }
        public string Message { get; private set; }
        public string Class { get; private set; }
    }
}