namespace Shared.S3
{
	public interface IMinioService
	{
		Task DeleteObject(string bucketName, string objectName, CancellationToken cancellationToken = default);
		Task Init(string bucketName, string policy, CancellationToken cancellationToken = default);
		Task PutObject(string bucketName, string objectName, Stream data, string contentType, CancellationToken cancellationToken = default);
	}
}