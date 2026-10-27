using Application.Services.Blob;
using Application.Services.Users.Create;
using Domain.Entities.Users;


namespace Application.Services.Users;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IBlobService _blobService;

    public UserService(IUserRepository userRepository , IBlobService blobService)
    {
        _userRepository = userRepository;
        _blobService = blobService;
    }

    public async Task CreateUserAsync(CreateUserRequest request, CancellationToken ct = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            BirthDay = request.BirthDay,
            Gender = request.Gender,
            Weight = request.Weight,
            Height = request.Height,
            BMI = request.BMI,
            BodyFatPercentage = request.BodyFatPercentage,
            Age = request.Age
            
        };

        if (request.BodyMetricsRequest is { } m)
        {
            user.BodyMetrics = new BodyMetrics
            {
                Chest = m.Chest,
                Waist = m.Waist,
                Hips = m.Hips,
                Neck = m.Neck,
                LeftArm = m.LeftArm,
                RightArm = m.RightArm,
                LeftForearm = m.LeftForearm,
                RightForearm = m.RightForearm,
                LeftThigh = m.LeftThigh,
                RightThigh = m.RightThigh,
                LeftCalf = m.LeftCalf,
                RightCalf = m.RightCalf,
                UserId = user.Id,
                Date = today,
            };
        }

        user.WeightRecords.Add(new WeightRecord
        {
            UserId = user.Id,
            Weight = request.Weight,
            Date = today,
        });

        var uploadedKeys = new List<string>();
        if (request is null)
            throw new Exception("request es null");

        if (request.Images is null)
            throw new Exception("request.Images es null");

        if (user.ProgressPhotos is null)
            throw new Exception("user.ProgressPhotos es null");
        try
        {
            foreach (var file in request.Images)
            {
                var key = await _blobService.UploadBlobAsync(file);
                uploadedKeys.Add(key);

                user.ProgressPhotos.Add(new ProgressPhoto
                {
                    Id = key,
                    UserId = user.Id,
                    Date = today,
                });
            }

            await _userRepository.CreateUserAsync(user);
        }
        catch
        {
            foreach (var key in uploadedKeys)
                await _blobService.DeleteBlobAsync(key.ToString()); // limpieza
            throw;
        }
    }
}
