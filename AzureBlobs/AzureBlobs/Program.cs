using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading.Tasks;

namespace AzureBlobs
{
    class Program
    {
        static void Main(string[] args)
        {
            string storageConnection = "DefaultEndpointsProtocol=https;AccountName=raviazurestorage;AccountKey=Dell+Q3zB4ByuRFHgZfC6jmWnVh0MlVcrddyIwYyQy5NBS2MYk7wb3FM27VKRZxD9m5KBOKhZwx/yVvkPBwubA==;EndpointSuffix=core.windows.net";

            Console.WriteLine("Connecting to our storage account");

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnection);

            Console.WriteLine("Getting container reference");

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference("azureravicontainer");

            Console.WriteLine("Creating blob container if not exists");

            blobContainer.CreateIfNotExistsAsync();
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference("azureraviblob");

            Console.WriteLine("Trying to download file");
            DownloadFile(blockBlob).Wait();

            Console.WriteLine("Uploading file to blob");

            UploadFile(blockBlob).Wait();

            Console.WriteLine("Upload done");
            Console.ReadKey();
        }

        private static async Task UploadFile(CloudBlockBlob blob)
        {
            using (var fileStream = System.IO.File.OpenRead(@"C:\Users\ravi.raghav\70-532-questions.pdf"))
            {
                await blob.UploadFromStreamAsync(fileStream);
            }
        }

        private static async Task DownloadFile(CloudBlockBlob blob)
        {
            if (blob.ExistsAsync().Result)
                await blob.DownloadToFileAsync(@"C:\Users\ravi.raghav\file_from_azure.pdf", 
                    System.IO.FileMode.CreateNew);
        }

        private static void GetContainerAttributes(CloudBlobContainer container)
        {
            container.FetchAttributesAsync().Wait();
            Console.WriteLine("Name :-  " + container.StorageUri.PrimaryUri.ToString()); ;
            Console.WriteLine("Last Modified Date :-  " + container.Properties.LastModified.ToString()); ;
        }

        private static void SetMetaData(CloudBlobContainer container)
        {
            container.Metadata.Clear();
            container.Metadata.Add("author", "Ravi");
            container.Metadata["category"] = "pdf doc";
            container.SetMetadataAsync().Wait();
        }

        private static void GetMetaData(CloudBlobContainer container)
        {
            container.FetchAttributesAsync().Wait();
            foreach (var item in container.Metadata)
            {
                Console.WriteLine("Key - " + item.Key);
                Console.WriteLine("Value - " + item.Value);
            }
        }

        private static void CopyBlob(CloudBlobContainer container)
        {
            CloudBlockBlob blob = container.GetBlockBlobReference("azureraviblob");
            CloudBlockBlob blobCopy = container.GetBlockBlobReference("azureraviblob-copy");
            blobCopy.StartCopyAsync(new Uri(blob.Uri.AbsoluteUri));
        }

        private static void CreateDirectoryAndUpload(CloudBlobContainer container)
        {
            CloudBlobDirectory cloudBlobDirectory = container.GetDirectoryReference("parent-folder");
            CloudBlobDirectory subCloudBlobDirectory = cloudBlobDirectory.GetDirectoryReference("child-folder");
            CloudBlockBlob blob = subCloudBlobDirectory.GetBlockBlobReference("newBlob");
            UploadFile(blob).Wait();
        }
    }
}
