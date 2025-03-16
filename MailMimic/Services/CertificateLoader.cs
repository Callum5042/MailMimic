using System.Security.Cryptography.X509Certificates;

namespace MailMimic.Services;

public class CertificateLoader : ICertificateLoader
{
    public X509Certificate2 GetDevelopmentCertificate()
    {
        using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        store.Open(OpenFlags.ReadOnly);

        var certs = store.Certificates.Find(X509FindType.FindBySubjectName, "localhost", validOnly: false);
        if (!certs.Any())
        {
            throw new Exception("Unable to find X509 certificate");
        }

        return certs[0];
    }

    public X509Certificate2 LoadCertificate(string thumbprint)
    {
        using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        store.Open(OpenFlags.ReadOnly);

        var certs = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, validOnly: true);
        if (!certs.Any())
        {
            throw new Exception("Unable to find X509 certificate");
        }

        return certs[0];
    }
}
