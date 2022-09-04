using Service.PersonalData.Models;

namespace Service.PersonalData.Abstractions;

public interface IPersonalDataEncryptionService
{
    void Encrypt(PersonalDataDatabaseModel model);
    void Decrypt(PersonalDataDatabaseModel model);
}
