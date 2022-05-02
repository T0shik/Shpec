namespace Shpec;

public static partial class CommonPredicates
{
    #region numeric

    /// <summary>
    /// checks that the number is more than 0
    /// </summary>
    public static bool positive(int _) => _ > 0;
    /// <summary>
    /// checks that the number is less than 0
    /// </summary>
    public static bool negative(int _) => _ < 0;
    /// <summary>
    /// checks that the number is zero
    /// </summary>
    public static bool zero(int _) => _ == 0;
    /// <summary>
    /// checks that the number is even
    /// </summary>
    public static bool even(int _) => _ % 2 == 0;
    /// <summary>
    /// checks that the number is odd
    /// </summary>
    public static bool odd(int _) => _ % 2 == 1;

    #endregion

    # region object

    public static bool nil(object o) => o == null;
    public static bool present(object o) => o != null;

    #endregion
}