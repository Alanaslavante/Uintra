﻿
namespace uCommunity.Core
{
     public interface ITimezoneOffsetProvider
    {
        void SetTimezoneOffset(int offsetInMinutes);
        int GetTimezoneOffset();
    }
}