using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MockApplication.Data;
using MockApplication.Models;
using BoomerangX.SyncEngine.Tasks;
using BoomerangX.Setup;
using System.Text;

namespace MockApplication.Controllers
{
    public class SalesOrderDetailsController : Controller
    {
        private Entities db = new Entities();
        private bool inited;


        // GET: SalesOrderDetails
        public async Task<ActionResult> Index(long? id)
        {
            if (id == null)
            {
                return View(await db.SalesOrderDetails.ToListAsync());
            }
            else
            {
                try
                {
                    List<SalesOrderDetail> salesOrderDetailList = await db.SalesOrderDetails.Where(so => so.SalesOrderId == id).ToListAsync<SalesOrderDetail>();
                    if (salesOrderDetailList == null)
                    {
                        throw new InvalidOperationException("SalesOrderHeader was not found");
                    }

                    return View(salesOrderDetailList);
                }
                catch (Exception)
                {
                    return HttpNotFound();
                }
            }
        }

        // GET: SalesOrderDetails/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SalesOrderDetail salesOrderDetail = await db.SalesOrderDetails.FindAsync(id);
            if (salesOrderDetail == null)
            {
                return HttpNotFound();
            }
            return View(salesOrderDetail);
        }

        // GET: SalesOrderDetails/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SalesOrderDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "SalesOrderDetailId,SalesOrderHeaderId,ProductId")] SalesOrderDetail salesOrderDetail)
        {
            if (ModelState.IsValid)
            {
                db.SalesOrderDetails.Add(salesOrderDetail);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(salesOrderDetail);
        }

        // GET: SalesOrderDetails/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SalesOrderDetail salesOrderDetail = await db.SalesOrderDetails.FindAsync(id);
            if (salesOrderDetail == null)
            {
                return HttpNotFound();
            }
            return View(salesOrderDetail);
        }

        // POST: SalesOrderDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "SalesOrderDetailId,SalesOrderHeaderId,ProductId")] SalesOrderDetail salesOrderDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(salesOrderDetail).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(salesOrderDetail);
        }

        // GET: SalesOrderDetails/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SalesOrderDetail salesOrderDetail = await db.SalesOrderDetails.FindAsync(id);
            if (salesOrderDetail == null)
            {
                return HttpNotFound();
            }
            return View(salesOrderDetail);
        }

        // POST: SalesOrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            SalesOrderDetail salesOrderDetail = await db.SalesOrderDetails.FindAsync(id);
            db.SalesOrderDetails.Remove(salesOrderDetail);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

#pragma warning disable CS1998
        public async Task<ActionResult> Recommendations(string commaSeparatedItemIds)
#pragma warning restore CS1998
        {
            if (!inited)
            {
                TaskLibrary.AccountInfo = new Account();
                TaskLibrary.AccountInfo.Initialize();
                inited = true;
            }

            var mlLibrary = new AzureMLTaskLibrary(TaskLibrary.AccountInfo);
            if (!string.IsNullOrEmpty(commaSeparatedItemIds))
            {
                var listItems = commaSeparatedItemIds.Split('-').ToList();
                var listRecommendedItems = mlLibrary.GetRecommendation("", listItems, 10);
                List<RecommendationViewModel> transformedList = new List<RecommendationViewModel>(listRecommendedItems.Count());
                foreach(var item in listRecommendedItems)
                {
                    transformedList.Add(new RecommendationViewModel {
                        Name = item.Name, Id = item.Id, Rating = item.Rating, Reasoning = item.Reasoning
                    });
                }

                return PartialView("Recommendations", transformedList);
            }

            return View(new List<RecommendationViewModel> { });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
