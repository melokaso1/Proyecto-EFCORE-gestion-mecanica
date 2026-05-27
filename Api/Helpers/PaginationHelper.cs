namespace Api.Helpers;

public static class PaginationHelper
{
    public static void AddPaginationHeader(HttpResponse response, int total, int page, int size)
    {
        response.Headers["X-Total-Count"] = total.ToString();
        response.Headers["X-Page-Number"] = page.ToString();
        response.Headers["X-Page-Size"] = size.ToString();
    }
}
