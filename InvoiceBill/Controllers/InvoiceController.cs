using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvoiceBill.Models;
using IronPdf;


namespace InvoiceBill.Controllers
{
    public class InvoiceController : Controller
    {
        // GET: Invoice
        public ActionResult Index()
        {   
            using(InvoiceDBEntities db = new InvoiceDBEntities())
            {

                List<tblInvoice> InvoiceList = db.tblInvoices.ToList();
                return View(InvoiceList);
            }
        }
        public ActionResult Create()
        {
            return View(new tblInvoice());
        }


        public ActionResult PrintInvoice( int id)
        {
            try
            {

                using (InvoiceDBEntities db = new InvoiceDBEntities())
                {
                    tblInvoice invoice = db.tblInvoices.Find(id);
                    var invoiceId = invoice.InvoiceId;
                    var ProductName = invoice.ProductName;
                    var ProductDescription = invoice.ProductDesc;
                    var ProductPrice = invoice.Price;
                    var Renderer = new ChromePdfRenderer(); // Instantiates Chrome Renderer

                    System.Text.StringBuilder invoiceHtml = new System.Text.StringBuilder();
                    invoiceHtml.Append("<!DOCTYPE html>\n");
                    invoiceHtml.Append("<html lang=\"en\">\n");
                    invoiceHtml.Append("<head>\n");
                    invoiceHtml.Append("    <meta charset=\"UTF-8\">\n");
                    invoiceHtml.Append("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\n");
                    invoiceHtml.Append("    <title>Invoice</title>\n");
                    invoiceHtml.Append("    <style>\n");
                    invoiceHtml.Append("        body {\n");
                    invoiceHtml.Append("            font-family: Arial, sans-serif;\n");
                    invoiceHtml.Append("        }\n");
                    invoiceHtml.Append("        table {\n");
                    invoiceHtml.Append("            width: 100%;\n");
                    invoiceHtml.Append("            border-collapse: collapse;\n");
                    invoiceHtml.Append("            margin-top: 20px;\n");
                    invoiceHtml.Append("        }\n");
                    invoiceHtml.Append("        th, td {\n");
                    invoiceHtml.Append("            border: 1px solid #ddd;\n");
                    invoiceHtml.Append("            padding: 8px;\n");
                    invoiceHtml.Append("            text-align: left;\n");
                    invoiceHtml.Append("        }\n");
                    invoiceHtml.Append("        th {\n");
                    invoiceHtml.Append("            background-color: #f2f2f2;\n");
                    invoiceHtml.Append("        }\n");
                    invoiceHtml.Append("    </style>\n");
                    invoiceHtml.Append("</head>\n");
                    invoiceHtml.Append("<body>\n");
                    invoiceHtml.Append("\n");
                    invoiceHtml.Append("    <h2>Invoice</h2>\n");
                    invoiceHtml.Append("    \n");
                    invoiceHtml.Append("    <p><strong>Invoice ID:</strong>"+ invoiceId + "</p>\n");
                    invoiceHtml.Append("    \n");
                    invoiceHtml.Append("    <table>\n");
                    invoiceHtml.Append("        <thead>\n");
                    invoiceHtml.Append("            <tr>\n");
                    invoiceHtml.Append("                <th>Product Name</th>\n");
                    invoiceHtml.Append("                <th>Product Description</th>\n");
                    invoiceHtml.Append("                <th>Price</th>\n");
                    invoiceHtml.Append("            </tr>\n");
                    invoiceHtml.Append("        </thead>\n");
                    invoiceHtml.Append("        <tbody>\n");
                    invoiceHtml.Append("            <tr>\n");
                    invoiceHtml.Append("                <td>" + ProductName + "</td>\n");
                    invoiceHtml.Append("                <td>" + ProductDescription + "</td>\n");
                    invoiceHtml.Append("                <td>$" + ProductPrice + "</td>\n");
                    invoiceHtml.Append("            </tr>\n");
                    invoiceHtml.Append("            <!-- Add more rows for additional products if needed -->\n");
                    invoiceHtml.Append("        </tbody>\n");
                    invoiceHtml.Append("    </table>\n");
                    invoiceHtml.Append("\n");
                    invoiceHtml.Append("</body>\n");
                    invoiceHtml.Append("</html>");

                    var pdf = Renderer.RenderHtmlAsPdf(invoiceHtml.ToString());
                    string filePath = Server.MapPath("~/App_Data/html_saved.pdf");
                    pdf.SaveAs(filePath);
               
                    return RedirectToAction("Index");

                
                  
                }
            }
            catch(Exception ex)
            {

                Console.WriteLine("you got error" + ex +"");

            }
            return RedirectToAction("Index");
        }

        public ActionResult ViewPdf()
        {
            string filePath = Server.MapPath("~/App_Data/html_saved.pdf");
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "application/pdf");
        }
         public ActionResult ViewPdfOnView()
        {
            return View();

        }

        [HttpPost]

        public ActionResult Create(tblInvoice invoice)
        {
            if (ModelState.IsValid)
            {
                using (InvoiceDBEntities db = new InvoiceDBEntities())
                {
                    db.tblInvoices.Add(invoice);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
               
            }
            else
            {
                return View();


            }
            
        }

        public ActionResult Edit(int id)
        {
            using (InvoiceDBEntities db = new InvoiceDBEntities()) {

                tblInvoice tbl = db.tblInvoices.Find(id);


                return View(tbl);
            }

          
        }

        [HttpPost]

        public ActionResult Edit(tblInvoice invoice)
        {
            using (InvoiceDBEntities db = new InvoiceDBEntities())
            {
                tblInvoice tbl = db.tblInvoices.Find(invoice.InvoiceId);

                tbl.ProductName = invoice.ProductName;
                tbl.ProductDesc = invoice.ProductDesc;
                tbl.Price = invoice.Price;
                db.SaveChanges();


                return RedirectToAction("Index");
            }


        }

        public ActionResult Delete(int id)
        {
            using (InvoiceDBEntities db = new InvoiceDBEntities())
            {

                db.tblInvoices.Remove(db.tblInvoices.Find(id));
                db.SaveChanges();
                return RedirectToAction("Index");
            }


        }
    }
}