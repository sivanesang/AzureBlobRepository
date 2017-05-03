using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using MyAzureBlobStorage.Models;
using Microsoft.ServiceBus.Messaging;

namespace MyAzureBlobStorage.Controllers
{
    public class HomeController : Controller
    {
        public const string accountName = "sivashan";
        public const string accountKey = "KUQJSK+WtkpdAFe3p0c8r2stjmvsvkTUqFFmXzdvU4ubSCMkqmTtOF8fparLDzUhwG4h7+w9375VZ+gk5QJEVg==";
        public const string connectionStringServiceBus = "Endpoint=sb://sivasservicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Tlca5fEs98xChsLVtBH3ilUXVCP3jmUqIAYwaO0M1BE=";
        public const string queueNameServiceBus = "mytestque1";

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

        public ActionResult SendMessage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SendMessagePost(MessageQueue model)
        {

          

            var stringContent = model.MessageContent;
            //model.MessageContent;

            var client = QueueClient.CreateFromConnectionString(connectionStringServiceBus, queueNameServiceBus);
            var message = new BrokeredMessage(stringContent);
            client.Send(message);

            return RedirectToAction("SendMessage");
        }

        public ActionResult ReceiveMessage()
        {
            var client = QueueClient.CreateFromConnectionString(connectionStringServiceBus, queueNameServiceBus);

            List<MessageQueue> msgList = new List<MessageQueue>();

            client.OnMessage(message =>
            {
                msgList.Add(new MessageQueue
                {
                    MessageId = message.MessageId,
                    MessageBody = message.GetBody<String>()
                });
            });

            return View(msgList);
        }



    }
}