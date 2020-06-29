namespace ChristmasPi.Data.Interfaces {
    public interface IRedirectable {
        string ShouldRedirect(string controller, string action, string method);
    }
}