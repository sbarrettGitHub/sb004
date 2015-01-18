using System;
namespace SB004.Data
{
    public interface IRepository
    {
        SB004.Domain.ISeed AddSeed(SB004.Domain.ISeed seed);
    }
}
