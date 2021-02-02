namespace Authorizer.Application
{
    public class CreateAccount : Operation
    {
        public bool ActiveCard { get; }
        public int AvailableLimit { get; }

        public CreateAccount(bool activeCard, int availableLimit)
        {
            ActiveCard = activeCard;
            AvailableLimit = availableLimit;
        }
    }
}
