using Sunnet.Cli.Core.Permission.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Permission.Models
{
    public class AssignedPackageModel
    {
        public int ID { get; set; }

        public int PackageId { get; set; }

        public string PackageName { get; set; }

        public string PackageDescription { get; set; }

        public int ScopeId { get; set; }

        public string ScopeName { get; set; }

        public AssignedType Type { get; set; }
    }


    /// <summary>
    /// Community和School所对应的package
    /// </summary>
    public class GroupPackageModel
    {
        public int PackageId { get; set; }

        public string PackageName { get; set; }

        public string PackageDescription { get; set; }
    }
}
