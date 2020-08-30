using System;

namespace vtb.Utils
{
    public class ValueTenantIdProvider : ITenantIdProvider
    {
        private Guid? _tenantId = null;

        public Guid TenantId
        {
            get
            {
                return _tenantId != null ? _tenantId.Value : Guid.Empty;
            }

            set
            {
                if (_tenantId != null)
                    throw new InvalidOperationException("TenantId can be set only once.");

                _tenantId = value;
            }
        }

        public ValueTenantIdProvider()
        {
            _tenantId = null;
        }

        public ValueTenantIdProvider(Guid tenantId)
        {
            _tenantId = tenantId;
        }
    }
}