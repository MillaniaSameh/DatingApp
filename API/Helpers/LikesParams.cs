namespace API.Helpers;

public class LikesParams : PaginationParams
{
  public int UserId { get; set; } = 100;
  public string Predicate { get; set; }
}
