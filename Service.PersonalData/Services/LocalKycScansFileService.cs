using Service.PersonalData.Abstractions;
using Service.PersonalData.Models;
using Shared.Grpc.PersonalData.Models;

namespace Service.PersonalData.Services;

public class LocalKycScansFileService : IKycScansFileExplorer
{
    private readonly string _scansFolderPath;

    public LocalKycScansFileService(string scansFolderPath)
    {
        _scansFolderPath = scansFolderPath;
    }

    public async Task SaveKycScan(KycScanCreateGrpcModel kycScanGrpcModel, string fileName)
    {
        var path = $"{_scansFolderPath}/{fileName}{kycScanGrpcModel.FileExtension}";

        await using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        fs.Write(kycScanGrpcModel.Content, 0, kycScanGrpcModel.Content.Length);
    }

    public void DeleteKycScan(KycScanDatabaseModel kycScan)
    {
        var path = $"{_scansFolderPath}/{kycScan.FileName}{kycScan.FileExtension}";

        if (File.Exists(path))
            File.Delete(path);
    }

    public async Task<byte[]> GetKyсScanContentAsync(KycScanDatabaseModel kycScan)
    {
        var path = $"{_scansFolderPath}/{kycScan.FileName}{kycScan.FileExtension}";

        return await File.ReadAllBytesAsync(path);
    }
}
