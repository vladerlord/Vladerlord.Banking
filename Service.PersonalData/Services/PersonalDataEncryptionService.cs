using Service.PersonalData.Models;

namespace Service.PersonalData.Services;

public class PersonalDataEncryptionService
{
    private readonly EncryptionService _encryptionService;

    public PersonalDataEncryptionService(EncryptionService encryptionService)
    {
        _encryptionService = encryptionService;
    }

    public void Encrypt(PersonalDataDatabaseModel model)
    {
        model.FirstName = _encryptionService.Encrypt(model.FirstName, model.Iv);
        model.LastName = _encryptionService.Encrypt(model.LastName, model.Iv);
    }

    public void Decrypt(PersonalDataDatabaseModel model)
    {
        model.FirstName = _encryptionService.Decrypt(model.FirstName, model.Iv);
        model.LastName = _encryptionService.Decrypt(model.LastName, model.Iv);
    }
}
