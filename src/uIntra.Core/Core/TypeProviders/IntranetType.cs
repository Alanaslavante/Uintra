﻿namespace uIntra.Core.TypeProviders
{
    public class IntranetType : IIntranetType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override int GetHashCode()
        {
            return Id ^ Name.GetHashCode();
        }
    }
}
