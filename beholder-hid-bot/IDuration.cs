namespace beholder_hid_bot
{
  public interface IDuration
  {
    uint Delay
    {
      get;
      set;
    }

    uint Min
    {
      get;
      set;
    }

    uint Max
    {
      get;
      set;
    }
  }
}