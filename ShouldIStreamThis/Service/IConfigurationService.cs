namespace ShouldIStreamThis.Service
{
    public interface IConfigurationService
    {
        bool CheckIfTOMLFileExist();
        void MakeTOMLFile();
        T ReadTOMLField<T>(string fieldName);
        void UpdateTOMLField<T>(string fieldName, T newValue);
    }
}
