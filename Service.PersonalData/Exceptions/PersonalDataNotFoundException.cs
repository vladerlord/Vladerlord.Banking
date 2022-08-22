namespace Service.PersonalData.Exceptions;

public class PersonalDataNotFoundException : Exception
{
    public PersonalDataNotFoundException(Guid id) : base($"Personal data with id: {id} not found")
    {
    }
}
