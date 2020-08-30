using System;

namespace vtb.Utils
{
    public interface ITenantIdProvider
    {
        Guid TenantId { get; set; }
    }
}