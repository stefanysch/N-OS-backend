namespace N_OS.Application.Pagination;

public class PageQuery
{
    public const int PageSizeMaximo = 100;
    public const int PageSizePadrao = 20;

    private int _page = 1;
    private int _pageSize = PageSizePadrao;

    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value < 1 ? PageSizePadrao : Math.Min(value, PageSizeMaximo);
    }

    public string? Sort { get; set; }

    public string Dir { get; set; } = "asc";

    public string? Q { get; set; }

    public bool Descendente => string.Equals(Dir, "desc", StringComparison.OrdinalIgnoreCase);
}
