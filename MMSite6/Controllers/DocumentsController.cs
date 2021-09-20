using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MMSite6.Models;

namespace MMSite6.Controllers
{
    public class DocumentsController : Controller
    {

        private readonly MMSite6Context _context;
        private IConfiguration _configuration;
        private DocumentsAPIController _documentsAPIController;
        private ItemsAPIController _itemsAPIController;
        private OrdersAPIController _ordersAPIController;
        public DocumentsController(MMSite6Context context, IConfiguration Configuration)
        {
            _context = context;
            _configuration = Configuration;
            _ordersAPIController = new OrdersAPIController(_context);
            _documentsAPIController = new DocumentsAPIController(_context);
            _itemsAPIController = new ItemsAPIController(_context);
        }
		[Authorize(Roles = "Admin")]
		public IActionResult AdminUpload()
        {
            var orders = from o in _context.order
                         select o;
            orders = orders.Where(m => m.status == OrderStatus.InProgress || m.status == OrderStatus.Opened);
            orders = orders.Include(m => m.user);

            ViewData["orderSummary"] = new SelectList(from o in orders select new
            { orderId = o.orderId, displayText = o.summary + " - " + o.user.firstName + " " + o.user.lastName}, "orderId", "displayText", null);
            ViewData["orderId"] = new SelectList(_context.order, "orderId", "orderId");
            return View();
        }
		[Authorize(Roles = "Admin")]
		[HttpPost("UploadFiles")]
        //OPTION A: Disables Asp.Net Core's default upload size limit
        [DisableRequestSizeLimit]
        //OPTION B: Uncomment to set a specified upload file limit
        //[RequestSizeLimit(40000000)] 

        public async Task<IActionResult> Post(List<IFormFile> files, Document document, [Bind("itemId,address")] Item item)
        {
            var uploadSuccess = false;
            string uploadedUri = null;

            foreach (var formFile in files)
            {
                if (formFile.Length <= 0)
                {
                    continue;
                }

                // NOTE: uncomment either OPTION A or OPTION B to use one approach over another

                // OPTION A: convert to byte array before upload
                //using (var ms = new MemoryStream())
                //{
                //    formFile.CopyTo(ms);
                //    var fileBytes = ms.ToArray();
                //    uploadSuccess = await UploadToBlob(formFile.FileName, fileBytes, null);

                //}

                // OPTION B: read directly from stream for blob upload      
                using (var stream = formFile.OpenReadStream())
                {
                    (uploadSuccess, uploadedUri) = await UploadToBlob(formFile.FileName, null, stream);
                    TempData["uploadedUri"] = uploadedUri;
                    var selectedItem = item.itemId;
                    var i = _itemsAPIController.GetItembyId(selectedItem).Result.Value;
                    //await _ordersAPIController.UpdateStatus(i.orderId, OrderStatus.InProgress);
                    var documentLink = new Document { documentPath = uploadedUri, item = i };
                    await _documentsAPIController.PostDocument(documentLink);
                }

            }

            if (uploadSuccess)
                return View("UploadSuccess");
            else
                return View("UploadError");
        }
		[Authorize(Roles = "Admin")]
		public ActionResult Delete(int id)
        {
            var document = _documentsAPIController.GetDocument(id).Result.Value;
            var fileName = document.documentPath;
            string[] strings = fileName.Split('/');
            var _containerName = strings[3];
            var blockName = strings[4];
            //var _containerName = "uploadblob7d920e25-95b1-4aa3-adcb-daf088a7654c";
            string storageConnectionString = _configuration["storageconnectionstring"];
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudBlobClient _blobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer _cloudBlobContainer = _blobClient.GetContainerReference(_containerName);
            CloudBlockBlob _blockBlob = _cloudBlobContainer.GetBlockBlobReference(blockName);
            
            _blockBlob.Delete();
            _cloudBlobContainer.Delete();

            _documentsAPIController.DeleteDocument(id);

			return Redirect("https://mitchellmeasures.azurewebsites.net/");

		}
		[Authorize(Roles = "Admin")]
		private async Task<(bool, string)> UploadToBlob(string filename, byte[] imageBuffer = null, Stream stream = null)
        {
            CloudStorageAccount storageAccount = null;
            CloudBlobContainer cloudBlobContainer = null;
            string storageConnectionString = _configuration["storageconnectionstring"];

            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                try
                {
                    // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                    // Create a container called 'uploadblob' and append a GUID value to it to make the name unique. 
                    cloudBlobContainer = cloudBlobClient.GetContainerReference("uploadblob" + Guid.NewGuid().ToString());
                    await cloudBlobContainer.CreateAsync();

                    // Set the permissions so the blobs are public. 
                    BlobContainerPermissions permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    };
                    await cloudBlobContainer.SetPermissionsAsync(permissions);

                    // Get a reference to the blob address, then upload the file to the blob.
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(filename);

                    if (imageBuffer != null)
                    {
                        // OPTION A: use imageBuffer (converted from memory stream)
                        await cloudBlockBlob.UploadFromByteArrayAsync(imageBuffer, 0, imageBuffer.Length);
                    }
                    else if (stream != null)
                    {
                        // OPTION B: pass in memory stream directly
                        await cloudBlockBlob.UploadFromStreamAsync(stream);
                    }
                    else
                    {
                        return (false, null);
                    }

                    return (true, cloudBlockBlob.SnapshotQualifiedStorageUri.PrimaryUri.ToString());
                }
                catch (StorageException ex)
                {
                    return (false, null);
                }
                finally
                {
                    // OPTIONAL: Clean up resources, e.g. blob container
                    //if (cloudBlobContainer != null)
                    //{
                    //    await cloudBlobContainer.DeleteIfExistsAsync();
                    //}
                }
            }
            else
            {
                return (false, null);
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}