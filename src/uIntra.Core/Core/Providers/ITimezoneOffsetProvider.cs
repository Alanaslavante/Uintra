﻿
namespace Uintra.Core
{
    public interface ITimezoneOffsetProvider
    {
        bool HasTimeZoneOffset();
        void SetTimezoneOffset(int offsetInMinutes);
        int GetTimezoneOffset();
    }
}
