namespace SiteYonetimi.API.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class RequirePageAttribute : Attribute
{
    public string PageName { get; }
    public RequirePageAttribute(string pageName) => PageName = pageName;
}
