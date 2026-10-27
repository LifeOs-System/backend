using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace Application.Services.Blob.Minio;

public class MinioBlobService : IBlobService
{
    private readonly string _bucketName;
    private readonly IMinioClient _minioClient;
    private readonly ILogger<MinioBlobService> _logger;

    public MinioBlobService(IOptions<MinioSettings> options, ILogger<MinioBlobService> logger)
    {
        _bucketName = options.Value.Bucket;
        _logger = logger;
        _minioClient = new MinioClient()
            .WithEndpoint(options.Value.Endpoint, options.Value.Port)
            .WithCredentials(options.Value.AccessKey, options.Value.SecretKey)
            .WithSSL(options.Value.UseSsl)
            .Build();
    }

    private string GenerateUniqueKey(string extension)
    {
        return $"{Guid.NewGuid():N}{extension}";
    }

    public async Task<string?> UploadBlobAsync(IFormFile? file, string? previousKey = null, CancellationToken ct = default)
    {
        if (file is null || file.Length == 0)
        {
            _logger.LogWarning("Intento de subida con archivo inválido o vacío.");
            return null;
        }

        var ext = Path.GetExtension(file.FileName).ToLower();
        var newKey = GenerateUniqueKey(ext);

        if (!string.IsNullOrEmpty(previousKey))
        {
            await DeleteBlobAsync(previousKey, ct);
        }

        await using var stream = file.OpenReadStream();
        var putArgs = new PutObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(newKey)
            .WithStreamData(stream)
            .WithObjectSize(file.Length)
            .WithContentType(file.ContentType);

        await _minioClient.PutObjectAsync(putArgs, ct);
        _logger.LogInformation("⬆ Archivo subido a MinIO: {Key}", newKey);

        return newKey;
    }

    public async Task<string?> UploadBlobDocumentAsync(IFormFile? file, string? previousKey = null, CancellationToken ct = default)
    {
        if (file is null || file.Length == 0)
        {
            _logger.LogWarning("Intento de subida de documento inválido o vacío.");
            return null;
        }

        var ext = Path.GetExtension(file.FileName).ToLower();
        var newKey = GenerateUniqueKey(ext);

        if (!string.IsNullOrEmpty(previousKey))
        {
            await DeleteBlobAsync(previousKey, ct);
        }

        await using var stream = file.OpenReadStream();
        var putArgs = new PutObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(newKey)
            .WithStreamData(stream)
            .WithObjectSize(file.Length)
            .WithContentType(file.ContentType);

        await _minioClient.PutObjectAsync(putArgs, ct);
        _logger.LogInformation("⬆ Documento subido a MinIO: {Key}", newKey);

        return newKey;
    }

    public async Task<string?> UploadBlobFromPathAsync(string localFilePath, string? previousKey = null, CancellationToken ct = default)
    {
        if (!File.Exists(localFilePath))
        {
            _logger.LogWarning("Archivo no encontrado en la ruta: {Path}", localFilePath);
            return null;
        }

        var ext = Path.GetExtension(localFilePath).ToLower();
        var newKey = GenerateUniqueKey(ext);

        if (!string.IsNullOrEmpty(previousKey))
        {
            await DeleteBlobAsync(previousKey, ct);
        }

        await using var stream = File.OpenRead(localFilePath);
        var putArgs = new PutObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(newKey)
            .WithStreamData(stream)
            .WithObjectSize(new FileInfo(localFilePath).Length)
            .WithContentType("application/octet-stream");

        await _minioClient.PutObjectAsync(putArgs, ct);
        _logger.LogInformation("⬆ Archivo subido desde path a MinIO: {Key}", newKey);

        return newKey;
    }

    public async Task<string?> PresignedGetUrlAsync(string key, CancellationToken ct = default)
    {
        try
        {
            var args = new PresignedGetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(key)
                .WithExpiry(3600);

            var url = await _minioClient.PresignedGetObjectAsync(args);
            return url;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar URL presignada para MinIO con la clave: {Key}", key);
            return null;
        }
    }

    public async Task DeleteBlobAsync(string key, CancellationToken ct = default)
    {
        try
        {
            var args = new RemoveObjectArgs().WithBucket(_bucketName).WithObject(key);
            await _minioClient.RemoveObjectAsync(args, ct);
            _logger.LogInformation("🗑 Archivo eliminado de MinIO: {Key}", key);
        }
        catch (Exception ex)
        {
            // Se captura la excepción para evitar que rompa el flujo si se intenta 
            // borrar un archivo que ya no existe (ej. al limpiar el previousKey).
            _logger.LogWarning(ex, "Error al intentar eliminar el archivo de MinIO con la clave: {Key}. Es posible que ya no exista.", key);
        }
    }

    public async Task<bool> ValidateBlobExistenceAsync(string key, CancellationToken ct = default)
    {
        try
        {
            var args = new StatObjectArgs().WithBucket(_bucketName).WithObject(key);
            await _minioClient.StatObjectAsync(args, ct);
            return true;
        }
        catch
        {
            return false;
        }
    }
}