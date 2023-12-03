using System.Security.Cryptography;
using System.Text;

using Microsoft.Extensions.Options;

namespace NSeguin.Dev.AdventOfCode;

internal class SessionAccessor
{
    public SessionAccessor(IOptionsMonitor<AdventOfCodeSessionSettings> settings)
    {
        Settings = settings;
        UpdateSessionId();
        settings.OnChange((_, _) => UpdateSessionId());
    }

    public Session Session { get; private set; } = null!;
    private IOptionsMonitor<AdventOfCodeSessionSettings> Settings { get; }

    private void UpdateSessionId()
    {
        SetSessionId(Settings.CurrentValue.SessionId);
    }

    private void SetSessionId(string? sessionId)
    {
        ArgumentNullException.ThrowIfNull(sessionId, nameof(sessionId));
        Session = new Session(
            sessionId,
            Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(sessionId))));
    }
}
