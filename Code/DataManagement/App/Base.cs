using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core;

namespace DataManagement
{
    public class Base
    {
        EFUnitOfWorkContext _unitWorkContext;
        protected EFUnitOfWorkContext UnitWorkContext
        {
            get
            {
                if (_unitWorkContext == null)
                    _unitWorkContext = new EFUnitOfWorkContext();
                return _unitWorkContext;
            }
        }

        VCWUnitOfWorkContext _vcwUnitWorkContext;
        protected VCWUnitOfWorkContext VcwUnitWorkContext
        {
            get
            {
                if (_vcwUnitWorkContext == null)
                    _vcwUnitWorkContext = new VCWUnitOfWorkContext();
                return _vcwUnitWorkContext;
            }
        }


        PracticeUnitOfWorkContext _practiceUnitWorkContext;
        protected PracticeUnitOfWorkContext PracticeUnitWorkContext
        {
            get
            {
                if (_practiceUnitWorkContext == null)
                    _practiceUnitWorkContext = new PracticeUnitOfWorkContext();
                return _practiceUnitWorkContext;
            }
        }

        AdeUnitOfWorkContext _adeUnitWorkContext;
        protected AdeUnitOfWorkContext AdeUnitWorkContext
        {
            get
            {
                if (_adeUnitWorkContext == null)
                    _adeUnitWorkContext = new AdeUnitOfWorkContext();
                return _adeUnitWorkContext;
            }
        }

    }
}
