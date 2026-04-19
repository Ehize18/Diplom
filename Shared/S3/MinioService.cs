using Minio;
using Minio.DataModel.Args;

namespace Shared.S3
{
	public class MinioService : IMinioService
	{
		private readonly IMinioClient _minioClient;

		public MinioService(IMinioClient minioClient)
		{
			_minioClient = minioClient;
		}

		public async Task Init(string bucketName, string policy, CancellationToken cancellationToken = default)
		{
			if (await CheckBucket(bucketName, cancellationToken))
			{
				return;
			}
			await CreateBucket(bucketName, policy, cancellationToken);
		}

		private async Task<bool> CheckBucket(string name, CancellationToken cancellationToken = default)
		{
			var bea = new BucketExistsArgs();
			bea.WithBucket(name);
			return await _minioClient.BucketExistsAsync(bea, cancellationToken);
		}

		private async Task CreateBucket(string name, string policy, CancellationToken cancellationToken = default)
		{
			var mba = new MakeBucketArgs();
			mba.WithBucket(name);
			await _minioClient.MakeBucketAsync(mba, cancellationToken);
			await _minioClient.SetPolicyAsync(new SetPolicyArgs()
				.WithBucket(name)
				.WithPolicy(policy));
		}

		public async Task PutObject(string bucketName, string objectName, Stream data, string contentType, CancellationToken cancellationToken = default)
		{
			var poa = new PutObjectArgs()
				.WithBucket(bucketName)
				.WithObject(objectName)
				.WithStreamData(data)
				.WithObjectSize(data.Length)
				.WithContentType(contentType);
			await _minioClient.PutObjectAsync(poa, cancellationToken);
		}

		public async Task DeleteObject(string bucketName, string objectName, CancellationToken cancellationToken = default)
		{
			var roa = new RemoveObjectArgs()
				.WithBucket(bucketName)
				.WithObject(objectName);
			await _minioClient.RemoveObjectAsync(roa, cancellationToken);
		}

		public async Task<Stream> GetObject(string bucketName, string objectName, CancellationToken cancellationToken = default)
		{
			var ms = new MemoryStream();
			var goa = new GetObjectArgs()
				.WithBucket(bucketName)
				.WithObject(objectName)
				.WithCallbackStream(async cb =>
				{
					await cb.CopyToAsync(ms);
				});
			await _minioClient.GetObjectAsync(goa, cancellationToken);
			ms.Position = 0;
			return ms;
		}
	}
}
