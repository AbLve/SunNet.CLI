using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Business.Students.Models;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Core.Users.Entities;
using System.Linq.Expressions;
using LinqKit;
using Sunnet.Framework.Core.Extensions;

namespace Sunnet.Cli.Business.Students
{
    public partial class StudentBusiness
    {

        public ChildEntity GetChildById(int id, UserBaseEntity user)
        {
            ChildEntity entity = null;
            var child = _studentContract.GetChild(id);
            if (child != null && user.Parent != null)
            {
                if (ExistParentChild(user.Parent.ID, id))
                {
                    entity = child;
                }
            }
            return entity;
        }

        public ChildEntity GetChild(string firstName, string lastName, DateTime birthdate)
        {
            return _studentContract.Children.FirstOrDefault(x =>
                x.FirstName == firstName && x.LastName == lastName && x.BirthDate == birthdate);
        }

        public ChildEntity GetChildByStudentId(int studentId)
        {
            return _studentContract.Children.FirstOrDefault(x => x.StudentId == studentId);
        }

        public List<ChildListModel> SearchChilds(Expression<Func<ChildEntity, bool>> condition,
            string sort, string order, int first, int count, out int total)
        {
            var query = _studentContract.Children.AsExpandable().Where(condition).Select(e => new ChildListModel()
                {
                    ID = e.ID,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    BirthDate = e.BirthDate,
                    PINCode = e.PINCode, 
                    StudentId = e.StudentId
                });
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }

        public OperationResult InsertChild(ChildEntity entity)
        {
            return _studentContract.InsertChild(entity);
        }

        public ChildEntity ChildListModelToEntity(ChildListModel model)
        {
            ChildEntity entity = new ChildEntity();
            entity.ID = model.ID;
            entity.FirstName = model.FirstName;
            entity.LastName = model.LastName;
            entity.BirthDate = model.BirthDate;
            entity.SchoolCity = model.SchoolCity;
            entity.SchoolZip = model.SchoolZip;
            entity.SchoolId = model.SchoolId;
            entity.PINCode = model.PINCode;
            entity.StudentId = model.StudentId;
            return entity;
        }

        public ChildListModel ChildEntityToListModel(ChildEntity entity)
        {
            ChildListModel model = new ChildListModel();
            model.ID = entity.ID;
            model.FirstName = entity.FirstName;
            model.LastName = entity.LastName;
            model.BirthDate = entity.BirthDate;
            model.SchoolCity = entity.SchoolCity;
            model.SchoolZip = entity.SchoolZip;
            model.SchoolId = entity.SchoolId;
            model.PINCode = entity.PINCode;
            model.SchoolName = entity.School != null ? entity.School.Name : "";
            model.StudentId = entity.StudentId;
            return model;
        }

        public bool ExistParentChild(int parentId, int childId)
        {
            return _studentContract.ParentChilds.Any(x => x.ParentId == parentId && x.ChildId == childId);
        }

        public OperationResult InsertParentChild(ParentChildEntity parentChild)
        {
            return _studentContract.InsertParentChild(parentChild);
        }


        public ParentChildEntity GetParentChild(int parentId, int childId)
        {
            return _studentContract.ParentChilds.FirstOrDefault(x => x.ParentId == parentId && x.ChildId == childId);
        }



        public OperationResult UpdateChild(ChildEntity entity)
        {
            return _studentContract.UpdateChild(entity);
        }



    }
}
