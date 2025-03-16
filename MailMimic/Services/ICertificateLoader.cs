using System.Security.Cryptography.X509Certificates;

namespace MailMimic.Services;

public interface ICertificateLoader
{
    X509Certificate2 LoadCertificate(string thumbprint);

    X509Certificate2 GetDevelopmentCertificate();
}
