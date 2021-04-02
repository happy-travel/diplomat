using System;
using HappyTravel.Diplomat.Abstractions;

namespace HappyTravel.Diplomat
{
    public class DiplomatFactory : IDiplomatFactory
    {
        public DiplomatFactory(ISettingsProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }


        public Diplomat Create()
        {
            var provider = GetProvider();
            return new Diplomat(provider);
        }


        private ISettingsProvider GetProvider()
        {
            if (!_isSet)
            {
                _provider!.SetSettings();
                _isSet = true;
            }

            return _provider!;
        }


        private static bool _isSet;

        private readonly ISettingsProvider? _provider;
    }
}
