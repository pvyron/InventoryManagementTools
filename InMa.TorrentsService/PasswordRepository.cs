using CredentialManagement;

namespace InMa.TorrentsService;

public static class PasswordRepository
{
    private const string PasswordName = "vyron_torrents_storage_account_connection_string";

    public static void SavePassword(string password)
    {
        using var cred = new Credential();
        cred.Password = password;
        cred.Target = PasswordName;
        cred.Type = CredentialType.Generic;
        cred.PersistanceType = PersistanceType.LocalComputer;
        cred.Save();
    }

    public static string GetPassword()
    {
        using var cred = new Credential();
        cred.Target = PasswordName;
        cred.Load();
        return cred.Password;
    }
}