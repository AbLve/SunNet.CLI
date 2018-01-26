using System;
using System.Linq;
using System.Collections.Generic;

namespace Sunnet.Cli.Business.CAC.Models
{
    public class MyActivityQueryModel
    {
        public string SearchCollectionType { get; set; }

        public string SearchActivityName { get; set; }

        public string SearchDomainOrSubDomain { get; set; }
    }
}
