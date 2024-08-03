Console.WriteLine(TestIt(FileAccess.Read));

static bool TestIt<TEnum>(TEnum x) where TEnum : struct, Enum
{
    return false;
}