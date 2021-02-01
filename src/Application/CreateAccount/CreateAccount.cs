namespace Authorizer.Application
{
    public class CreateAccount : Operation
    {
        public bool ActiveCard { get; set; }
        public int AvailableLimit { get; set; }
    }
}
