using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sunnet.Cli.MainSite.Areas.Demo.Models
{
    public enum SexOfPerson
    {
        None = 0,
        Male = 1,
        Female = 2
    }

    public class Person
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public SexOfPerson Sex { get; set; }
        public string Photo { get; set; }
    }
}