//
// Copyright (C) Microsoft Corporation.  All rights reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using BoomerangX.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Globalization;
using System.Net.Mail;

namespace BoomerangX.Utils
{
    public class BlobOperationUtils : IDisposable
    {
        ILogger log = null;

        public BlobOperationUtils(ILogger logger)
        {
            log = logger;
        }

        public void CreateAndConfigure(string containerName = "default")
        {
            try
            {
                CloudStorageAccount storageAccount = StorageUtils.StorageAccount;

                // Create a blob client and retrieve reference to images container
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(GetUsername(containerName));

                // Create the "images" container if it doesn't already exist.
                if (container.CreateIfNotExists())
                {
                    // Enable public access on the newly created "images" container
                    container.SetPermissions(
                        new BlobContainerPermissions
                        {
                            PublicAccess =
                                BlobContainerPublicAccessType.Blob
                        });

                    log.Information(string.Format(CultureInfo.InvariantCulture, "Successfully created Blob Storage {0} Container and made it public", containerName));
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "Failure to Create or Configure images container in Blob Storage Service");
                throw;
            }
        }

        private string GetUsername(string emailAddress)
        {
            var mailAddress = new MailAddress(emailAddress);
            return mailAddress.User;
        }

        async public Task<string> UploadFileAsync(string fileToUpload, string containerName = "default")
        {            
            if (fileToUpload == null || !File.Exists(fileToUpload))
            {
                return null;
            }

            string fullPath = null;
            Stopwatch timespan = Stopwatch.StartNew();

            try
            {
                CloudStorageAccount storageAccount = StorageUtils.StorageAccount;

                // Create the blob client and reference the container
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(GetUsername(containerName));

                // Create a unique name for the images we are about to upload
                string fileName = string.Format("task-file-{0}{1}",
                    Guid.NewGuid().ToString(),
                    Path.GetExtension(fileToUpload));

                // Upload image to Blob Storage
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
                await blockBlob.UploadFromStreamAsync(File.OpenRead(fileToUpload));
                fullPath = blockBlob.Uri.ToString();
                timespan.Stop();
                log.TraceApi("Blob Service", "BlobOperationUtils.UploadFileAsync", timespan.Elapsed, "filepath={0}", fullPath);
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error upload file blob to storage");
                throw;
            }

            return fullPath;
        }

        public void Dispose()
        {
            log = null;
        }
    }
}