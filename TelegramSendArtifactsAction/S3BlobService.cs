using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace TelegramSendArtifactsAction
{
    public class S3BlobService
    {
        private readonly string _accessKeyId;
        private readonly string _secretAccessKey;
        private readonly string _endpointUrl;
        private readonly string _space;

        public S3BlobService(
            string accessKeyId,
            string secretAccessKey,
            string endpointUrl,
            string space
        )
        {
            _accessKeyId = accessKeyId;
            _secretAccessKey = secretAccessKey;
            _endpointUrl = endpointUrl;
            _space = space;
        }

        public async Task<GetObjectMetadataResponse> GetMetaData(string directory, string fileName)
        {
            using var client = new AmazonS3Client(
                _accessKeyId,
                _secretAccessKey,
                new AmazonS3Config
                {
                    ServiceURL = _endpointUrl
                }
            );

            try
            {
                var objRes = await client.GetObjectMetadataAsync(
                    new GetObjectMetadataRequest()
                    {
                        BucketName = _space + "/" + directory,
                        Key = fileName
                    }
                );

                if (objRes.HttpStatusCode != HttpStatusCode.OK)
                {
                    return null;
                }

                return objRes;
            }
            catch (AmazonS3Exception e)
            {
                // if (e.StatusCode == HttpStatusCode.NotFound)
                // {
                //     _logger.LogWarning("s3Storage file metadata not found");
                // }
                // else
                // {
                //     _logger.LogError(e, "s3Storage error filemetadata:'{0}'", e.Message);
                // }

                return null;
            }
            catch (Exception e)
            {
                // _logger.LogError(e, "s3Storage error file metadata:'{0}'", e.Message);

                return null;
            }
        }

        public async Task Upload(string srcPath, string directory, string fileName)
        {
            using var client = new AmazonS3Client(
                _accessKeyId,
                _secretAccessKey,
                new AmazonS3Config
                {
                    ServiceURL = _endpointUrl
                }
            );

            try
            {
                await using var fs = File.OpenRead(srcPath);
                var fileTransferUtility = new TransferUtility(client);

                var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                {
                    InputStream = fs,
                    BucketName = _space + "/" + directory,
                    Key = fileName,
                    CannedACL = S3CannedACL.PublicRead
                };

                await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);
            }
            catch (AmazonS3Exception e)
            {
                // _logger.LogError(e, "s3Storage error upload file:'{0}'", e.Message);
            }
            catch (Exception e)
            {
                // _logger.LogError(e, "s3Storage error upload file:'{0}'", e.Message);
            }
        }

        public async Task<bool> Download(
            string srcPath,
            string directory,
            string fileName
        )
        {
            using var client = new AmazonS3Client(
                _accessKeyId,
                _secretAccessKey,
                new AmazonS3Config
                {
                    ServiceURL = _endpointUrl
                }
            );

            try
            {
                var objRes = await client.GetObjectAsync(
                    new GetObjectRequest()
                    {
                        BucketName = _space + "/" + directory,
                        Key = fileName
                    }
                );

                if (objRes.HttpStatusCode != HttpStatusCode.OK)
                {
                    // _logger.LogWarning("s3Storage download file not found");

                    return false;
                }

                await using var fo = File.OpenWrite(srcPath);
                await objRes.ResponseStream.CopyToAsync(fo);
                fo.Flush();
                fo.Close();

                return true;
            }
            catch (AmazonS3Exception e)
            {
                // if (e.StatusCode == HttpStatusCode.NotFound)
                // {
                //     _logger.LogWarning(e, "s3Storage download file not found");
                // }
                // else
                // {
                //     _logger.LogError(e, "s3Storage error download file:'{0}'", e.Message);
                // }

                return false;
            }
            catch (Exception e)
            {
                // _logger.LogError(e, "s3Storage error download file:'{0}'", e.Message);

                return false;
            }
        }
    }
}