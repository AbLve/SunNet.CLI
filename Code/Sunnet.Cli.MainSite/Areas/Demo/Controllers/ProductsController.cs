using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Sunnet.Cli.MainSite.Areas.Demo.Models;
using Sunnet.Framework.Core.Extensions;
using Sunnet.Framework.Core.Tool;

namespace Sunnet.Cli.MainSite.Areas.Demo.Controllers
{
    public class ProductsController : Controller
    {
        private OrderDbContext _db = new OrderDbContext();
        //
        // GET: /Demo/Products/
        public ActionResult Index()
        {
            return View(new List<Products>());
        }

        [Route("~/Demo/Products/GetProducts/{keyword}/{sort}/{order}/{first}/{count}")]
        public string GetProducts(string keyword, string sort, string order, int first, int count)
        {
            return SearchProducts(keyword, sort, order, first, count);
        }

        public string SearchProducts(string keyword, string sort, string order, int first, int count)
        {
            var query = _db.Products.Where(x => x.Name.Contains(keyword)).OrderBy(sort, order.Equals("asc", StringComparison.CurrentCultureIgnoreCase) ? ListSortDirection.Ascending : ListSortDirection.Descending).AsQueryable();
            var result = new Dictionary<string, object>();
            result.Add("data", query.Skip(first).Take(count));
            result.Add("total", query.Count());
            return JsonConvert.SerializeObject(result);
        }
        //
        // GET: /Demo/Products/Details/5
        public ActionResult Details(int id)
        {
            return View(_db.Products.Find(id));
        }

        //
        // GET: /Demo/Products/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Demo/Products/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Demo/Products/Edit/5
        public ActionResult Edit(int id)
        {
            return View(_db.Products.Find(id));
        }

        private OperationResult SaveProduct(Products product)
        {
            // 这一步应该在Business完成
            OperationResult result = new OperationResult(OperationResultType.Success);
            if (product.Name.Contains("Android") && product.Price > 6000)
                result.Message = "Android devices' price can not bigger than 6000";
            if (product.Name.Contains("Apple") && product.Price < 1800)
                result.Message = "Apple devices' price can not small than 1800";
            try
            {
                if (result.ResultType == OperationResultType.Success)
                {
                    product.CreatedOn = DateTime.Now;
                    product.CreatedOnDateTime = DateTime.Now;
                    product.CreatedOnTime = DateTime.Now;
                    _db.Entry(product).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }
            catch (DbEntityValidationException ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        //
        // POST: /Demo/Products/Edit/5
        [ValidateAntiForgeryToken]
        [HttpPost]
        public string Edit(int id, Products product)
        {
            var response = new Dictionary<string, object>();
            // UI 层的try不是必须的, 只有有可能出异常的情况才需要, 需要记录日志
            // 表单请参考AssessmentController, 这个Demo版本已过时
            //try
            //{
            if (ModelState.IsValid)
            {
                var result = SaveProduct(product);
                response.Add("success", result.ResultType == OperationResultType.Success);
                response.Add("data", product);
                response.Add("msg", result.Message);
            }
            //}
            //catch (Exception ex)
            //{
            //    ModelState.AddModelError("Exception", ex);
            //}
            response.Add("modelState", ModelState);
            return JsonConvert.SerializeObject(response);
        }

        //
        // GET: /Demo/Products/Delete/5
        public ActionResult Delete(int id)
        {
            return View(_db.Products.Find(id));
        }

        //
        // POST: /Demo/Products/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
