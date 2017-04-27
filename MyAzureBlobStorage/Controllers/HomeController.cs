﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;

namespace MyAzureBlobStorage.Controllers
{
    public class HomeController : Controller
    {
        public const string accountName = "sivashan";
        public const string accountKey = "KUQJSK+WtkpdAFe3p0c8r2stjmvsvkTUqFFmXzdvU4ubSCMkqmTtOF8fparLDzUhwG4h7+w9375VZ+gk5QJEVg==";

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file != null)
            {
                var attachmentFileName = Path.GetFileName(file.FileName);
                string _fileExtension = System.IO.Path.GetExtension(file.FileName);
                byte[] filecontent = new byte[file.ContentLength];
                file.InputStream.Read(filecontent, 0, file.ContentLength);


                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=http;AccountName=" + accountName + ";AccountKey=" + accountKey);

                CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();

                CloudBlobContainer blobContainer = blobClient.GetContainerReference("sivacontainer");
                //blobContainer.CreateIfNotExists();

                BlobContainerPermissions containerPermissions = new BlobContainerPermissions();
                containerPermissions.PublicAccess = BlobContainerPublicAccessType.Off;
                blobContainer.SetPermissions(containerPermissions);

                CloudBlockBlob blob = blobContainer.GetBlockBlobReference(attachmentFileName);

                using (var stream = new MemoryStream(filecontent, writable: false))
                {
                    blob.UploadFromStream(stream, null, RequestOptions());
                }
            }
            return RedirectToAction("About");
        }
        
        private BlobRequestOptions RequestOptions()
        {
            return new BlobRequestOptions() { ServerTimeout = TimeSpan.FromMinutes(60), MaximumExecutionTime = TimeSpan.FromMinutes(60) };
        }
    }
}