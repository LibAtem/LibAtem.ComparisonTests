﻿using BMDSwitcherAPI;
using LibAtem.Common;

namespace LibAtem.ComparisonTests2.Util
{
    public static class Conversion
    {
        public static _BMDSwitcherInputAvailability AvailabilityToSdk(SourceAvailability src, MeAvailability me)
        {
            return (_BMDSwitcherInputAvailability) (((int) src << 2) + me);
        }
    }
}