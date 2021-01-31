using System;

namespace Authorizer.Domain.Entities
{
    public class Account
    {
        public bool ActiveCard { get; }
        public int AvailableLimit { get; }

        public Account(int availableLimit, bool activeCard)
        {
            if (availableLimit < 0) throw new ArgumentException(
                $"{nameof(availableLimit)} must be postive"
            );

            AvailableLimit = availableLimit;
            ActiveCard = activeCard;
        }
    }
}
