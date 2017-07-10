using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace T_Touch_Central_Web.Controllers
{
    public class FileController : Controller
    {
        // GET: File
        public ActionResult Index()
        {
            return View();
        }

        #region Actions

        /// <summary>
        /// Uploads the file.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public virtual ActionResult UploadFile()
        {
            HttpPostedFileBase myFile = Request.Files["MyFile"];
            bool isUploaded = false;
            string message = "文件上传失败";
            if (myFile != null && myFile.ContentLength != 0)
            {
                string pathForSaving = Server.MapPath("~/Uploads");
                if (this.CreateFolderIfNeeded(pathForSaving))
                {
                    try
                    {
                        myFile.SaveAs(Path.Combine(pathForSaving, myFile.FileName));
                        isUploaded = true;
                        message = "文件上传成功!";
                    }
                    catch (Exception ex)
                    {
                        message = string.Format("文件上传失败: {0}", ex.Message);
                    }
                }
            }
            return Json(new { isUploaded = isUploaded, message = message }, "text/html");
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Creates the folder if needed.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private bool CreateFolderIfNeeded(string path)
        {
            bool result = true;
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            return result;
        }
        #endregion
    }
}